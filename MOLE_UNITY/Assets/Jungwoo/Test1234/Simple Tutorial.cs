using UnityEngine;

public class SimpleTutorial : MonoBehaviour
{
    public GameObject tutorialPanel; // overlaypanel 2 ����
    private string tutorialKey = "PaperSearchTutorialDone"; // ���� Ű

    void Start()
    {
        // ����� �� Ȯ�� (0: �Ⱥ���, 1: ����)
        if (PlayerPrefs.GetInt(tutorialKey, 0) == 0)
        {
            tutorialPanel.SetActive(true);  // ó���̸� �ѱ�
        }
        else
        {
            tutorialPanel.SetActive(false); // �̹� ������ �� �ѱ�
        }
    }

    public void ClosePanel()
    {
        tutorialPanel.SetActive(false);
        PlayerPrefs.SetInt(tutorialKey, 1); // �� �ɷ� ����
        PlayerPrefs.Save();
    }
}

