using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LeftMenuBridge : MonoBehaviour
{
    public GameObject alarmPanel;
    public GameObject profilePanel;
    public GameObject noticePanel;

    UIDocument ui;
    Button btnAlarm, btnProfile, btnNotice;
    readonly List<Button> all = new();

    void Awake()
    {
        ui = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        var root = ui.rootVisualElement;

        btnAlarm = root.Q<Button>("BtnAlarm");
        btnProfile = root.Q<Button>("BtnProfile");
        btnNotice = root.Q<Button>("BtnNotice");

        all.Clear();
        if (btnAlarm != null) { all.Add(btnAlarm); btnAlarm.clicked += OnAlarm; }
        if (btnProfile != null) { all.Add(btnProfile); btnProfile.clicked += OnProfile; }
        if (btnNotice != null) { all.Add(btnNotice); btnNotice.clicked += OnNotice; }

        SelectTab("alarm"); // ±âº» ÅÇ
    }

    void OnDisable()
    {
        if (btnAlarm != null) btnAlarm.clicked -= OnAlarm;
        if (btnProfile != null) btnProfile.clicked -= OnProfile;
        if (btnNotice != null) btnNotice.clicked -= OnNotice;
    }

    void OnAlarm() => SelectTab("alarm");
    void OnProfile() => SelectTab("profile");
    void OnNotice() => SelectTab("notice");

    void SelectTab(string key)
    {
        foreach (var b in all) b.RemoveFromClassList("selected");
        if (key == "alarm") btnAlarm?.AddToClassList("selected");
        if (key == "profile") btnProfile?.AddToClassList("selected");
        if (key == "notice") btnNotice?.AddToClassList("selected");

        if (alarmPanel) alarmPanel.SetActive(key == "alarm");
        if (profilePanel) profilePanel.SetActive(key == "profile");
        if (noticePanel) noticePanel.SetActive(key == "notice");
    }
}

