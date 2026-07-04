using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonGroupController : MonoBehaviour
{
    public List<Button> buttons;

    public Color normalBgColor = new Color32(243, 245, 253, 255);   // #F3F5FD
    public Color selectedBgColor = new Color32(60, 120, 242, 255);  // #3C78F2

    public Color normalTextColor = new Color32(117, 117, 117, 255); // #757575
    public Color selectedTextColor = new Color32(255, 255, 255, 255); // #FFFFFF

    private Button currentSelected;

    void Start()
    {
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => OnButtonClicked(btn));
        }

        // ���� �� �⺻�� (ù ��° ��ư ���� ���·� ����)
        if (buttons.Count > 0)
            OnButtonClicked(buttons[0]);
    }

    void OnButtonClicked(Button clicked)
    {
        // ���� ��ư ���� ��������
        if (currentSelected != null)
            SetButtonState(currentSelected, false);

        // �� ��ư ���� ���·�
        currentSelected = clicked;
        SetButtonState(currentSelected, true);
    }

    void SetButtonState(Button btn, bool isSelected)
    {
        // ���� �ٲٱ�
        Image bg = btn.GetComponent<Image>();
        if (bg != null)
            bg.color = isSelected ? selectedBgColor : normalBgColor;

        // ���ڻ� �ٲٱ�
        TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
        if (txt != null)
            txt.color = isSelected ? selectedTextColor : normalTextColor;
    }

    public void StartButtonState()
    {

        /*foreach (Button btn in buttons)
        {
            Image bg = btn.GetComponent<Image>();
            if (bg != null)
                bg.color = normalBgColor;

            TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.color = normalTextColor;
        }

        Image bg0 = buttons[0].GetComponent<Image>();
        if (bg0 != null)
            bg0.color = selectedBgColor;

        TMP_Text txt0 = buttons[0].GetComponentInChildren<TMP_Text>();
        if (txt0 != null)
            txt0.color = selectedTextColor;*/
        OnButtonClicked(buttons[0]);
        
    }
}

