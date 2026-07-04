using System.Collections.Generic;
using UnityEngine;

public class TabUIManager : MonoBehaviour
{
    public List<TabButton> tabButtons;

    private TabButton currentTab;

    void Start()
    {
        foreach (var tab in tabButtons)
        {
            tab.manager = this;
            tab.button.onClick.AddListener(() => OnTabClicked(tab));
        }

        // 기본 첫 탭 선택
        if (tabButtons.Count > 0)
        {
            OnTabClicked(tabButtons[0]);
        }
    }

    public void OnTabClicked(TabButton clickedTab)
    {
        foreach (var tab in tabButtons)
        {
            tab.SetSelected(tab == clickedTab);
        }

        currentTab = clickedTab;
    }
}
