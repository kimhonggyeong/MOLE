using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EraserManager : MonoBehaviour
{
    public AtomSpawner atomSpawner;
    public BondDrawer bondDrawer;
    public Camera uiCamera;
    public RectTransform canvasRect;

    public bool isEraseMode = false;

    void Update()
    {
        // 클릭 판정은 각 오브젝트에서 처리됨
    }

    public void EraseAtomByGameObject(GameObject atomObj)
    {
        var atom = atomSpawner.atoms.Find(a => a.uiObject == atomObj);
        if (atom != null)
        {
            EraseAtom(atom);
        }
    }

    public void EraseBondByGameObject(GameObject bondObj)
    {
        RectTransform clickedRect = bondObj.GetComponent<RectTransform>();
        Vector2 clickedPos = clickedRect.anchoredPosition;

        foreach (var bond in new List<BondDrawer.BondInfo>(bondDrawer.bonds))
        {
            var atom1 = atomSpawner.atoms.Find(a => a.id == bond.atomId1)?.uiObject;
            var atom2 = atomSpawner.atoms.Find(a => a.id == bond.atomId2)?.uiObject;
            if (atom1 == null || atom2 == null) continue;

            Vector2 mid = (atom1.GetComponent<RectTransform>().anchoredPosition +
                           atom2.GetComponent<RectTransform>().anchoredPosition) / 2f;

            if (Vector2.Distance(mid, clickedPos) < 10f)
            {
                EraseBond(bond);
                break;
            }
        }
    }

    void EraseAtom(AtomSpawner.AtomNode atom)
    {
        int id = atom.id;

        List<BondDrawer.BondInfo> bondsToRemove = bondDrawer.bonds.FindAll(b => b.atomId1 == id || b.atomId2 == id);
        foreach (var bond in bondsToRemove)
        {
            EraseBond(bond);
        }

        Destroy(atom.uiObject);
        atomSpawner.atoms.Remove(atom);

        Debug.Log($"[❌] 원자(ID={id}) 삭제됨");
    }

    void EraseBond(BondDrawer.BondInfo bond)
    {
        var atom1 = atomSpawner.atoms.Find(a => a.id == bond.atomId1)?.uiObject;
        var atom2 = atomSpawner.atoms.Find(a => a.id == bond.atomId2)?.uiObject;
        if (atom1 == null || atom2 == null) return;

        Vector2 mid = (atom1.GetComponent<RectTransform>().anchoredPosition +
                       atom2.GetComponent<RectTransform>().anchoredPosition) / 2f;

        GameObject[] allBonds = GameObject.FindGameObjectsWithTag("Bond");
        foreach (var go in allBonds)
        {
            Vector2 pos = go.GetComponent<RectTransform>().anchoredPosition;
            if (Vector2.Distance(pos, mid) < 10f)
            {
                Destroy(go);
            }
        }

        bondDrawer.bonds.Remove(bond);
        Debug.Log($"[❌] 결합({bond.atomId1} - {bond.atomId2}) 삭제됨");
    }

    public void ToggleEraseMode()
    {
        isEraseMode = !isEraseMode;
        if (isEraseMode)
        {
            atomSpawner.isSpawning = false;
            bondDrawer.isDrawingBond = false;
            Debug.Log("[🧽] 삭제 모드");
        }
    }

    public void ResetAll()
    {
        atomSpawner.isSpawning = false;
        bondDrawer.isDrawingBond = false;

        // 1. 모든 결합 삭제
        foreach (var bond in new List<BondDrawer.BondInfo>(bondDrawer.bonds))
        {
            EraseBond(bond);
        }

        // 2. 모든 원자 삭제
        foreach (var atom in new List<AtomSpawner.AtomNode>(atomSpawner.atoms))
        {
            Destroy(atom.uiObject);
        }
        atomSpawner.atoms.Clear();

        isEraseMode = false;

        Debug.Log("[🧼] 전체 초기화 완료");
    }
}
