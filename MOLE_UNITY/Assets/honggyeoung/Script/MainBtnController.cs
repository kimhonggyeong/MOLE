using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SQLite;
using System.IO;
using System.Linq;                 // Where, Select, ToList 등의 LINQ 메서드
using static CreateTable;
using Unity.VisualScripting;

public class MainBtnController : MonoBehaviour
{
    //3D모델 태그와 전체 불러오기
    private string targetTag = "3D_Model";

    public string NowModelName;

    public GameObject Model2DOn;
    public GameObject Model2DOff;

    public GameObject Total_Model_Object; //모든 3D 모델 모음

    /*private GameObject Alanine_Model; // 이름으로 Alanine 오브젝트 찾기
    private GameObject Alanine_Model2; // 이름으로 Alanine 오브젝트 찾기*/
    //UI 오브젝트 불러오기
    public GameObject BtnsPanel;//초기 화면 왼쪽 버튼
    public GameObject amino_acid_Scroll; //아미노산 스크롤
    public GameObject MainCharacter;//중앙 캐릭터
    public GameObject UIPanel;//시뮬레이션에 필요한 UI들 (회전,확대등)
    public GameObject UIButtons;
    public GameObject SlideCheck;
    
    public GameObject ChatBotPanel;//챗봇 패널
    public GameObject ChoiceChatBot;//챗봇 선택
    public GameObject ChatBotTalkScroll;//챗봇 채팅 스크롤
    public GameObject ModelDetail;//챗봇 버튼 옆 상세정보

    public GameObject QuizUI;
    public GameObject QuizBtn;

    public Image ColorPanel;
    public Sprite BlueColorPanel;
    public Sprite YellowColorPanel;

    public GameObject UnderMenuPenel;
    public FavoriteDB_Controller FDB_C; //FavoriteDB_Controller
    public Dictionary<string, GameObject> Models = new Dictionary<string, GameObject>();
    private SQLiteConnection db;

    // === 필드 추가 ===
    public GameObject Model2D_PushButtonObj; // "Push Button (1)" 오브젝트
    public ToggleVisual toggleVisual;

    //public Button Model2D_Button;
    /*public Sprite Model2D_Button_Sprite1;        // 바꿀 이미지 Sprite
    public Sprite Model2D_Button_Sprite2;        // 바꿀 이미지 Sprite*/

    public Quiz_Controller quiz_Controller;
    public QuizTabSpriteSelector quizTabSpriteSelector;

    public GameObject DetailModel;
    public GameObject QuizModel;
    public List<GameObject> prefabList = new List<GameObject>();

    //회전 체크
    public bool isRotationOn = false;

    public bool isChatBotPanelOn = false;

    public bool is2DModelOn = false;

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        //Debug.Log($"[씬 전환] {oldScene.name} → {newScene.name}");
        Armino_Close_Btn();

        var tableExists = db.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='user_favorite';"
        );
        if (tableExists == 0)
        {
            Debug.LogWarning("user_favorite 테이블이 존재하지 않아 즐겨찾기를 불러올 수 없습니다.");
            return;
        }
