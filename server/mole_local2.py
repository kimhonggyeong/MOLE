# app_combined.py
# -*- coding: utf-8 -*-

"""
필수/권장 환경
--------------
Python 3.10+ 권장

pip install fastapi uvicorn[standard] slowapi pydantic requests
pip install transformers torch --extra-index-url https://download.pytorch.org/whl/cu121  # CUDA 환경일 때 예시
pip install rdkit-pypi                      # (or conda-forge rdkit 권장)
pip install deep-translator                 # 번역
# 선택: Open Babel(pybel)를 쓰고 싶다면
#  - 시스템에 openbabel 설치(권장: conda install -c conda-forge openbabel)
#  - Python 바인딩: pip install openbabel-wheel

실행
----
python app_combined.py
# or
uvicorn app_combined:app --host 0.0.0.0 --port 8000

환경변수
--------
- NCBI_API_KEY : (선택) PubMed E-utilities API Key
- HF_MODEL_ID  : (선택) 허깅페이스 모델 ID (기본: "pleyel/chatbot_test3")
"""

import os
import time
import base64
import logging
import calendar
import urllib.parse
from io import BytesIO
from typing import Optional, List, Dict, Any

import requests
from fastapi import FastAPI, Request, APIRouter, Query, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
from pydantic import BaseModel
from slowapi import Limiter
from slowapi.util import get_remote_address
from slowapi.errors import RateLimitExceeded

# ---------------- RDKit ----------------
from rdkit import Chem
from rdkit.Chem import AllChem, Draw, rdMolDescriptors
from rdkit.Chem.MolStandardize import rdMolStandardize as std

# ------------- Open Babel (optional) -------------
try:
    from openbabel import pybel  # type: ignore
    HAVE_PYBEL = True
except Exception:
    HAVE_PYBEL = False

# ------------- Transformers (chat) -------------
import torch
from transformers import PreTrainedTokenizerFast, GPT2LMHeadModel

# ------------- 번역 -------------
from deep_translator import GoogleTranslator
import xml.etree.ElementTree as ET

# ------------- 추천 서비스용 추가 의존성 -------------
import sqlite3
import joblib
import numpy as np
import torch.nn as nn

# ------------------------- 로그 설정 -------------------------
LOG_DIR = "./logs"
os.makedirs(LOG_DIR, exist_ok=True)
log_file_path = os.path.join(LOG_DIR, "chat_server.log")

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    handlers=[
        logging.StreamHandler(),
        logging.FileHandler(log_file_path, mode='a', encoding='utf-8')
    ]
)

# ------------------------- 환경 -------------------------
EUTILS_API_KEY = os.getenv("NCBI_API_KEY")
HEADERS = {"User-Agent": "PaperTranslator/1.0 (contact: you@example.com)"}
RATE_LIMIT_SEC = 0.34  # NCBI 권고

HF_MODEL_ID = os.getenv("HF_MODEL_ID", "pleyel/chatbot_test3")

# ------------------------- FastAPI 초기화 -------------------------
app = FastAPI(title="Unified Chem/Chat/Papers/QuizRec API")
limiter = Limiter(key_func=get_remote_address)
app.state.limiter = limiter

@app.exception_handler(RateLimitExceeded)
async def rate_limit_handler(request: Request, exc: RateLimitExceeded):
    logging.warning(f"⛔ 속도 제한 초과: {request.client.host}")
    return JSONResponse(status_code=429, content={"error": "Too Many Requests"})

# ------------------------- CORS -------------------------
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # 필요 시 도메인 제한
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# =========================================================
# 공통 유틸
# =========================================================
def translate(text: str) -> str:
    if not text:
        return ""
    try:
        return GoogleTranslator(source="auto", target="ko").translate(text)
    except Exception as e:
        logging.warning(f"[번역 오류] {e}")
        return text

