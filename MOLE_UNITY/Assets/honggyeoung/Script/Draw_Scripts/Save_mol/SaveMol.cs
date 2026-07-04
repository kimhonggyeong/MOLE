/*using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SaveMol : MonoBehaviour
{
    public string moleculeName = "alanine";
    public SaveMolRender rendererPrefab; // ✅ 빈 프리팹 참조
    public string serverUrl = "http://localhost:5000/from_name";

    void Start()
    {
        StartCoroutine(LoadMoleculeByName(moleculeName));
    }

    IEnumerator LoadMoleculeByName(string name)
    {
        string url = $"{serverUrl}?name={UnityWebRequest.EscapeURL(name)}";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Molecule load failed: " + www.error);
            yield break;
        }

        // 🔸 JSON 파싱
        var json = JsonUtility.FromJson<MolResponse>(www.downloadHandler.text);

        // 🔸 렌더러 생성
        SaveMolRender renderer = Instantiate(rendererPrefab);
        renderer.RenderMol(json.mol, name);

        // ✅ 프리팹 저장
*//*#if UNITY_EDITOR
        string path = $"Assets/Draw_Mol/Draw_Prefab/Mol/{name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(GetComponent<Renderer>().gameObject, path);
        Debug.Log($"Prefab saved: {path}");
#endif*//*
    }

    [System.Serializable]
    class MolResponse
    {
        public string mol;
        public string smiles;
        public string formula;
        public string name;
    }
}
*/