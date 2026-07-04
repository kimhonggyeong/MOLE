using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabButton : MonoBehaviour
{
    public Button button;
    public Image background;
    public TextMeshProUGUI label;
    public GameObject targetPanel;

    [HideInInspector] public TabUIManager manager;

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            background.color = new Color32(60, 120, 242, 255); 
            label.color = Color.white;
            targetPanel.SetActive(true);
        }
        else
        {
            background.color = new Color32(245, 245, 245, 255); // 연회색
            label.color = Color.black;
            targetPanel.SetActive(false);
        }
    }
}
