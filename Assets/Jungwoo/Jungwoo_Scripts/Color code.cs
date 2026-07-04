using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SortButtonGroup : MonoBehaviour
{
    public List<Button> buttons;
    public Color selectedColor = new Color(0.0f, 0.48f, 1.0f); // 파란색
    public Color defaultColor = new Color(0.6f, 0.6f, 0.6f);   // 회색

    void Start()
    {
        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(() => OnButtonClicked(btn));
        }


    }

    public void OnButtonClicked(Button clicked)
    {
        foreach (var btn in buttons)
        {
            var text = btn.GetComponentInChildren<TMP_Text>();
            if (btn == clicked)
                text.color = selectedColor;
            else
                text.color = defaultColor;
        }
    }
}
