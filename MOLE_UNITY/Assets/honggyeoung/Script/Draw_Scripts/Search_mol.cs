using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Search_mol : MonoBehaviour
{
    private const string MoleculeLayerName = "MoleculeOnly";
    private GameObject moleculeParent;

    [Header("Scene Refs")]
    public Transform targetParent;                 // Search_Root
    public TMP_InputField nameInput;
    public TMP_Text errorText;

    [Header("Server")]
    //public string fromNameAllUrl = "http://localhost:5001/from_name_all";
    private string serverUrl;

    [Header("UI Images")]
    public RawImage mol3DView;                     // RenderTexture가 물린 RawImage
    public RawImage neutralImage;                  // 2D 중성
    public RawImage ionizedImage;                  // 2D 이온화
    public RectTransform panel3WayView;            // 3분할 패널

    public string molname;

    const float BondRadius = 0.07f;   // 본드 굵기
    const float BondGap = 0.12f;   // 이중/삼중 간격

    void Awake()
    {
        if (mol3DView) mol3DView.raycastTarget = false;
        if (neutralImage) neutralImage.raycastTarget = false;
        if (ionizedImage) ionizedImage.raycastTarget = false;

        EnsureFlexibleWidth(mol3DView);
        EnsureFlexibleWidth(neutralImage);
        EnsureFlexibleWidth(ionizedImage);

        ForceOpaque(mol3DView);
        ForceOpaque(neutralImage);
        ForceOpaque(ionizedImage);
    }

    private void EnsureFlexibleWidth(Graphic g)
    {
        if (!g) return;
        var le = g.GetComponent<LayoutElement>() ?? g.gameObject.AddComponent<LayoutElement>();
        le.flexibleWidth = 1f;
    }
    private void ForceOpaque(Graphic g)
    {
        if (!g) return;
        var c = g.color; if (c.a < 0.99f) { c.a = 1f; g.color = c; }
    }


    public void search_mol_name()
    {
        if (errorText) errorText.text = "";
        molname = nameInput.text;
        StartCoroutine(LoadMoleculeByName(molname));
    }

    IEnumerator LoadMoleculeByName(string name)
    {
        serverUrl = PlayerPrefs.GetString("ServerUrl");

        string url = $"{serverUrl + "/search/from_name"}?name={UnityWebRequest.EscapeURL(name)}";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            Debug.Log("요청 주소: " + url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Molecule load failed: " + www.error);
                if (errorText)
                {
                    if (www.error == "Cannot connect to destination host") errorText.text = "서버연결 실패";
                    else if (www.error == "HTTP/1.1 400 Bad Request") errorText.text = "이름을 입력해 주세요";
                    else if (www.error == "HTTP/1.1 404 Not Found") errorText.text = "존재하지 않는 이름";
                    else errorText.text = www.error;
                }
                yield break;
            }

            var json = JsonUtility.FromJson<FullMolResponse>(www.downloadHandler.text);

            // 🔎 여기서 먼저 Base64 문자열이 같은지 체크
            bool sameB64 = json.img2d_neutral == json.img2d_ionized;
            Debug.Log($"[Check] neutral vs ionized base64 same? {sameB64}");

            var neutralTex = DecodeDataUrlToTexture(json.img2d_neutral, $"NeutralTexture_{molname}");
            var ionTex = DecodeDataUrlToTexture(json.img2d_ionized, $"IonizedTexture_{molname}");
            Debug.Log($"[RefEq] same Texture instance? {ReferenceEquals(neutralTex, ionTex)}");

            // 3D
            RenderMol(json.mol3d);

            // 2D 이미지 (안전 디코드 + 비율 맞춤)
            //var neutralTex = DecodeDataUrlToTexture(json.img2d_neutral, $"NeutralTexture_{molname}");
            if (neutralTex)
            {
                Debug.Log($"Neutral size: {neutralTex.width}x{neutralTex.height}");
                neutralImage.texture = neutralTex;
                SetAspectFromTexture(neutralImage, neutralTex);
                neutralImage.enabled = true;
                neutralImage.gameObject.SetActive(true);
            }
            else Debug.LogError("Neutral LoadImage 실패");

            //var ionTex = DecodeDataUrlToTexture(json.img2d_ionized, $"IonizedTexture_{molname}");
            if (ionTex)
            {
                Debug.Log($"Ionized size: {ionTex.width}x{ionTex.height}");
                ionizedImage.texture = ionTex;
                SetAspectFromTexture(ionizedImage, ionTex);
                ionizedImage.enabled = true;
                ionizedImage.gameObject.SetActive(true);
            }
            else Debug.LogError("Ionized LoadImage 실패");

            if (panel3WayView) LayoutRebuilder.ForceRebuildLayoutImmediate(panel3WayView);
            LogRects();

            if (!string.IsNullOrEmpty(json.ionization_note))
                Debug.Log($"[Ionization] note = {json.ionization_note}");
        }
    }

    // Base64 → Texture
    private Texture2D DecodeDataUrlToTexture(string dataOrBase64, string texName)
    {
        if (string.IsNullOrEmpty(dataOrBase64)) return null;
        string b64 = dataOrBase64;
        int comma = dataOrBase64.IndexOf(',');
        if (comma >= 0) b64 = dataOrBase64.Substring(comma + 1);
        b64 = b64.Replace("\n", "").Replace("\r", "").Trim();

        byte[] bytes;
        try { bytes = System.Convert.FromBase64String(b64); }
        catch (System.Exception e) { Debug.LogError("Base64 디코드 실패: " + e.Message); return null; }

        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!tex.LoadImage(bytes, true)) return null;
        tex.name = texName;
        return tex;
    }

    private void SetAspectFromTexture(RawImage img, Texture2D tex)
    {
        if (!img || !tex) return;
        var arf = img.GetComponent<AspectRatioFitter>() ?? img.gameObject.AddComponent<AspectRatioFitter>();
        arf.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
        arf.aspectRatio = tex.width > 0 && tex.height > 0 ? (float)tex.width / tex.height : 1f;
    }

    private void LogRects()
    {
        if (!mol3DView || !neutralImage || !ionizedImage) return;
        var r0 = mol3DView.rectTransform.rect;
        var r1 = neutralImage.rectTransform.rect;
        var r2 = ionizedImage.rectTransform.rect;
        Debug.Log($"Rect Mol3D=({r0}) | Neutral=({r1}) | Ionized=({r2})");
    }

    // ===== 3D 렌더 =====
    class AtomData { public string element; public Vector3 position; }
    List<AtomData> atoms = new List<AtomData>();

    public void RenderMol(string mol)
    {
        atoms.Clear();

        GameObject[] objects = GameObject.FindGameObjectsWithTag("DrawModel");
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }

        moleculeParent = new GameObject("MoleculeModel");
        moleculeParent.transform.SetParent(targetParent, false); // 부모 지정
        moleculeParent.transform.position = Vector3.zero;
        moleculeParent.AddComponent<Draw_Move3D>();
        moleculeParent.tag = "DrawModel";


        string[] lines = mol.Split('\n');
        if (lines.Length < 4) return;

        int atomCount = int.Parse(lines[3].Substring(0, 3));
        int bondCount = int.Parse(lines[3].Substring(3, 3));

        int atomStart = 4;
        int bondStart = atomStart + atomCount;

        // 🔵 원자 생성
        for (int i = 0; i < atomCount; i++)
        {
            string line = lines[atomStart + i];
            float x = float.Parse(line.Substring(0, 10));
            float y = float.Parse(line.Substring(10, 10));
            float z = float.Parse(line.Substring(20, 10));
            string element = line.Substring(31, 3).Trim();

            Vector3 pos = new Vector3(x, y, z);
            atoms.Add(new AtomData { element = element, position = pos });

            GameObject atom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            atom.transform.position = pos;
            atom.transform.localScale = Vector3.one * 0.5f;
            atom.name = element;

            // ✅ 머티리얼 로드
            Material mat = Resources.Load<Material>("Materials/" + element);
            if (mat != null)
                atom.GetComponent<Renderer>().material = mat;
            else
                atom.GetComponent<Renderer>().material.color = Color.magenta;

            atom.transform.SetParent(moleculeParent.transform);
        }

        // 🔗 결합 생성
        for (int i = 0; i < bondCount; i++)
        {
            string line = lines[bondStart + i];
            int a1 = int.Parse(line.Substring(0, 3)) - 1;
            int a2 = int.Parse(line.Substring(3, 3)) - 1;
            int bondType = int.Parse(line.Substring(6, 3));

            string element1 = atoms[a1].element;
            string element2 = atoms[a2].element;

            CreateBond(atoms[a1].position, atoms[a2].position, bondType, element1, element2); // ✅ element 넘김
        }

        moleculeParent.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        Vector3 moleculepos = mol3DView.transform.position;
        moleculeParent.transform.position = new Vector2(moleculepos.x, moleculepos.y);
    }

    void CreateBond(Vector3 pos1, Vector3 pos2, int bondType, string element1, string element2)
    {
        Vector3 dir = (pos2 - pos1);
        float length = dir.magnitude;
        Vector3 up = dir.normalized;
        Vector3 center = (pos1 + pos2) / 2;

        Vector3 offset = Vector3.Cross(up, Vector3.up);
        if (offset == Vector3.zero) offset = Vector3.Cross(up, Vector3.right);
        offset = offset.normalized * 0.07f;

        Vector3[] offsets = bondType == 1 ? new Vector3[] { Vector3.zero }
                          : bondType == 2 ? new Vector3[] { offset, -offset }
                          : new Vector3[] { offset, -offset, Vector3.zero };

        foreach (var off in offsets)
        {
            GameObject bond = new GameObject("BondSegment");
            bond.transform.SetParent(moleculeParent.transform);
            bond.transform.position = center + off;
            bond.transform.rotation = Quaternion.LookRotation(Vector3.forward, up);

            float seg1 = length * 0.45f;
            float seg2 = length * 0.10f;
            float seg3 = length * 0.45f;

            CreateSegment(bond.transform, pos1 + off, up, seg1, element1);
            CreateSegment(bond.transform, pos1 + up * seg1 + off, up, seg2, "Bond");  // 검정색 중간
            CreateSegment(bond.transform, pos1 + up * (seg1 + seg2) + off, up, seg3, element2);
        }
    }
    void CreateSegment(Transform parent, Vector3 center, Vector3 direction, float height, string element)
    {
        GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        seg.transform.SetParent(parent);
        seg.transform.position = center + direction.normalized * (height / 2f);
        seg.transform.up = direction;
        seg.transform.localScale = new Vector3(0.1f, height / 2f, 0.1f);

        Material mat = Resources.Load<Material>("Materials/" + element);
        if (mat != null)
            seg.GetComponent<Renderer>().material = mat;
        else
            seg.GetComponent<Renderer>().material.color = element == "Bond" ? Color.black : Color.magenta;
    }

    [System.Serializable]
    class FullMolResponse
    {
        public string mol3d;
        public string img2d_neutral;
        public string img2d_ionized;
        public string name;
        public string smiles;
        public string ionization_note; // "no_change" 등
    }
}
