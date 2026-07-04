using UnityEngine;
using System.Collections.Generic;

public class NoticeToggleManager : MonoBehaviour
{
    public List<NoticeToggle> toggleItems = new List<NoticeToggle>();

    private NoticeToggle currentOpen;

    void Start()
    {
        foreach (var item in toggleItems)
        {
            item.manager = this;
            item.Close(); // 시작은 모두 닫힘
        }
    }

    public void OnItemClicked(NoticeToggle clicked)
    {
        // 이미 열린 걸 다시 누른 경우 → 닫기
        if (currentOpen == clicked)
        {
            clicked.Close();
            currentOpen = null;
            return;
        }

        // 다른 거 열면 기존은 닫기
        if (currentOpen != null)
        {
            currentOpen.Close();
        }

        clicked.Open();
        currentOpen = clicked;
    }
}