def parse_pub_date(article_root) -> str:
    """
    PubDate는 형식이 제각각이라 Year/Month/Day, MedlineDate 등을 순서대로 시도
    """
    try:
        pub_date_elem = article_root.find(".//PubDate")
        if pub_date_elem is None:
            return "날짜 정보 없음"

        # 1) 일반 Year/Month/Day 조합
        year = pub_date_elem.findtext("Year", "")
        month_raw = pub_date_elem.findtext("Month", "")
        day = pub_date_elem.findtext("Day", "01")

        month_map = {name: f"{num:02d}" for num, name in enumerate(calendar.month_abbr) if name}
        month = month_map.get((month_raw[:3] if month_raw else "").capitalize(), "01") if month_raw else "01"

        if year:
            return f"{year}-{month}-{day}"

        # 2) MedlineDate (예: "2019 Jan-Feb")
        medline = pub_date_elem.findtext("MedlineDate", "")
        if medline:
            import re
            m = re.search(r"\b(19|20)\d{2}\b", medline)
            if m:
                return f"{m.group(0)}-01-01"
            return medline
    except Exception as e:
        logging.warning(f"[발행일 파싱 오류] {e}")

    return "날짜 정보 없음"

# =========================================================
# PubChem Helpers
# =========================================================
def fetch_pubchem_cid(name: str) -> Optional[str]:
    try:
        url = f"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/{urllib.parse.quote(name)}/cids/TXT"
        r = requests.get(url, timeout=15)
        if r.status_code != 200:
            return None
        cid = r.text.strip().splitlines()[0].strip()
        return cid or None
    except Exception:
        return None

def fetch_pubchem_sdf(name: str, record_type: str = "3d") -> Optional[str]:
    cid = fetch_pubchem_cid(name)
    if not cid:
        return None
    url = f"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/{cid}/SDF?record_type={record_type}"
    r = requests.get(url, timeout=20)
    return r.text if r.status_code == 200 and r.text.strip() else None

# =========================================================
# RDKit utility
# =========================================================
def ensure_2d_coords(m: Chem.Mol) -> Chem.Mol:
    if not m.GetNumConformers():
        AllChem.Compute2DCoords(m)
    return m

def mol_to_base64_png(m: Chem.Mol, size=(300, 300), with_prefix=True) -> str:
    ensure_2d_coords(m)
    img = Draw.MolToImage(m, size=size)
    buf = BytesIO()
    img.save(buf, format="PNG")
    b64 = base64.b64encode(buf.getvalue()).decode("utf-8")
    return f"data:image/png;base64,{b64}" if with_prefix else b64

def obabel_ionize_sdf_to_sdf(sdf_text: str, ph: float) -> Optional[str]:
    if not HAVE_PYBEL:
        return None
    try:
        mol = pybel.readstring("sdf", sdf_text)
        # 좌표가 없으면 임시 좌표 생성
        if mol.OBMol.NumConformers() == 0:
            mol.make3D()
        mol.OBMol.AddHydrogens()
        mol.OBMol.CorrectForPH(float(ph))
        ion_sdf = mol.write("sdf")
        return ion_sdf
    except Exception:
        return None

def rdkit_reionize_copy(m: Chem.Mol) -> Chem.Mol:
    x = Chem.Mol(m)
    x = std.Cleanup(x)
    x = std.Reionize(x)
    Chem.SanitizeMol(x)
    ensure_2d_coords(x)
    return x

def rdkit_neutral_copy(m: Chem.Mol) -> Chem.Mol:
    x = Chem.Mol(m)
    x = std.Cleanup(x)
    uncharger = std.Uncharger()
    x = uncharger.uncharge(x)
    Chem.SanitizeMol(x)
    ensure_2d_coords(x)
    return x

# =========================================================
# Chat (Transformers)
# =========================================================
logging.info("🔄 Hugging Face 모델 로딩 중...")
tokenizer = PreTrainedTokenizerFast.from_pretrained(HF_MODEL_ID)
model = GPT2LMHeadModel.from_pretrained(HF_MODEL_ID)
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
model = model.to(device)
model.eval()
logging.info("✅ 모델 로딩 완료")

