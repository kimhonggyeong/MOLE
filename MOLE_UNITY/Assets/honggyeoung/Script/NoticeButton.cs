using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoticeButton : MonoBehaviour
{
    [Header("버튼")]
    public Button toggleButton;

    [Header("펼쳐질 패널")]
    public GameObject contentPanel;

    [Header("UI 변경용")]
    public Image backgroundImage;
    public TextMeshProUGUI buttonText;

    [Header("화살표(선택)")]
    public Image arrowImage;                 // 없으면 회전/스왑 생략
    public Sprite collapsedSprite;           // 닫힘 상태 스프라이트(선택)
    public Sprite expandedSprite;            // 열림 상태 스프라이트(선택)
    public float collapsedZ = 0f;            // 닫힘 각도(Z)
    public float expandedZ = 270f;          // 열림 각도(Z)

    [HideInInspector] public NoticeToggleManager manager;
    private bool isOpen = false;

    public void NoticeToggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    public void Open(bool force = false)
    {
        if (!force && isOpen) return;
        isOpen = true;

        if (contentPanel) contentPanel.SetActive(true);
        if (backgroundImage) backgroundImage.color = new Color32(60, 120, 242, 255); // 파랑
        if (buttonText) buttonText.color = Color.white;

        ApplyArrow(true);
    }

    // 매니저에서 호출
    public void Close(bool force = false)
    {
        if (!force && !isOpen) return;
        isOpen = false;

        if (contentPanel) contentPanel.SetActive(false);
        if (backgroundImage) backgroundImage.color = new Color32(245, 245, 245, 255); // 연회색
        if (buttonText) buttonText.color = Color.black;

        ApplyArrow(false);
    }

    public bool IsOpen() => isOpen;

    private void ApplyArrow(bool open)
    {
        if (!arrowImage) return;

        // 스프라이트 교체(둘 다 지정된 경우에만)
        if (collapsedSprite && expandedSprite)
            arrowImage.sprite = open ? expandedSprite : collapsedSprite;

        // 회전 적용
        var rt = arrowImage.rectTransform;
        var e = rt.localEulerAngles;
        e.z = open ? expandedZ : collapsedZ;
        rt.localEulerAngles = e;

        // 혹시 모를 tint 잔상 제거
        arrowImage.color = Color.white;
    }
}
