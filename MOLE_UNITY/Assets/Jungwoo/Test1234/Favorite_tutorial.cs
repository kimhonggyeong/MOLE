using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSequentialTutorial : MonoBehaviour
{
    [Header("�������� ��Ʈ(��ü Ŭ�� ����)")]
    public GameObject overlayPanel;   // Image + Button ����
    public Button overlayButton;

    [Header("�ܰ� �г� (����, ������, ���� ��)")]
    public List<GameObject> stepPanels;

    [Header("ó������ ���̰� ����")]
    public bool showOnlyOnce = true;
    public string prefsKey = "MainFavSceneTutorialDone";

    private int step = 0;

    void Start()
    {
        if (showOnlyOnce && PlayerPrefs.GetInt(prefsKey, 0) == 1)
        {
            overlayPanel.SetActive(false);
            return;
        }

        overlayPanel.SetActive(true);
        step = 0;
        ShowStep();

        if (overlayButton != null)
        {
            overlayButton.onClick.RemoveListener(NextStep);
            overlayButton.onClick.AddListener(NextStep);
        }
    }

    private void ShowStep()
    {
        for (int i = 0; i < stepPanels.Count; i++)
            stepPanels[i].SetActive(i == step);
    }

    public void NextStep()
    {
        step++;
        if (step >= stepPanels.Count)
        {
            overlayPanel.SetActive(false);
            if (showOnlyOnce)
            {
                PlayerPrefs.SetInt(prefsKey, 1);
                PlayerPrefs.Save();
            }
        }
        else
        {
            ShowStep();
        }
    }
}