class ChatRequest(BaseModel):
    message: str

# =========================================================
# Routes: Chemistry
# =========================================================
@app.post("/model/compose")
def chem_from_json(payload: Dict[str, Any]):
    """
    입력:
    {
      "atoms": [{"id": 1, "element": "C"}, ...],
      "bonds": [{"atom1": 1, "atom2": 2, "bondType": 1}, ...]
    }
    출력: mol block, smiles, formula, name(IUPAC; PubChem 조회)
    """
    try:
        atoms = payload["atoms"]
        bonds = payload["bonds"]

        mol = Chem.RWMol()
        atom_id_map = {}

        for atom in atoms:
            rd_atom = Chem.Atom(atom["element"])
            idx = mol.AddAtom(rd_atom)
            atom_id_map[atom["id"]] = idx

        for bond in bonds:
            a1 = atom_id_map[bond["atom1"]]
            a2 = atom_id_map[bond["atom2"]]
            bond_type = bond.get("bondType", 1)
            if bond_type == 1:
                bond_order = Chem.BondType.SINGLE
            elif bond_type == 2:
                bond_order = Chem.BondType.DOUBLE
            elif bond_type == 3:
                bond_order = Chem.BondType.TRIPLE
            else:
                bond_order = Chem.BondType.SINGLE
            mol.AddBond(a1, a2, bond_order)

        mol = mol.GetMol()
        Chem.SanitizeMol(mol)
        # 3D 좌표 부여 + UFF 최적화
        molH = Chem.AddHs(mol)
        AllChem.EmbedMolecule(molH, randomSeed=42)
        AllChem.UFFOptimizeMolecule(molH)

        mol_block = Chem.MolToMolBlock(molH)  # 👈 H가 포함된 MolBlock
        smiles = Chem.MolToSmiles(molH)
        formula = rdMolDescriptors.CalcMolFormula(molH)

        # PubChem IUPAC 이름 조회
        name = "Unknown"
        encoded_smiles = urllib.parse.quote(smiles)
        try:
            url = f"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/smiles/{encoded_smiles}/property/IUPACName/JSON"
            response = requests.get(url, timeout=15)
            if response.status_code == 200:
                json_data = response.json()
                name = json_data["PropertyTable"]["Properties"][0]["IUPACName"]
        except Exception as e:
            logging.warning(f"PubChem 이름 조회 실패: {e}")

        return {
            "mol": mol_block,
            "smiles": smiles,
            "formula": formula,
            "name": name
        }
    except KeyError:
        return JSONResponse(status_code=400, content={"error": "Invalid payload schema"})
    except Exception as e:
        logging.exception("chem_from_json error")
        return JSONResponse(status_code=500, content={"error": str(e)})

