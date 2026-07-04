using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavAccordionHighlighter : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public Button button;            // 상위 버튼 (비극성/극성/…)
        public TMP_Text label;           // 상위 라벨
        public Image icon;               // 상위 아이콘
        public RectTransform content;    // 펼쳐질 패널(하위 리스트 루트)
    }

    public List<Item> items = new List<Item>();

    public Color selected = new Color32(0x3D, 0x6C, 0xFF, 0xFF); // 파랑
    public Color normal = new Color32(0xA6, 0xAE, 0xBD, 0xFF); // 회색

    // 현재 열린 상위 항목 인덱스(-1 = 없음)
    private int selectedIndex = -1;

    // 하위 버튼 저장(자동 연결용)
    private readonly Dictionary<Button, TMP_Text> subBtnToText = new Dictionary<Button, TMP_Text>();

    void Awake()
    {
        // 상위 버튼 리스너 연결
        for (int i = 0; i < items.Count; i++)
        {
            int idx = i;
            var it = items[i];

            if (!it.label) it.label = it.button.GetComponentInChildren<TMP_Text>(true);
            if (!it.icon)
            {
                foreach (var img in it.button.GetComponentsInChildren<Image>(true))
                    if (img.gameObject != it.button.gameObject) { it.icon = img; break; }
            }

            it.button.transition = Selectable.Transition.None;
            it.button.onClick.RemoveAllListeners();   // 혹시 남아있던 것 제거(중복 방지)
            it.button.onClick.AddListener(() => Toggle(idx));
        }

        // 하위 버튼 자동 연결(있으면)
        AutoBindSubButtons();
    }

    void Start()
    {
        // 처음 진입: 전부 접고, 모든 텍스트 색을 normal로
        CollapseAll();
        ApplyNormalToAllTexts();
    }

    // 상위 버튼 토글
    public void Toggle(int index)
    {
        if (selectedIndex == index)
        {
            // 같은 항목 다시 클릭 → 접기 + 하위 초기화
            SetItemVisual(index, false);
            if (items[index].content)
            {
                ResetSubTexts(items[index].content);
                items[index].content.gameObject.SetActive(false);
            }
            selectedIndex = -1;
        }
        else
        {
            // 이전 열린 항목 닫기
            if (selectedIndex != -1)
            {
                SetItemVisual(selectedIndex, false);
                if (items[selectedIndex].content)
                {
                    ResetSubTexts(items[selectedIndex].content);
                    items[selectedIndex].content.gameObject.SetActive(false);
                }
            }
            // 새 항목 열기
            selectedIndex = index;
            SetItemVisual(index, true);
            if (items[index].content) items[index].content.gameObject.SetActive(true);
        }

        // 레이아웃 재계산(겹침 방지)
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    // 하위 버튼 클릭 시 하이라이트(선택 1개만 파랑)
    private void OnSubItemClicked(Button btn)
    {
        // 같은 그룹(현재 열린 content) 안의 하위 텍스트를 모두 normal로
        if (selectedIndex != -1 && items[selectedIndex].content)
            ResetSubTexts(items[selectedIndex].content);

        // 방금 누른 하위만 파랑
        if (subBtnToText.TryGetValue(btn, out var t) && t) t.color = selected;
    }

    private void SetItemVisual(int i, bool on)
    {
        var it = items[i];
        if (it.label) it.label.color = on ? selected : normal;
        if (it.icon) it.icon.color = on ? selected : normal;
    }

    private void ResetSubTexts(RectTransform root)
    {
        if (!root) return;
        var texts = root.GetComponentsInChildren<TMP_Text>(true);
        foreach (var t in texts) t.color = normal;
    }

    private void CollapseAll()
    {
        for (int i = 0; i < items.Count; i++)
        {
            var it = items[i];
            if (it.content) it.content.gameObject.SetActive(false);
            SetItemVisual(i, false);
        }
        selectedIndex = -1;
    }

    private void ApplyNormalToAllTexts()
    {
        // 상위
        foreach (var it in items)
        {
            if (it.label) it.label.color = normal;
            if (it.icon) it.icon.color = normal;
            if (it.content) ResetSubTexts(it.content);
        }
        // 안전장치: 이 오브젝트 하위의 모든 TMP_Text를 한번 더 회색으로
        foreach (var t in GetComponentsInChildren<TMP_Text>(true))
            t.color = normal;
    }

    private void AutoBindSubButtons()
    {
        subBtnToText.Clear();
        foreach (var it in items)
        {
            if (!it.content) continue;
            var childButtons = it.content.GetComponentsInChildren<Button>(true);
            foreach (var b in childButtons)
            {
                // Button에 달린 텍스트 자동 추적
                TMP_Text txt = b.GetComponentInChildren<TMP_Text>(true);
                if (txt) subBtnToText[b] = txt;

                // 중복 리스너 방지
                b.onClick.RemoveListener(() => OnSubItemClicked(b)); // RemoveListener에 람다 비교는 안 되므로 아래 방식 사용 불가
                // 안전하게 모두 제거 후 다시 연결
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => OnSubItemClicked(b));
            }
        }
    }

    // 외부에서 완전 초기화하고 싶을 때
    public void SelectNone()
    {
        CollapseAll();
        ApplyNormalToAllTexts();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}


