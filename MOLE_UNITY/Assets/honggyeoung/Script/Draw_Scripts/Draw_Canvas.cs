using UnityEngine;
using UnityEngine.EventSystems;

public class Draw_Canvas : MonoBehaviour, IDragHandler
{
    public RectTransform leftPanel;
    public RectTransform rightPanel;
    public RectTransform parentPanel;
    public RectTransform dragBar;
    //public GameObject dragBarObj;

    private float totalWidth;
    private float halfWidth;

    private float totalheight;

    void Start()
    {
        totalWidth = parentPanel.rect.width;
        totalheight = parentPanel.rect.height;

        halfWidth = (totalWidth - dragBar.rect.width) / 2f;

        // 왼쪽 패널 너비 및 위치
        leftPanel.sizeDelta = new Vector2(totalWidth, totalheight);
        leftPanel.anchoredPosition = new Vector2(0, 0);

        // dragBar 위치 중앙
        dragBar.anchoredPosition = new Vector2(halfWidth, 0);  // Anchor가 중앙이므로 이게 가운데임
        // 오른쪽 패널 너비 및 위치
        float rightWidth = totalWidth - halfWidth - dragBar.rect.width;
        rightPanel.sizeDelta = new Vector2(0, totalheight);
        rightPanel.anchoredPosition = new Vector2(dragBar.rect.width / 2f, 0); // 오른쪽 패널 왼쪽이 dragBar 끝에 붙도록

        this.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentPanel, eventData.position, eventData.pressEventCamera, out localPos);

        float totalWidth = parentPanel.rect.width;
        float dragWidth = dragBar.rect.width;

        // 중심 기준 드래그 위치
        float dragX = Mathf.Clamp(localPos.x, -halfWidth, halfWidth);

        // 패널 크기 조정
        leftPanel.sizeDelta = new Vector2(dragX + halfWidth, leftPanel.sizeDelta.y);
        rightPanel.sizeDelta = new Vector2(totalWidth - dragX - halfWidth, rightPanel.sizeDelta.y);

        // DragBar는 Anchor가 중앙이므로 X = 0으로 두되, 패널 크기 기준으로 유지
        dragBar.anchoredPosition = new Vector2(dragX, 0);
        rightPanel.anchoredPosition = new Vector2(dragWidth / 2f, 0); // dragBar 끝에 위치

        Transform leftChild = leftPanel.transform.Find("Draw_Atoms");
        if (leftChild != null)
        {
            float maxWidth = totalWidth - dragBar.rect.width; // 최대 너비
            float ratio = leftPanel.rect.width / maxWidth; // 0~1 사이
            float scale = Mathf.Lerp(0f, 1f, ratio);       // → 0~20으로 변환

            leftChild.localPosition = new Vector3(leftPanel.rect.width / 2, 0, 0);
            leftChild.localScale = new Vector3(scale, scale, scale);
        }

        Transform rightChild = rightPanel.transform.Find("MoleculeModel");
        if(rightChild != null)
        {
            float maxWidth = totalWidth - dragBar.rect.width; // 최대 너비
            float ratio = rightPanel.rect.width / maxWidth; // 0~1 사이
            float scale = Mathf.Lerp(0f, 2f, ratio);       // → 0~20으로 변환

            rightChild.localPosition = new Vector3(-rightPanel.rect.width / 2, 0, -500);
            rightChild.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void two_panel_clear()
    {
        this.gameObject.SetActive(true);

        totalWidth = parentPanel.rect.width;
        totalheight = parentPanel.rect.height;

        halfWidth = (totalWidth - dragBar.rect.width) / 2f;

        // 왼쪽 패널 너비 및 위치
        leftPanel.sizeDelta = new Vector2(halfWidth, totalheight);
        leftPanel.anchoredPosition = new Vector2(0, 0);

        // dragBar 위치 중앙
        dragBar.anchoredPosition = new Vector2(0, 0);  // Anchor가 중앙이므로 이게 가운데임

        // 오른쪽 패널 너비 및 위치
        float rightWidth = totalWidth - halfWidth - dragBar.rect.width;
        rightPanel.sizeDelta = new Vector2(rightWidth, totalheight);
        rightPanel.anchoredPosition = new Vector2(dragBar.rect.width / 2f, 0); // 오른쪽 패널 왼쪽이 dragBar 끝에 붙도록

        Transform leftChild = leftPanel.transform.Find("Draw_Atoms");
        if (leftChild != null)
        {
            float maxWidth = totalWidth - dragBar.rect.width; // 최대 너비
            float ratio = leftPanel.rect.width / maxWidth; // 0~1 사이
            float scale = Mathf.Lerp(0f, 1f, ratio);       // → 0~20으로 변환

            leftChild.localPosition = new Vector3(leftPanel.rect.width / 2, 0, 0);
            leftChild.localScale = new Vector3(scale, scale, scale);
        }

        foreach (Transform rightChild in rightPanel)
        {
            float maxWidth = totalWidth - dragBar.rect.width; // 최대 너비
            float ratio = rightPanel.rect.width / maxWidth; // 0~1 사이
            float scale = Mathf.Lerp(0f, 2f, ratio);       // → 0~20으로 변환

            rightChild.localPosition = new Vector3(-rightPanel.rect.width / 2, 0, -500);
            rightChild.localScale = new Vector3(scale, scale, scale);
        }
        /*if (rightChild != null)
        {
            float maxWidth = totalWidth - dragBar.rect.width; // 최대 너비
            float ratio = rightPanel.rect.width / maxWidth; // 0~1 사이
            float scale = Mathf.Lerp(0f, 2f, ratio);       // → 0~20으로 변환

            rightChild.localPosition = new Vector3(-rightPanel.rect.width / 2, 0, -100);
            rightChild.localScale = new Vector3(scale, scale, scale);
        }*/
    }

}