@app.get("/search/from_name")
def chem_from_name(name: str):
    """
    name 쿼리로 PubChem 2D/3D 불러와서:
    - 3D mol block
    - 2D neutral / ionized PNG (base64)
    - SMILES
    """
    name = (name or "").strip()
    if not name:
        return JSONResponse(status_code=400, content={"error": "Missing molecule name"})

    try:
        # 3D
        sdf_3d = fetch_pubchem_sdf(name, "3d")
        if not sdf_3d:
            return JSONResponse(status_code=404, content={"error": "3D record not available"})
        mol3d = Chem.MolFromMolBlock(sdf_3d)
        if mol3d is None:
            return JSONResponse(status_code=500, content={"error": "Invalid 3D SDF"})
        mol3d_block = Chem.MolToMolBlock(mol3d)

        # 2D
        sdf_2d = fetch_pubchem_sdf(name, "2d")
        if not sdf_2d:
            return JSONResponse(status_code=404, content={"error": "2D record not available"})
        mol2d = Chem.MolFromMolBlock(sdf_2d)
        if mol2d is None:
            return JSONResponse(status_code=500, content={"error": "Invalid 2D SDF"})
        Chem.SanitizeMol(mol2d)
        ensure_2d_coords(mol2d)

        neutral = rdkit_neutral_copy(mol2d)

        # pH 7.4 이온화: Open Babel 우선, 실패 시 RDKit Reionize
        ionized_rdkit = None
        ion_sdf = obabel_ionize_sdf_to_sdf(sdf_2d, ph=7.4) if HAVE_PYBEL else None
        if ion_sdf:
            ion_mol = Chem.MolFromMolBlock(ion_sdf)
            if ion_mol:
                Chem.SanitizeMol(ion_mol)
                ensure_2d_coords(ion_mol)
                ionized_rdkit = ion_mol
        if ionized_rdkit is None:
            ionized_rdkit = rdkit_reionize_copy(mol2d)

        img2d_neutral = mol_to_base64_png(neutral, size=(300, 300))
        img2d_ionized = mol_to_base64_png(ionized_rdkit, size=(300, 300))

        return {
            "mol3d": mol3d_block,
            "img2d_neutral": img2d_neutral,
            "img2d_ionized": img2d_ionized,
            "name": name,
            "smiles": Chem.MolToSmiles(mol2d),
            "engine": "obabel" if ion_sdf else "rdkit-fallback",
        }
    except Exception as e:
        logging.exception("chem_from_name error")
        return JSONResponse(status_code=500, content={"error": str(e)})

# =========================================================
# Routes: Chat
# =========================================================
@app.post("/chat/generate")
@limiter.limit("5/10seconds")
async def chat_generate(request: Request, body: ChatRequest):
    message = body.message.strip()
    if not message:
        return JSONResponse(status_code=400, content={"error": "메시지가 비어 있습니다."})

    logging.info(f"📩 요청 수신: {message}")

    try:
        inputs = tokenizer(
            message,
            return_tensors="pt",
            max_length=512,
            padding="max_length",
            truncation=True,
        ).to(device)

        max_output_len = 64
        start_time = time.time()
        with torch.no_grad():
            outputs = model.generate(
                input_ids=inputs["input_ids"],
                attention_mask=inputs["attention_mask"],
                max_new_tokens=max_output_len,
                do_sample=True,
                temperature=0.8,
                top_k=50,
                top_p=0.9,
                repetition_penalty=2.0,
                no_repeat_ngram_size=3,
            )
        elapsed = time.time() - start_time

        answer = tokenizer.decode(outputs[0], skip_special_tokens=True).strip()
        tokens = answer.split()
        if len(tokens) > 6 and tokens[:3] == tokens[3:6]:
            answer = "답변이 반복되어 정확히 인식되지 않았습니다. 다시 질문해 주세요."

        logging.info(f"✅ 응답 완료 ({elapsed:.2f}s): {answer[:60]}...")

        return {
            "response": answer,
            "time_taken": round(elapsed, 2),
            "model": HF_MODEL_ID,
        }

    except Exception as e:
        logging.exception("❌ chat_generate 에러")
        return JSONResponse(status_code=500, content={"error": str(e)})

