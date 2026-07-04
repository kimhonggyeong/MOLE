using UnityEngine;
using UnityEngine.UI;

public class ArrowToggle : MonoBehaviour
{
    [Header("클릭 트리거(행 버튼)")]
    public Button trigger;

    [Header("펼칠 본문 패널")]
    public GameObject content;

    [Header("화살표 이미지")]
    public Image arrow;

    [Header("스프라이트 교체(선택)")]
    public Sprite collapsedSprite;     // 닫힘
    public Sprite expandedSprite;      // 열림

    [Header("회전(Z축, 도)")]
    public float collapsedZ = 0f;
    public float expandedZ = 270f;

    [Header("초기 상태")]
    public bool startOpen = false;

    [HideInInspector] public bool controlledByGroup = false; // 그룹이 제어 중인지
    private bool isOpen;

    void Awake()
    {
        // 그룹이 제어하지 않을 때만 자기 스스로 클릭 바인딩
        if (!controlledByGroup && trigger)
            trigger.onClick.AddListener(Toggle);
    }

    void OnEnable() => Set(startOpen, true);

    public void Toggle() => Set(!isOpen);

    public void ForceSet(bool open) => Set(open, true); // 그룹에서 강제 설정용

    public void Set(bool open, bool force = false)
    {
        if (!force && isOpen == open) return;
        isOpen = open;

        if (content) content.SetActive(isOpen);

        if (arrow)
        {
            if (collapsedSprite && expandedSprite)
                arrow.sprite = isOpen ? expandedSprite : collapsedSprite;

            var rt = arrow.rectTransform;
            var e = rt.localEulerAngles;
            e.z = isOpen ? expandedZ : collapsedZ;
            rt.localEulerAngles = e;

            // 눌림색 잔상 방지
            arrow.color = Color.white;
        }
    }
}
