using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AtomSpawner : MonoBehaviour
{
    public GameObject atomPrefab;             // 생성할 원자 프리팹
    public RectTransform canvasRect;          // Canvas의 RectTransform
    public GameObject Draw_Atoms;
    public Camera uiCamera;                   // Canvas에 연결된 UI 카메라
    public BondDrawer bondDrawer;
    public EraserManager eraserManager;
    public string elementSymbol = "C";        // 원소 기호
    private Color carbonColor = Color.black;   // 원소 색상


    public bool isSpawning = false;

    private int nextAtomId = 0;  // 자동 증가하는 ID
    public List<AtomNode> atoms = new List<AtomNode>();
    
    [System.Serializable]
    public class AtomNode
    {
        public int id;
        public string element;
        public Vector2 position;
        public GameObject uiObject;  // 연결된 UI 오브젝트
    }

    public void ClickSpawnBtn(string elesymbol)
    {
        if (isSpawning == false)
        {
            bondDrawer.isDrawingBond = false;
            eraserManager.isEraseMode = false;
            EnableSpawnMode(elesymbol);
        }
        else if (isSpawning == true && elementSymbol != elesymbol)
        {
            EnableSpawnMode(elesymbol);
        }
        else if (isSpawning == true && elementSymbol == elesymbol)
        {
            isSpawning = false;
        }
    }

    void EnableSpawnMode(string elesymbol)
    {
        isSpawning = true;
        elementSymbol = elesymbol;

        switch (elementSymbol)
        {
            case "H": carbonColor = Color.white; break;
            case "C": carbonColor = Color.black; break;
            case "O": carbonColor = Color.red; break;
            case "N": carbonColor = Color.blue; break;
            case "S": carbonColor = Color.yellow; break;
            default: carbonColor = Color.magenta; break; // 정의되지 않은 경우
        }
    }

    void Update()
    {
        if (isSpawning && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Draw_Atoms.GetComponent<RectTransform>(),
                Input.mousePosition,
                uiCamera,
                out localPoint))
            {
                // ✅ 보정: 중심 기준으로 변환
                //Vector2 corrected = localPoint - new Vector2(canvasRect.rect.width * 0.5f, 0f);

                GameObject atom = Instantiate(atomPrefab, Draw_Atoms.transform);
                atom.GetComponent<RectTransform>().anchoredPosition = localPoint;

                TextMeshProUGUI atomText = atom.GetComponentInChildren<TextMeshProUGUI>();
                if (atomText != null)
                {
                    atomText.text = elementSymbol;
                    atomText.color = carbonColor;
                }

                // 💾 리스트에 저장
                AtomNode node = new AtomNode
                {
                    id = nextAtomId++,
                    element = elementSymbol,
                    position = localPoint,
                    uiObject = atom
                };
                atoms.Add(node);

                Debug.Log($"[✔] 원자 생성됨: ID={node.id}, Element={node.element}");

                //isSpawning = false;
            }
        }
    }
}