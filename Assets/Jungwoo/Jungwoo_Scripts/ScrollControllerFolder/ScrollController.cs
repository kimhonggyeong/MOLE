using UnityEngine;

public class ScrollController : MonoBehaviour
{
    public GameObject contentPanel;

    private bool isExpanded = false;
    public void ToggleGroup1()
    {
        isExpanded = !isExpanded;
        contentPanel.SetActive(isExpanded);
    }

    public void NonpolarGroup()
    {
        isExpanded = !isExpanded;
        contentPanel.SetActive(isExpanded);
    }
}