# =========================================================
# Routes: Papers (PubMed + 번역)
# =========================================================
@app.get("/paper/papers")
def translate_paper(query: str,
                    order: str = "relevance",
                    limit: int = 3,
                    translate_flag: bool = True):
    """
    쿼리 파라미터:
      - query (필수)
      - order=relevance|latest (기본 relevance)
      - limit=1..10 (기본 3)
      - translate_flag=true|false (기본 true)
    """
    order = (order or "relevance").lower()
    try:
        limit = max(1, min(int(limit), 10))
    except Exception:
        limit = 3

    do_translate = bool(translate_flag)

    sort_map = {
        "latest": "pub+date",
        "relevance": "relevance",
    }
    sort_value = sort_map.get(order, "relevance")

    # 1) PMID 검색
    search_url = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi"
    search_params = {
        "db": "pubmed",
        "term": query,
        "retmax": limit,
        "retmode": "json",
        "sort": sort_value,
    }
    if EUTILS_API_KEY:
        search_params["api_key"] = EUTILS_API_KEY

    try:
        r = requests.get(search_url, params=search_params, headers=HEADERS, timeout=15)
        r.raise_for_status()
        id_list = r.json().get("esearchresult", {}).get("idlist", [])
    except Exception as e:
        return JSONResponse(status_code=500, content={"error": f"PubMed 검색 오류: {str(e)}"})

    results = []

    # 2) 각 PMID 상세 조회
    for pmid in id_list:
        try:
            fetch_url = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi"
            fetch_params = {"db": "pubmed", "id": pmid, "retmode": "xml"}
            if EUTILS_API_KEY:
                fetch_params["api_key"] = EUTILS_API_KEY

            fr = requests.get(fetch_url, params=fetch_params, headers=HEADERS, timeout=20)
            fr.raise_for_status()

            root = ET.fromstring(fr.text)
            article = root.find(".//PubmedArticle")
            art_data = article.find(".//Article") if article is not None else None
            if art_data is None:
                continue

            # 제목
            title_en = art_data.findtext("ArticleTitle", default="(No Title)")
            title_ko = translate(title_en) if do_translate else None

            # 초록 (라벨 포함 결합)
            parts = []
            for node in art_data.findall(".//Abstract/AbstractText"):
                label = node.get("Label")
                text = "".join(node.itertext()).strip()
                parts.append(f"{label}: {text}" if label else text)
            abstract_en = " ".join([p for p in parts if p]) if parts else ""
            abstract_ko = translate(abstract_en) if (do_translate and abstract_en) else (None if do_translate else "")

            # 저자
            authors_en, authors_ko = [], []
            for author in art_data.findall(".//Author"):
                first = author.findtext("ForeName")
                last = author.findtext("LastName")
                if first and last:
                    full = f"{first} {last}"
                    authors_en.append(full)
                    if do_translate:
                        try:
                            authors_ko.append(translate(full))
                        except Exception:
                            authors_ko.append(full)

            # 유형
            type_nodes = article.findall(".//PublicationType")
            pub_type = type_nodes[0].text if type_nodes else "유형 없음"

            # 학술지
            journal = art_data.findtext(".//Journal/Title", default="(No Journal)")

            # 발행일
            published = parse_pub_date(article)

            # 페이지
            pages = article.findtext(".//Pagination/MedlinePgn", default="(No Pages)")

            results.append({
                "pmid": pmid,
                "title_en": title_en,
                "title_ko": title_ko,
                "abstract_en": abstract_en,
                "abstract_ko": abstract_ko,
                "authors_en": authors_en,
                "authors_ko": authors_ko if do_translate else None,
                "type": pub_type,
                "journal": journal,
                "published": published,
                "pages": pages,
                "link": f"https://pubmed.ncbi.nlm.nih.gov/{pmid}/",
            })

            time.sleep(RATE_LIMIT_SEC)  # NCBI rate limit
        except Exception as e:
            logging.warning(f"[⚠️] PMID {pmid} 처리 중 오류: {e}")
            continue

    return {
        "query": query,
        "order": order,
        "limit": limit,
        "translate": do_translate,
        "results": results,
    }

# =========================================================
# Health
# =========================================================
@app.get("/healthz")
def healthz():
    return {"ok": True}

# =========================================================
# Router: Quiz Recommendation (mounted)
# =========================================================
BASE_DIR    = os.path.dirname(__file__)
LOG_DB_PATH = os.path.join(BASE_DIR, "myDB.db")   # 풀이 로그 DB
QUIZ_DB_PATH= os.path.join(BASE_DIR, "quiz.db")   # 퀴즈 콘텐츠 DB
MODEL_PATH  = os.path.join(BASE_DIR, "models", "trained_models", "quizrec_with_time_db.pt")
SCALER_PATH = os.path.join(BASE_DIR, "models", "trained_models", "time_scaler.pkl")

