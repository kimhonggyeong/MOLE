using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateAtomAndBondMaterials : MonoBehaviour
{
    void Awake()
    {
        string folderPath = "Assets/Resources/Materials";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 원자 색상 정의
        var atomColors = new Dictionary<string, Color>()
        {
            { "H", Color.white },
            { "C", Color.black },
            { "O", Color.red },
            { "N", Color.blue },
            { "S", Color.yellow },
        };

        foreach (var pair in atomColors)
        {
            string name = pair.Key;
            string filePath = $"{folderPath}/{name}.mat";
            if (!File.Exists(filePath))
            {
                CreateAndSaveMaterial(filePath, pair.Value);
            }
        }

        // 본드 머티리얼
        string bondPath = $"{folderPath}/Bond.mat";
        if (!File.Exists(bondPath))
        {
            CreateAndSaveMaterial(bondPath, Color.gray);
        }

        Debug.Log("✅ 머티리얼 자동 확인 및 생성 완료!");
    }

    void CreateAndSaveMaterial(string path, Color color)
    {
#if UNITY_EDITOR
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = color;
        UnityEditor.AssetDatabase.CreateAsset(mat, path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
