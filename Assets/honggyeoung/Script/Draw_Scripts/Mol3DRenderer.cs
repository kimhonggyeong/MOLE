using System.Collections.Generic;
using UnityEngine;

public class Mol3DRenderer : MonoBehaviour
{
    private GameObject moleculeParent;
    public Transform targetParent;
    public Draw_Canvas draw_Canvas;
    class AtomData
    {
        public string element;
        public Vector3 position;
    }

    List<AtomData> atoms = new List<AtomData>();

    /*void Awake()
    {
        moleculeParent = new GameObject("MoleculeModel");
        moleculeParent.transform.position = Vector3.zero;
        moleculeParent.AddComponent<Move3D>();
        moleculeParent.tag = "DrawModel";
    }*/

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

        /*foreach (Transform child in moleculeParent.transform)
        {
            Destroy(child.gameObject);
        }*/

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

        /* // 🔗 결합 생성
         for (int i = 0; i < bondCount; i++)
         {
             string line = lines[bondStart + i];
             int a1 = int.Parse(line.Substring(0, 3)) - 1;
             int a2 = int.Parse(line.Substring(3, 3)) - 1;
             int bondType = int.Parse(line.Substring(6, 3));

             CreateBond(atoms[a1].position, atoms[a2].position, bondType);
         }*/

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
        moleculeParent.transform.position = new Vector3(0f, 1f, 0f);

        draw_Canvas.two_panel_clear();
    }

    /*void CreateBond(Vector3 pos1, Vector3 pos2, int bondType)
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
            CreateCylinderBond(center, up, length);
        }
        else if (bondType == 2)
        {
            CreateCylinderBond(center + offset, up, length);
            CreateCylinderBond(center - offset, up, length);
        }
        else if (bondType == 3)
        {
            CreateCylinderBond(center, up, length);
            CreateCylinderBond(center + offset, up, length);
            CreateCylinderBond(center - offset, up, length);
        }
    }*/

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

            float seg1 = length * 0.4f;
            float seg2 = length * 0.2f;
            float seg3 = length * 0.4f;

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


    /*void CreateCylinderBond(Vector3 center, Vector3 direction, float length)
    {
        GameObject bond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bond.transform.position = center;
        bond.transform.up = direction;
        bond.transform.localScale = new Vector3(0.1f, length / 2f, 0.1f);

        // ✅ 본드 머티리얼 적용
        Material bondMat = Resources.Load<Material>("Materials/Bond");
        if (bondMat != null)
            bond.GetComponent<Renderer>().material = bondMat;
        else
            bond.GetComponent<Renderer>().material.color = Color.gray;

        bond.transform.SetParent(moleculeParent.transform);
    }*/
}