quizrec_router = APIRouter(tags=["quizrec"])

def _table_columns(conn, table: str) -> List[str]:
    cur = conn.cursor()
    cur.execute(f"PRAGMA table_info({table})")
    cols = [r[1] for r in cur.fetchall()]
    cur.close()
    return cols

def load_history_from_logdb() -> (list, str):
    if not os.path.exists(LOG_DB_PATH):
        logging.warning(f"[WARN] 로그 DB 없음: {LOG_DB_PATH}")
        return [], ""

    conn = cursor = None
    try:
        conn = sqlite3.connect(LOG_DB_PATH)
        cols = _table_columns(conn, "quiz_log")
        cursor = conn.cursor()

        if {"user_id","quiz_id","is_correct","time_taken"}.issubset(set(cols)):
            cursor.execute("SELECT user_id, quiz_id, is_correct, time_taken FROM quiz_log")
            rows = cursor.fetchall()
            return rows, "A"
        elif {"user_id","quiz_id","status","answered_at"}.issubset(set(cols)):
            cursor.execute("SELECT user_id, quiz_id, status, answered_at FROM quiz_log")
            rows = cursor.fetchall()
            return rows, "B"
        else:
            logging.error(f"[ERR] quiz_log 스키마 감지 실패. 실제 컬럼: {cols}")
            return [], ""
    except sqlite3.Error as e:
        logging.error(f"[ERR] 로그 DB 오류: {e}")
        return [], ""
    finally:
        if cursor: cursor.close()
        if conn: conn.close()

def load_quiz_ids_from_quizdb() -> List[int]:
    if not os.path.exists(QUIZ_DB_PATH):
        logging.warning(f"[WARN] 퀴즈 DB 없음: {QUIZ_DB_PATH} → 1..160 사용")
        return list(range(1, 161))

    conn = cursor = None
    try:
        conn = sqlite3.connect(QUIZ_DB_PATH)
        cursor = conn.cursor()
        cursor.execute("SELECT DISTINCT quiz_id FROM quiz ORDER BY quiz_id")
        rows = cursor.fetchall()
        if not rows:
            logging.warning("[WARN] quiz.db에 quiz_id 없음 → 1..160 사용")
            return list(range(1, 161))
        return [int(r[0]) for r in rows if r and r[0] is not None]
    except sqlite3.Error as e:
        logging.warning(f"[WARN] 퀴즈 DB 오류({e}) → 1..160 사용")
        return list(range(1, 161))
    finally:
        if cursor: cursor.close()
        if conn: conn.close()

class QuizRecModel(nn.Module):
    def __init__(self, n_users: int, n_quizzes: int, emb_dim: int = 16):
        super().__init__()
        self.user_emb = nn.Embedding(max(n_users, 1), emb_dim)
        self.quiz_emb = nn.Embedding(max(n_quizzes, 1), emb_dim)
        self.fc = nn.Sequential(
            nn.Linear(emb_dim * 2 + 1, 32),
            nn.ReLU(),
            nn.Linear(32, 1),
            nn.Sigmoid()
        )
    def forward(self, user_idx, quiz_idx, time_feat):
        u = self.user_emb(user_idx)
        q = self.quiz_emb(quiz_idx)
        x = torch.cat([u, q, time_feat], dim=1)
        return self.fc(x).squeeze(1)

rec_device = torch.device("cpu")
logging.info(f"[quizrec] device = {rec_device}")

_history, _schema = load_history_from_logdb()
if not _history:
    logging.warning("[quizrec] history 비어 있음(또는 로드 실패). 추천 품질이 제한될 수 있음.")

_users = sorted({row[0] for row in _history}) if _history else []
_quizzes_all = load_quiz_ids_from_quizdb()

_user_idx = {u: i for i, u in enumerate(_users)}
_quiz_idx = {q: i for i, q in enumerate(_quizzes_all)}