/*        var FavoriteSelectedText = db.Table<FavoriteSelect>()
        .Where(f => f.id == 1)
        .Select(f => f.selectedText)
        .FirstOrDefault();*/
        var FavoriteSelectedText = PlayerPrefs.GetString("FavoriteSelect", "");
        if (FavoriteSelectedText != "")
        {
            FDB_C.CheckAminoFavorites(FavoriteSelectedText);
            Armino_Open_Btn();
            ShowModelByName(FavoriteSelectedText);
        }
        FavoriteSelectedText = "";
        //db.DeleteAll<FavoriteSelect>();
        PlayerPrefs.SetString("FavoriteSelect", "");
        PlayerPrefs.Save();

        /*var ProgressSelectedText = db.Table<ProgressSelect>()
        .Where(f => f.id == 1)
        .Select(f => f.selectedText)
        .FirstOrDefault();
        var ProgressSelectedNum = db.Table<ProgressSelect>()
        .Where(f => f.id == 1)
        .Select(f => f.QuizNumber)
        .FirstOrDefault();*/
        var ProgressSelectedText = PlayerPrefs.GetString("ProgressSelectText", "");
        var ProgressSelectedNum = PlayerPrefs.GetInt("ProgressSelectNumber", 0);

        if (ProgressSelectedText != "")
        {
            FDB_C.CheckAminoFavorites(ProgressSelectedText);
            Armino_Open_Btn();
            ShowModelByName(ProgressSelectedText);
            ChatBotPanelOpenClose();
            QuizUIOpen();
            quiz_Controller.MultipleChoiceQuizOpen(ProgressSelectedNum);
            quizTabSpriteSelector.Select(ProgressSelectedNum-1);
        }
        ProgressSelectedText = "";
        PlayerPrefs.SetString("ProgressSelectText", "");
        PlayerPrefs.SetInt("ProgressSelectNumber", 0);
        PlayerPrefs.Save();
        /*db.DeleteAll<ProgressSelect>();*/
    }


    void Start()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "myDB.db");
        db = new SQLiteConnection(dbPath);


        Total_Model_Object.SetActive(true);

        // 오브젝트 이름들 리스트
        string[] modelNames = { "Alanine_Object", "Valine_Object", "Leucine_Object", "Isoleucine_Object", "Proline_Object", "Phenylalanine_Object", "Tryptophan_Object", "Methionine_Object", "Glycine_Object" ,
            "Serine_Object" ,"Threonine_Object" ,"Tyrosine_Object" ,"Cysteine_Object" ,"Glutamine_Object" ,"Asparagine_Object" ,
            "Asparticacid_Object","Glutamicacid_Object" ,
            "Histidine_Object" ,"Lysine_Object" ,"Arginine_Object"};

        foreach (string name in modelNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                Models[name] = obj;
            }
        }

        Total_Model_Object.SetActive(false);
        
    }

    public void Armino_Open_Btn() //아미노산 스크롤 열기
    {
        //초기화면 UI는 숨기고 시뮬레이션에 필요한 버튼과 모델을 보이게한다.
        BtnsPanel.SetActive(false);
        MainCharacter.SetActive(false);
        amino_acid_Scroll.SetActive(true);

        //스크롤 열고 닫을 때마다 펼친 부분이 다시 닫히도록 한다.
        string[] panelNames = { "Nonpolar_Btn_Panel", "Polar_Btn_Panel", "Acidic_Btn_Panel", "Basic_Btn_Panel" }; //아미노산 스크롤 버튼 모음 패널 목록

        foreach (string name in panelNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
                obj.SetActive(false);
        }
    }
    public void Armino_Close_Btn() //아미노산 스크롤 닫기
    {
        //시뮬레이션에 필요한 버튼과 모델은 숨기고 초기화면 UI를 보이게한다.
        UIPanel.SetActive(false);
        SlideCheck.SetActive(false);

        Total_Model_Object.SetActive(false);
        QuizBtn.SetActive(false);

        BtnsPanel.SetActive(true);
        MainCharacter.SetActive(true);
        amino_acid_Scroll.SetActive(false);
        // Armino_Close_Btn() 안 어딘가에 ↓ 추가
        //if (Model2D_PushButtonObj) Model2D_PushButtonObj.SetActive(false);

    }

    public void RotationToggleBtn() //회전 체크 함수 (슬라이더와 회전 담당 코드에서 이를 참조)
    {
        GameObject ModelSlider_obj = GameObject.Find("Total_Model_Object");
        if (ModelSlider_obj != null)
        {
            ModelSlider ModelSlider = ModelSlider_obj.GetComponent<ModelSlider>();
            if (ModelSlider.isSliding == true)
            {
                return;
            }
        }
        isRotationOn = !isRotationOn;
        if (isRotationOn)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
            foreach (GameObject obj in targets)
            {
                obj.transform.localScale = Vector3.one;
            }
            amino_acid_Scroll.SetActive(false);
            UIButtons.SetActive(false);
            UnderMenuPenel.SetActive(false);
            QuizBtn.SetActive(false);
            //if (Model2D_PushButtonObj) Model2D_PushButtonObj.SetActive(false);
        }
        else
        {
            amino_acid_Scroll.SetActive(true);
            UIButtons.SetActive(true);
            UnderMenuPenel.SetActive(true);
            QuizBtn.SetActive(true);
            //if (Model2D_PushButtonObj) Model2D_PushButtonObj.SetActive(true);
        }
    }

    public void Model2DToggleBtn()
    {
        GameObject ModelSlider_obj = GameObject.Find("Total_Model_Object");
        if (ModelSlider_obj != null)
        {
            ModelSlider ModelSlider = ModelSlider_obj.GetComponent<ModelSlider>();
            if (ModelSlider.isSliding == true)
            {
                return;
            }
        }
        is2DModelOn = !is2DModelOn;
        if (is2DModelOn == true)
        {
            Model2DOn.SetActive(true);
            Model2DOff.SetActive(false);
            //Model2D_Button.image.sprite = Model2D_Button_Sprite2;
        }
        else
        {
            Model2DOn.SetActive(false);
            Model2DOff.SetActive(true);
            //Model2D_Button.image.sprite = Model2D_Button_Sprite1;
        }
    }

    /*public void Alanine_ModelBtn() // Alanine 모델을 선택시 나오게 하는 함수
    {
        isRotationOn = false; // 회전 false

        Total_Model_Object.SetActive(false);// 슬라이드 초기화를 위해

        // UI 및 모델 전체 모음 오브젝트 보이게하기
        UIPanel.SetActive(true);
        SlideCheck.SetActive(true);
        Total_Model_Object.SetActive(true);

        // 태그가 targetTag인 모든 오브젝트 비활성화 (원래 켜져 있던것들을 꺼야하니)
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in taggedObjects)
        {
            obj.SetActive(false);
        }

        // 특정 오브젝트 활성화
        if (Alanine_Model != null)
        {
            Alanine_Model.transform.localScale = new Vector3(1f, 1f, 1f); //크기는 반드시 1,1,1로 고정
            Alanine_Model.SetActive(true);

        }
    }
    public void Alanine_ModelBtn2() // Alanine2 모델을 선택시 나오게 하는 함수
    {
        isRotationOn = false; // 회전 false

        Total_Model_Object.SetActive(false);

        // UI 및 모델 전체 모음 오브젝트 보이게하기
        UIPanel.SetActive(true);
        SlideCheck.SetActive(true);
        Total_Model_Object.SetActive(true);

        // 태그가 targetTag인 모든 오브젝트 비활성화 (원래 켜져 있던것들을 꺼야하니)
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in taggedObjects)
        {
            obj.SetActive(false);
        }

        // 특정 오브젝트 활성화
        if (Alanine_Model2 != null)
        {
            Alanine_Model2.transform.localScale = new Vector3(1f, 1f, 1f); //크기는 반드시 1,1,1로 고정
            Alanine_Model2.SetActive(true);

        }
    }*/

    public void ShowModelByName(string modelName)
    {
        isRotationOn = false;
        is2DModelOn = false;
        Total_Model_Object.SetActive(false);

        UIPanel.SetActive(true);
        SlideCheck.SetActive(true);
        if (Model2D_PushButtonObj) Model2D_PushButtonObj.SetActive(false);
        toggleVisual.Set(false);
        is2DModelOn = false;
        Total_Model_Object.SetActive(true);
        QuizBtn.SetActive(true);

        //Model2D_Button.image.sprite = Model2D_Button_Sprite1;

        NowModelName = modelName;

        if (Model2DOn || Model2DOff != null)
        {
            Model2DOn.SetActive(true);
            Model2DOff.SetActive(true);
        }

        // 태그로 기존 모델 비활성화
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in taggedObjects)
        {
            obj.SetActive(false);
        }

        // 이름으로 활성화
        if (Models.ContainsKey(modelName + "_Object"))
        {
            Models[modelName + "_Object"].transform.localScale = Vector3.one;
            Models[modelName + "_Object"].SetActive(true);
        }

        Model2DOn = GameObject.Find("ON" + modelName);
        Model2DOff = GameObject.Find("OFF" + modelName);

        Model2DOn.SetActive(false);
    }


    //모델 크기 확대/축소
    public void ScaleUp()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in targets)
        {
            obj.transform.localScale *= 1.1f; // 현재 크기에 * 1.1
        }
    }
    public void ScaleDown()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in targets)
        {
            obj.transform.localScale *= 0.9f;// 현재 크기에 * 0.9
        }
    }

    public void ChatBotPanelOpenClose()
    {
        isChatBotPanelOn = !isChatBotPanelOn;
        if (isChatBotPanelOn == true)
        {
            foreach (var item in prefabList)
            {
                if (item.name == NowModelName)
                {
                    if (DetailModel.transform.childCount > 0)
                    {
                        Destroy(DetailModel.transform.GetChild(0).gameObject);
                    }
                    if (QuizModel.transform.childCount > 0)
                    {
                        Destroy(QuizModel.transform.GetChild(0).gameObject);
                    }
                    GameObject obj1 = Instantiate(item, DetailModel.transform);
                    obj1.transform.localPosition = new Vector3(0f, 0f, -50f);
                    obj1.transform.localScale = new Vector3(10f, 10f, 10f);
                    obj1.transform.SetAsLastSibling();

                    GameObject obj2 = Instantiate(item, QuizModel.transform);
                    obj2.transform.localPosition = new Vector3(0f, 0f, -50f);
                    obj2.transform.localScale = new Vector3(10f, 10f, 10f);
                    obj2.transform.SetAsLastSibling();
                }
            }

            ChoiceChatBot.SetActive(true);
            ChatBotTalkScroll.SetActive(false);
            ModelDetail.SetActive(false);
            QuizUI.SetActive(false);
            ChatBotPanel.SetActive(true);
        }
        else
        {
            ChoiceChatBot.SetActive(true);
            ChatBotTalkScroll.SetActive(false);
            ModelDetail.SetActive(false);
            QuizUI.SetActive(false);
            ChatBotPanel.SetActive(false);
        }
    }
    public void ChatBotPanelBlue()
    {
        ColorPanel.sprite = BlueColorPanel;
    }
    public void ChatBotPanelYellow()
    {
        ColorPanel.sprite = YellowColorPanel;
    }
    public void ChatBotOpen()
    {
        ChoiceChatBot.SetActive(false);
        ChatBotTalkScroll.SetActive(true);
    }
    public void ModelDetailOpen()
    {
        ChoiceChatBot.SetActive(false);
        ModelDetail.SetActive(true);
    }
    public void QuizUIOpen()
    {
        quizTabSpriteSelector.Select(0);
        ChoiceChatBot.SetActive(false);
        QuizUI.SetActive(true);
    }
}


