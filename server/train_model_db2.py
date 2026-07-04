# train_model_sqlite_full.py
import os
import json
import sqlite3
import numpy as np
import torch
import torch.nn as nn
from sklearn.preprocessing import StandardScaler
import joblib
from typing import List, Tuple

# ─────────────────────────────────────────────────────────────
# 경로/상수
# ─────────────────────────────────────────────────────────────
BASE_DIR    = os.path.dirname(__file__)
LOG_DB_PATH = os.path.join(BASE_DIR, "myDB.db")   # 풀이 로그 DB
QUIZ_DB_PATH= os.path.join(BASE_DIR, "quiz.db")   # 퀴즈 콘텐츠 DB

MODEL_PATH  = os.path.join(BASE_DIR, "quizrec_with_time_sqlite.pt")
SCALER_PATH = os.path.join(BASE_DIR, "time_scaler.pkl")
USERIDX_JSON= os.path.join(BASE_DIR, "user_idx.json")
QUIZIDX_JSON= os.path.join(BASE_DIR, "quiz_idx.json")

EMB_DIM     = 16
LR          = 1e-2
EPOCHS      = 1000
PRINT_EVERY = 200
USE_LOG1P   = False  # 긴 꼬리일 때 True 권장

# ─────────────────────────────────────────────────────────────
# 데이터 로더들
# ─────────────────────────────────────────────────────────────
def load_history_from_logdb(db_path: str) -> List[Tuple[str, int, str, float]]:
    """myDB.db에서 (user_id, quiz_id, status, answered_at) 로드."""
    if not os.path.exists(db_path):
        print(f"[ERR] 로그 DB 파일 없음: {db_path}")
        return []
    conn = cursor = None
    try:
        conn = sqlite3.connect(db_path)
        cursor = conn.cursor()
        cursor.execute("""
            SELECT user_id, quiz_id, status, answered_at
            FROM quiz_log
            WHERE answered_at IS NOT NULL
        """)
        rows = cursor.fetchall()
        return rows
    except sqlite3.Error as e:
        print(f"[ERR] 로그 DB 오류: {e}")
        return []
    finally:
        if cursor: cursor.close()
        if conn: conn.close()

def load_quiz_ids_from_quizdb(db_path: str) -> List[int]:
    """quiz.db에서 전체 quiz_id 목록. 없거나 비면 1..20 반환."""
    if not os.path.exists(db_path):
        print(f"[WARN] 퀴즈 DB 없음: {db_path} → 1..20 사용")
        return list(range(1, 21))
    conn = cursor = None
    try:
        conn = sqlite3.connect(db_path)
        cursor = conn.cursor()
        cursor.execute("SELECT DISTINCT quiz_id FROM quiz ORDER BY quiz_id")
        rows = cursor.fetchall()
        if not rows:
            print("[WARN] quiz.db에 quiz_id 없음 → 1..20 사용")
            return list(range(1, 21))
        return [int(r[0]) for r in rows if r and r[0] is not None]
    except sqlite3.Error as e:
        print(f"[WARN] 퀴즈 DB 오류({e}) → 1..20 사용")
        return list(range(1, 21))
    finally:
        if cursor: cursor.close()
        if conn: conn.close()

# ─────────────────────────────────────────────────────────────
# 모델
# ─────────────────────────────────────────────────────────────
class QuizRecModel(nn.Module):
    def __init__(self, n_users: int, n_quizzes: int, emb_dim: int = EMB_DIM):
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

# ─────────────────────────────────────────────────────────────
# 메인
# ─────────────────────────────────────────────────────────────
def main():
    # 1) 데이터 로드
    history = load_history_from_logdb(LOG_DB_PATH)
    if not history:
        print("[ERR] history가 비어 학습을 중단합니다.")
        raise SystemExit(1)

    quizzes_all = load_quiz_ids_from_quizdb(QUIZ_DB_PATH)

    # 2) 매핑 생성
    users = sorted({u for u, _, _, _ in history})
    user_idx = {u: i for i, u in enumerate(users)}
    quiz_idx = {q: i for i, q in enumerate(quizzes_all)}

    print(f"[INFO] 유저 수: {len(users)} | 퀴즈 수(quiz.db): {len(quizzes_all)}")

    # 3) 전처리 (history 중 quiz_idx에 없는 기록은 건너뜀)
    X_user, X_quiz, X_time, y = [], [], [], []
    skip_cnt = 0
    for u, q, s, t in history:
        q = int(q)
        if q not in quiz_idx:
            # quiz.db에 없는 퀴즈ID 기록은 스킵 (훈련 기준과 불일치)
            skip_cnt += 1
            continue
        X_user.append(user_idx[u])
        X_quiz.append(quiz_idx[q])
        X_time.append(float(t))
        y.append(1.0 if s == "correct" else 0.0)
    if skip_cnt:
        print(f"[WARN] quiz.db에 없는 quiz_id 샘플 {skip_cnt}건 제외")

    if not X_user:
        print("[ERR] 유효한 샘플이 없어 학습을 중단합니다.")
        raise SystemExit(1)

    X_user = torch.LongTensor(X_user)
    X_quiz = torch.LongTensor(X_quiz)
    X_time_np = np.array(X_time, dtype=np.float32).reshape(-1, 1)

    # 4) 시간 특징 변환 (선택: log1p)
    if USE_LOG1P:
        X_time_np = np.log1p(X_time_np)

    scaler = StandardScaler()
    X_time_scaled = scaler.fit_transform(X_time_np)
    X_time_t = torch.from_numpy(X_time_scaled).float()
    y_t = torch.tensor(y, dtype=torch.float32)

    # 5) 모델/옵티마이저
    model = QuizRecModel(n_users=len(users), n_quizzes=len(quizzes_all), emb_dim=EMB_DIM)
    optimizer = torch.optim.Adam(model.parameters(), lr=LR)
    criterion = nn.BCELoss()

    # 6) 학습 루프
    model.train()
    for epoch in range(1, EPOCHS + 1):
        optimizer.zero_grad()
        pred = model(X_user, X_quiz, X_time_t)
        loss = criterion(pred, y_t)
        loss.backward()
        optimizer.step()

        if epoch % PRINT_EVERY == 0 or epoch == 1 or epoch == EPOCHS:
            with torch.no_grad():
                acc = ((pred > 0.5).float() == y_t).float().mean().item()
            print(f"[EPOCH {epoch:4d}] loss={loss.item():.4f} | acc={acc:.3f}")

    # 7) 저장 (모델, 스케일러, 매핑)
    torch.save(model.state_dict(), MODEL_PATH)
    joblib.dump(scaler, SCALER_PATH)
    with open(USERIDX_JSON, "w", encoding="utf-8") as f:
        json.dump(user_idx, f, ensure_ascii=False)
    with open(QUIZIDX_JSON, "w", encoding="utf-8") as f:
        json.dump(quiz_idx, f, ensure_ascii=False)

    print(f"[DONE] 모델 저장: {MODEL_PATH}")
    print(f"[DONE] 스케일러 저장: {SCALER_PATH}")
    print(f"[DONE] 매핑 저장: {USERIDX_JSON}, {QUIZIDX_JSON}")

if __name__ == "__main__":
    main()