_rec_model = None
if _users and _quizzes_all:
    try:
        _rec_model = QuizRecModel(len(_users), len(_quizzes_all)).to(rec_device)
        _rec_model.load_state_dict(torch.load(MODEL_PATH, map_location=rec_device))
        _rec_model.eval()
        logging.info(f"[quizrec] 모델 로드 OK: {MODEL_PATH}")
    except FileNotFoundError:
        logging.error(f"[quizrec] 모델 파일 없음: {MODEL_PATH}")
        _rec_model = None
    except Exception as e:
        logging.exception(f"[quizrec] 모델 로드 오류: {e}")
        _rec_model = None
else:
    logging.error("[quizrec] 사용자/퀴즈 목록 비어 모델 초기화 생략")

try:
    _scaler = joblib.load(SCALER_PATH)
    logging.info(f"[quizrec] 스케일러 로드 OK: {SCALER_PATH}")
except FileNotFoundError:
    logging.warning(f"[quizrec] 스케일러 없음: {SCALER_PATH} → 비스케일링")
    _scaler = None
except Exception as e:
    logging.warning(f"[quizrec] 스케일러 로드 오류: {e}")
    _scaler = None

def _scale_time(avg_time: float) -> float:
    if _scaler is None:
        return float(avg_time)
    arr = np.array([[avg_time]], dtype=np.float32)
    return float(_scaler.transform(arr)[0][0])

def _user_avg_time(u_id: str) -> float:
    if not _history:
        return 30.0
    col = 3  # time_taken 또는 answered_at
    times = []
    for row in _history:
        if row[0] != u_id:
            continue
        try:
            times.append(float(row[col]))
        except:
            continue
    return (sum(times) / len(times)) if times else 30.0

def _recommend_quizzes_logic(user_id_str: str, candidate_quiz_ids: List[int], avg_time: float, top_n: int = 3) -> List[int]:
    if _rec_model is None:
        raise HTTPException(status_code=503, detail="Model is not available.")
    if user_id_str not in _user_idx:
        raise HTTPException(status_code=404, detail=f"Unknown user: {user_id_str}")

    t_scaled = _scale_time(avg_time)
    t_batch = torch.FloatTensor([t_scaled] * len(candidate_quiz_ids)).unsqueeze(1).to(rec_device)

    try:
        ui = torch.LongTensor([_user_idx[user_id_str]] * len(candidate_quiz_ids)).to(rec_device)
        qi = torch.LongTensor([_quiz_idx[q] for q in candidate_quiz_ids]).to(rec_device)
    except KeyError as e:
        missing = int(str(e).strip("'"))
        raise HTTPException(status_code=400, detail=f"Unknown quiz_id in candidates: {missing}")

    with torch.no_grad():
        prob_correct = _rec_model(ui, qi, t_batch).cpu().numpy()
    prob_wrong = 1.0 - prob_correct
    ranked = sorted(zip(prob_wrong, candidate_quiz_ids), reverse=True)
    return [qid for _, qid in ranked[:top_n]]

@quizrec_router.get("/recommend", summary="사용자 맞춤 퀴즈 추천")
def recommend_endpoint(user_id: str = Query(..., description="추천을 받을 사용자 ID"),
                       top_n: int = Query(3, ge=1, le=50, description="추천 개수")):
    if not _quizzes_all:
        return {"user_id": user_id, "recommended_quizzes": []}

    attempted = set()
    for row in _history:
        if row[0] == user_id:
            try:
                attempted.add(int(row[1]))
            except:
                continue

    candidates = [q for q in _quizzes_all if q not in attempted]
    if not candidates:
        return {"user_id": user_id, "recommended_quizzes": []}

    avg_time = _user_avg_time(user_id)
    recs = _recommend_quizzes_logic(user_id, candidates, avg_time, top_n)
    return {"user_id": user_id, "recommended_quizzes": recs}

# 메인 앱에 라우터 부착
app.include_router(quizrec_router)

# =========================================================
# Run
# =========================================================
def run():
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)

if __name__ == "__main__":
    run()
