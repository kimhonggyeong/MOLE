/*using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveMolRender : MonoBehaviour
{
    private GameObject moleculeParent;

    class AtomData
    {
        public string element;
        public Vector3 position;
    }

    List<AtomData> atoms = new List<AtomData>();

    public void RenderMol(string mol, string molName)
    {
        atoms.Clear();

        moleculeParent = new GameObject("MoleculeModel");
        moleculeParent.transform.position = Vector3.zero;
        moleculeParent.AddComponent<Move3D>();
        moleculeParent.tag = "DrawModel";

        foreach (Transform child in moleculeParent.transform)
        {
            Destroy(child.gameObject);
        }

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
            atom.transform.localScale = Vector3.one * 0.7f;
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

            CreateBond(atoms[a1].position, atoms[a2].position, bondType);
        }

        moleculeParent.transform.localScale = new Vector3(1f, 1f, 1f);
        moleculeParent.transform.position = new Vector3(0f, 1f, 0f);

        string path = $"Assets/Draw_Mol/Draw_Prefab/Mol/{molName}.prefab";
        PrefabUtility.SaveAsPrefabAsset(moleculeParent, path);
        Debug.Log($"Prefab saved: {path}");
    }

    void CreateBond(Vector3 pos1, Vector3 pos2, int bondType)
    {
        Vector3 dir = pos2 - pos1;
        Vector3 center = (pos1 + pos2) / 2;
        Vector3 up = dir.normalized;
        float length = dir.magnitude;

        Vector3 offset = Vector3.Cross(up, Vector3.up);
        if (offset == Vector3.zero) offset = Vector3.Cross(up, Vector3.right);
        offset = offset.normalized * 0.07f;

        if (bondType == 1)
        {
            CreateCylinderBond(center, up, length, 0.2f);
        }
        else if (bondType == 2)
        {
            CreateCylinderBond(center + offset, up, length, 0.1f);
            CreateCylinderBond(center - offset, up, length, 0.1f);
        }
        else if (bondType == 3)
        {
            CreateCylinderBond(center, up, length, 0.07f);
            CreateCylinderBond(center + offset, up, length, 0.07f);
            CreateCylinderBond(center - offset, up, length, 0.07f);
        }
    }

    void CreateCylinderBond(Vector3 center, Vector3 direction, float length, float weight)
    {
        GameObject bond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bond.transform.position = center;
        bond.transform.up = direction;
        bond.transform.localScale = new Vector3(weight, length / 2f, 0.1f);

        // ✅ 본드 머티리얼 적용
        Material bondMat = Resources.Load<Material>("Materials/Bond");
        if (bondMat != null)
            bond.GetComponent<Renderer>().material = bondMat;
        else
            bond.GetComponent<Renderer>().material.color = Color.gray;

        bond.transform.SetParent(moleculeParent.transform);
    }
}
*/