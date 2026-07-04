using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class TabButtonGroup : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Button button;        // 탭 버튼
        public Image background;    // 버튼 배경(Image, 9-slice 추천)
        public TMP_Text label;       // 버튼 라벨(TMP)
        public GameObject panel;     // (선택) 탭별 내용 패널
    }

    [Header("탭 목록(위→아래 순서)")]
    public List<Tab> tabs = new();

    [Header("색상")]
    public Color normalBg = new Color32(0xEE, 0xF2, 0xF8, 0xFF); // 비선택 배경(연회색 카드)
    public Color selectedBg = new Color32(0x3C, 0x78, 0xF2, 0xFF); // 선택 배경 #3C78F2
    public Color normalText = new Color32(0x75, 0x75, 0x75, 0xFF); // 비선택 글자 #757575
    public Color selectedText = new Color32(0xFF, 0xFF, 0xFF, 0xFF); // 선택 글자 #FFFFFF

    [Header("스프라이트 교체 모드(선택)")]
    public bool useSpriteSwap = false;
    public Sprite normalSprite;
    public Sprite selectedSprite;

    public int defaultIndex = 0;

    // 안전한 리스너 제거를 위한 맵
    private readonly Dictionary<Button, UnityAction> _handlers = new();

    void Awake()
    {
        // 편의: 비어 있으면 자식 Button 자동 수집
        if (tabs.Count == 0)
        {
            foreach (var btn in GetComponentsInChildren<Button>(true))
            {
                var t = new Tab
                {
                    button = btn,
                    background = btn.GetComponent<Image>(),
                    label = btn.GetComponentInChildren<TMP_Text>(true)
                };
                tabs.Add(t);
            }
        }
    }

    void OnEnable()
    {
        // 클릭 핸들러 등록
        for (int i = 0; i < tabs.Count; i++)
        {
            var tab = tabs[i];
            if (tab?.button == null) continue;

            tab.button.transition = Selectable.Transition.None; // 색 충돌 방지
            int idx = i;
            UnityAction act = () => Select(idx);
            tab.button.onClick.AddListener(act);
            _handlers[tab.button] = act;
        }

        if (tabs.Count > 0) Select(Mathf.Clamp(defaultIndex, 0, tabs.Count - 1));
    }

    void OnDisable()
    {
        // 우리가 추가한 리스너만 안전하게 제거
        foreach (var kv in _handlers)
            if (kv.Key) kv.Key.onClick.RemoveListener(kv.Value);
        _handlers.Clear();
    }

    public void Select(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        for (int i = 0; i < tabs.Count; i++)
        {
            bool on = (i == index);
            var t = tabs[i];

            // 배경 처리
            if (t.background)
            {
                if (useSpriteSwap && selectedSprite && normalSprite)
                {
                    t.background.sprite = on ? selectedSprite : normalSprite;
                    t.background.color = Color.white; // 스프라이트 본색 사용
                }
                else
                {
                    t.background.color = on ? selectedBg : normalBg;
                }
            }

            // 라벨 색
            if (t.label) t.label.color = on ? selectedText : normalText;

            // 패널 토글(있을 때만)
            if (t.panel) t.panel.SetActive(on);
        }
    }
}

