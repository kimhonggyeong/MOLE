using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("튜토리얼 오버레이")]
    public GameObject overlayPanel;

    [Header("튜토리얼 설명 단계별 패널 (말풍선, 텍스트 등)")]
    public List<GameObject> stepPanels;

    [Header("행동 유도 step 번호")]
    public List<int> waitForActionSteps; // 예: 7, 8, 9

    private int step = 0;
    private bool waitingForTrigger = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialCheck") == 0)
        {
            step = 0;
            ShowStep();
        }
    }

    void Update()
    {
        if (!overlayPanel.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                overlayPanel.GetComponent<RectTransform>(),
                Input.mousePosition,
                Camera.main))
            {
                if (waitForActionSteps.Contains(step))
                {
                    overlayPanel.SetActive(false);
                    waitingForTrigger = true;
                    Debug.Log($"[튜토리얼] step {step} 에서 유저 행동 대기");
                }
                else
                {
                    step++;

                    // ✅ step이 마지막(=16)인 경우 튜토리얼 종료
                    if (step >= stepPanels.Count)
                    {
                        EndTutorial();
                    }
                    else
                    {
                        ShowStep();
                    }
                }
            }

        }
    }
        


    public void Notify(string trigger)
    {
        if (PlayerPrefs.GetInt("TutorialCheck") == 0)
        {
            Debug.Log($"[튜토리얼] Notify 호출됨: {trigger}, step: {step}");

            if (!waitingForTrigger) return;

            if (step == 7 && trigger == "AminoAcidClicked") // ✅ 여기 수정!
            {
                step++;
                ShowStep();
                waitingForTrigger = false;
            }
            else if (step == 8 && trigger == "NonpolarClicked")
            {
                step++;
                ShowStep();
                waitingForTrigger = false;
            }
            else if (step == 9 && trigger == "AlanineClicked")
            {
                step++;
                ShowStep();
                waitingForTrigger = false;
            }
            else if (step == 15 && trigger == "ModelSlid")
            {
                step++;
                ShowStep();
                waitingForTrigger = false;
            }
        }
    }


    void ShowStep()
    {
        for (int i = 0; i < stepPanels.Count; i++)
            stepPanels[i].SetActive(i == step);

        overlayPanel.SetActive(true);
        waitingForTrigger = false;
        Debug.Log($"[튜토리얼] step {step} 설명 표시됨");
    }

    public void NextFunctionStep()
    {
        step++;
        if (step >= stepPanels.Count)
        {
            EndTutorial();
            return;
        }

        ShowStep();
    }

    public void EndTutorial()
    {
        overlayPanel.SetActive(false);
        foreach (var panel in stepPanels)
            panel.SetActive(false);

        PlayerPrefs.SetInt("TutorialCheck", 1);
        PlayerPrefs.Save();
        Debug.Log("튜토리얼 완료");
    }

    public void SkipTutorial()
    {
        EndTutorial();
    }
    public int CurrentStep => step;

}



