using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BondDrawer : MonoBehaviour
{
    public RectTransform canvasRect;
    public GameObject bondPrefab;
    public GameObject Draw_Atoms;
    private List<GameObject> previewBondLines = new List<GameObject>();


    public AtomSpawner spawner; // 🎯 Inspector에서 직접 연결해줘야 함
    public EraserManager eraserManager;

    public GameObject startAtom;
    public bool isDrawingBond = false;

    public int currentBondType = 1; // 1: 단일결합, 2: 이중결합

    [System.Serializable]
    public class BondInfo
    {
        public int atomId1;
        public int atomId2;
        public int bondType = 1; // 기본값: 단일결합
    }

    public List<BondInfo> bonds = new List<BondInfo>();

    public void EnableBondMode(int bondType)
    {
        if(isDrawingBond == false)
        {
            spawner.isSpawning = false;
            eraserManager.isEraseMode = false;
            isDrawingBond = true;
            startAtom = null;
            currentBondType = bondType;
        }
        else if(isDrawingBond == true && currentBondType != bondType)
        {
            spawner.isSpawning = false;
            eraserManager.isEraseMode = false;
            isDrawingBond = true;
            startAtom = null;
            currentBondType = bondType;
        }
        else if(isDrawingBond == true && currentBondType == bondType)
        {
            startAtom = null;
            isDrawingBond = false;
        }
    }

    void Update()
    {
        if (!isDrawingBond) return;

        // 1. 시작 원자까지 선택된 상태일 때만 선 표시
        if (startAtom != null)
        {
            float scaleFactor = Draw_Atoms.transform.localScale.x;
            Vector3 start = startAtom.GetComponent<RectTransform>().anchoredPosition * scaleFactor;
            Vector3 end = GetMouseCanvasPosition() * scaleFactor;
            Vector3 dir = end - start;
            Vector3 mid = (start + end) / 2;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float length = dir.magnitude;

            //float thickness = 3f;
            float baseThickness = 3f;
            float thickness = baseThickness * scaleFactor;
            float offsetAmount = 5f * scaleFactor;
            Vector3 offset = Vector3.Cross(dir.normalized, Vector3.forward) * offsetAmount;

            int requiredLines = currentBondType;

            // 생성 부족하면 추가
            while (previewBondLines.Count < requiredLines)
            {
                GameObject newLine = Instantiate(bondPrefab, canvasRect);
                newLine.transform.SetSiblingIndex(0);
                previewBondLines.Add(newLine);
            }

            // 여분 있으면 제거
            while (previewBondLines.Count > requiredLines)
            {
                Destroy(previewBondLines[previewBondLines.Count - 1]);
                previewBondLines.RemoveAt(previewBondLines.Count - 1);
            }

            // 선 위치/회전 업데이트
            for (int i = 0; i < previewBondLines.Count; i++)
            {
                Vector3 linePos = mid;
                if (currentBondType == 2)
                {
                    linePos += (i == 0 ? offset : -offset);
                }
                else if (currentBondType == 3)
                {
                    if (i == 0) linePos += offset;
                    else if (i == 1) linePos -= offset;
                    // i == 2 는 중앙 (아무 변화 X)
                }

                RectTransform rect = previewBondLines[i].GetComponent<RectTransform>();
                rect.anchoredPosition = linePos;
                rect.sizeDelta = new Vector2(length, thickness);
                rect.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }


        // 3. 마우스 버튼 떼면 결합 시도
        if (Input.GetMouseButtonUp(0))
        {
            if (startAtom == null) return;

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            GameObject endAtom = null;

            foreach (var hit in raycastResults)
            {
                if (hit.gameObject.CompareTag("Atom") && hit.gameObject != startAtom)
                {
                    endAtom = hit.gameObject;
                    break;
                }
            }

            // 4. 결합 생성 or 취소
            if (endAtom != null)
            {
                CreateBondFromUI(startAtom, endAtom);
                Debug.Log("결합 생성!");
            }
            else
            {
                Debug.Log("빈 곳에서 릴리즈됨. 결합 취소.");
            }

            // 5. 임시 선 제거 및 상태 초기화
            if (previewBondLines.Count > 0)
            {
                foreach (var line in previewBondLines)
                {
                    Destroy(line);
                }
                previewBondLines.Clear();
            }


            startAtom = null;
        }
    }

    public void CreateBondFromUI(GameObject atom1, GameObject atom2)
    {

        Color GetColor(string element)
        {
            switch (element)
            {
                case "H": return Color.gray;
                case "C": return Color.black;
                case "O": return Color.red;
                case "N": return Color.blue;
                case "S": return Color.yellow;
                default: return Color.magenta;
            }
        }

        var atomNode1 = spawner.atoms.Find(a => a.uiObject == atom1);
        var atomNode2 = spawner.atoms.Find(a => a.uiObject == atom2);

        Color startColor = GetColor(atomNode1.element);
        Color endColor = GetColor(atomNode2.element);

        int id1 = atomNode1.id;
        int id2 = atomNode2.id;

        /*// 🧠 결합 정보 저장
        int id1 = spawner.atoms.Find(a => a.uiObject == atom1).id;
        int id2 = spawner.atoms.Find(a => a.uiObject == atom2).id;*/

        // ✅ 중복 체크
        bool alreadyExists = bonds.Exists(b =>
            (b.atomId1 == id1 && b.atomId2 == id2) ||
            (b.atomId1 == id2 && b.atomId2 == id1));

        if (alreadyExists)
        {
            Debug.LogWarning($"[⚠️] 이미 연결된 원자 쌍입니다: {id1}-{id2}");
            return;
        }

        Vector3 start = atom1.GetComponent<RectTransform>().anchoredPosition;
        Vector3 end = atom2.GetComponent<RectTransform>().anchoredPosition;
        Vector3 mid = (start + end) / 2;
        Vector3 dir = end - start;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float length = dir.magnitude;
        float thickness = 3f;

        // 선 그리기
        if (currentBondType == 1)
        {
            CreateBondLine(mid, angle, length, thickness, startColor, endColor);
        }
        else if (currentBondType == 2)
        {
            Vector3 offset = Vector3.Cross(dir.normalized, Vector3.forward) * 5f;
            CreateBondLine(mid + offset, angle, length, thickness, startColor, endColor);
            CreateBondLine(mid - offset, angle, length, thickness, startColor, endColor);
        }
        else if (currentBondType == 3)
        {
            Vector3 offset = Vector3.Cross(dir.normalized, Vector3.forward) * 5f;
            CreateBondLine(mid, angle, length, thickness, startColor, endColor);
            CreateBondLine(mid + offset, angle, length, thickness, startColor, endColor);
            CreateBondLine(mid - offset, angle, length, thickness, startColor, endColor);
        }

        /*Vector3 start = atom1.GetComponent<RectTransform>().anchoredPosition;
        Vector3 end = atom2.GetComponent<RectTransform>().anchoredPosition;
        Vector3 direction = end - start;
        Vector3 midPoint = (start + end) / 2;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float length = direction.magnitude;

        float thickness = 3f; // ✅ 더 얇은 결합선
        float offsetAmount = 5f; // 선 간격
        Vector3 offset = Vector3.Cross(direction.normalized, Vector3.forward) * offsetAmount;

        if (currentBondType == 1)
        {
            // 단일결합
            CreateBondLine(midPoint, angle, length, thickness);
        }
        else if (currentBondType == 2)
        {
            // 이중결합 (평행한 선 2개)
            CreateBondLine(midPoint + offset, angle, length, thickness);
            CreateBondLine(midPoint - offset, angle, length, thickness);
        }
        else if (currentBondType == 3)
        {
            // 삼중결합 (중앙선 + 양쪽)
            CreateBondLine(midPoint, angle, length, thickness);
            CreateBondLine(midPoint + offset, angle, length, thickness);
            CreateBondLine(midPoint - offset, angle, length, thickness);
        }*/

        BondInfo bondInfo = new BondInfo
        {
            atomId1 = id1,
            atomId2 = id2,
            bondType = currentBondType
        };
        bonds.Add(bondInfo);

        Debug.Log($"[✔] 결합 저장됨: {id1} - {id2}, 타입: {currentBondType}");
    }
    /*void CreateBondLine(Vector3 position, float angle, float length, float thickness)
    {
        GameObject bond = Instantiate(bondPrefab, Draw_Atoms.transform);
        bond.transform.SetSiblingIndex(0);
        RectTransform rect = bond.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(length, thickness);  // ✅ 얇은 결합선
        rect.localRotation = Quaternion.Euler(0, 0, angle);
    }*/
    void CreateBondLine(Vector3 position, float angle, float length, float thickness, Color startColor, Color endColor)
    {
        GameObject bond = new GameObject("BondLine", typeof(RectTransform));
        bond.transform.SetParent(Draw_Atoms.transform, false);
        bond.transform.SetSiblingIndex(0);
        bond.tag = "Bond";

        RectTransform bondRect = bond.GetComponent<RectTransform>();
        bondRect.anchoredPosition = position;
        bondRect.sizeDelta = new Vector2(length, thickness);
        bondRect.localRotation = Quaternion.Euler(0, 0, angle);

        bond.AddComponent<BondClickHandler>();

        float leftWidth = length * 0.3f;
        float midWidth = length * 0.4f;
        float rightWidth = length * 0.3f;

        // Left (시작색)
        GameObject left = CreateSegment("Left", bond.transform, leftWidth, thickness, new Vector2(-length * 0.5f + leftWidth * 0.5f, 0), startColor);

        // Middle (검정)
        GameObject mid = CreateSegment("Middle", bond.transform, midWidth, thickness, Vector2.zero, Color.black);

        // Right (끝색)
        GameObject right = CreateSegment("Right", bond.transform, rightWidth, thickness, new Vector2(length * 0.5f - rightWidth * 0.5f, 0), endColor);
    }
    GameObject CreateSegment(string name, Transform parent, float width, float height, Vector2 anchoredPos, Color color)
    {
        GameObject seg = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Image));
        seg.transform.SetParent(parent, false);
        RectTransform rt = seg.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, height);
        rt.anchoredPosition = anchoredPos;
        rt.localRotation = Quaternion.identity;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        UnityEngine.UI.Image img = seg.GetComponent<UnityEngine.UI.Image>();
        img.color = color;

        return seg;
    }

    Vector3 GetMouseCanvasPosition()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Draw_Atoms.GetComponent<RectTransform>(), // 📌 기준 좌표계를 Draw_Atoms로 변경
            Input.mousePosition,
            Camera.main,
            out localPoint);

        return localPoint;
    }
}
