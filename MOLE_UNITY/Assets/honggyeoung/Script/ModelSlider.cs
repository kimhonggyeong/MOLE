using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModelSlider : MonoBehaviour
{
    public Transform modelGroup;
    public GameObject RTB;
    public GameObject Model2D_Button;

    public float slideDistance = 20f;
    private float slideSpeed = 10f;
    private int totalModels = 2;

    private Vector3 targetPosition;
    private Vector3 dragStartPos;
    private bool isDragging = false;
    private int currentIndex = 0;

    private GameObject SlideIndexCheckObj;
    private Image SlideIndexCheckImage;
    private string SlideIndexCheckName;
    private bool slideTriggeredByUser = false;


    private MainBtnController MBC; // 회전 여부 확인용

    public bool isSliding = false;

    void Start()
    {
        //초기 위치값 지정하고 회전여부 확인을 위한 코드 불러오기
        targetPosition = modelGroup.position;

        GameObject MBC_obj = GameObject.Find("MainBtnController");
        if (MBC_obj != null)
        {
            MBC = MBC_obj.GetComponent<MainBtnController>();
        }
        else
        {
            Debug.LogWarning("RotateToggleButton 오브젝트를 찾을 수 없습니다!");
        }
    }
    void OnEnable()//모델이 숨겨졌다 나올때마다
    {
        // 인덱스 초기화
        currentIndex = 0;

        // 슬라이드 위치 초기화
        targetPosition = new Vector3(0f, 0f, 0f);
        modelGroup.position = targetPosition;

        // 색상 및 버튼 상태 초기화
        if(GameObject.Find("SlideCheckPanel") != null)
        {
            SlideIndexEvent();
        }
    }
    void Update()
    {
        // 회전이 켜져 있으면 슬라이드 금지
        if (MBC != null && (MBC.isRotationOn || MBC.isChatBotPanelOn))
            return;

        HandleInput();

        modelGroup.position = Vector3.Lerp(modelGroup.position, targetPosition, Time.deltaTime * slideSpeed);

        if (Vector3.Distance(modelGroup.position, targetPosition) < 0.01f)
        {
            if (currentIndex == 0)
            {
                RTB.SetActive(true);
                Model2D_Button.SetActive(false);
                MBC.QuizBtn.SetActive(true);
            }
            else if (currentIndex == 1)
            {
                RTB.SetActive(false);
                Model2D_Button.SetActive(true);
            }

            // ✅ 튜토리얼 step 15일 때만 슬라이드 트리거 호출
            if (TutorialManager.Instance != null &&
    TutorialManager.Instance.CurrentStep == 15 &&
    slideTriggeredByUser) // ✅ 진짜 슬라이드했을 때만 작동
            {
                TutorialManager.Instance.Notify("ModelSlid");
                slideTriggeredByUser = false; // ✅ 한 번만 호출되도록 초기화
            }


            modelGroup.position = targetPosition;
            isSliding = false;
        }
        else
        {
            isSliding = true;
        }

        if (isSliding)
        {
            // 확대 축소 임시
            GameObject[] targets = GameObject.FindGameObjectsWithTag("3D_Model");
            foreach (GameObject obj in targets)
            {
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            MBC.QuizBtn.SetActive(false);

        }

    }

    void HandleInput() //드래그 탐지
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0)) //클릭시 드래그 시작 및 시작위치 저장
        {
            isDragging = true;
            dragStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging) //클릭이 끝난후 마지막 위치 저장 두 위치 데이터를 이용해 계산
        {
            isDragging = false;
            Vector3 dragEndPos = Input.mousePosition;
            float dragDelta = dragEndPos.x - dragStartPos.x; //끝 위치 - 시작 위치
            float screenWidth = Screen.width;
            float normalizedDelta = dragDelta / screenWidth;

            if (Mathf.Abs(normalizedDelta) > 0.05f)//드래그 거리가 150이상 날 시
            {
                if (dragDelta > 0) //양수라면 왼쪽 음수라면 오른쪽으로 슬라이드
                    SlideLeft();
                else
                    SlideRight();
            }
        }
#elif UNITY_ANDROID || UNITY_IOS //모바일 터치 부분
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                dragStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended && isDragging)
            {
                isDragging = false;
                Vector3 dragEndPos = touch.position;
                float dragDelta = dragEndPos.x - dragStartPos.x;
                float screenWidth = Screen.width;
                float normalizedDelta = dragDelta / screenWidth;
                if (Mathf.Abs(normalizedDelta) > 0.05f)
                {
                    if (dragDelta > 0)
                        SlideLeft();
                    else
                        SlideRight();
                }
            }
        }
#endif
    }

    //왼쪽 오른쪽에 따라 인덱스를 더하거나 빼고 (인덱스 값*각 오브젝트 거리)를 통해 위치 계산
    void SlideLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            targetPosition = new Vector3(-currentIndex * slideDistance, 0, 0);
            slideTriggeredByUser = true; // ✅ 슬라이드 감지됨!
        }
        SlideIndexEvent();
    }

    void SlideRight()
    {
        if (currentIndex < totalModels - 1)
        {
            currentIndex++;
            targetPosition = new Vector3(-currentIndex * slideDistance, 0, 0);
            slideTriggeredByUser = true; // ✅ 슬라이드 감지됨!
        }
        SlideIndexEvent();
    }


    // 각 번호에 따라 하단 점으로 되어있는 번호 체크 UI에 표시 (각 번호 도달시 파랑색)
    void SlideIndexEvent()
    {
        if (currentIndex == 0)
        {
            //UI_Btns.SetActive(true);
            for(int i = 0; i < totalModels; i++)
            {
                SlideIndexCheckName = "SlideIndexCheck" + i;
                GameObject.Find("SlideIndexCheck" + i).GetComponent<Image>().color = Color.white;
            }
            GameObject.Find("SlideIndexCheck" + currentIndex).GetComponent<Image>().color = new Color(60f / 255f, 120f / 255f, 242f / 255f);
        }
        else if (currentIndex == 1)
        {
            //UI_Btns.SetActive(false);
            for (int i = 0; i < totalModels; i++)
            {
                SlideIndexCheckName = "SlideIndexCheck" + i;
                GameObject.Find("SlideIndexCheck" + i).GetComponent<Image>().color = Color.white;
            }
            GameObject.Find("SlideIndexCheck" + currentIndex).GetComponent<Image>().color = new Color(60f / 255f, 120f / 255f, 242f / 255f);
        }
        /*else if (currentIndex == 2)
        {
            //UI_Btns.SetActive(false);
            for (int i = 0; i < totalModels; i++)
            {
                SlideIndexCheckName = "SlideIndexCheck" + i;
                GameObject.Find("SlideIndexCheck" + i).GetComponent<Image>().color = Color.white;
            }
            GameObject.Find("SlideIndexCheck" + currentIndex).GetComponent<Image>().color = new Color(60f / 255f, 120f / 255f, 242f / 255f);
        }*/
    }
}
