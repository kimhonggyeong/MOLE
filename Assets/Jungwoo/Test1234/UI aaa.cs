using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LeftMenuController : MonoBehaviour
{
    UIDocument ui;
    List<Button> buttons;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;
        buttons = root.Query<Button>(className: "menu-btn").ToList();

        foreach (var b in buttons)
            b.clicked += () => Select(b);

        if (buttons.Count > 0) Select(buttons[0]);
    }

    void Select(Button b)
    {
        foreach (var x in buttons) x.RemoveFromClassList("selected");
        b.AddToClassList("selected");
        // TODO: 여기서 패널 전환 등 동작 추가
    }
}

