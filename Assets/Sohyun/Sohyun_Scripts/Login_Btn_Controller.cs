using SQLite;
using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 전환용
using static CreateTable;

public class Login_Btn_Controller : MonoBehaviour
{
    private SQLiteConnection db;

    public UnderMenuBtnController umbc;

    [Header("UI Panels")]
    public GameObject serverpanel;

    [Header("Input Fields")]
    public TMP_InputField serverTextInput;
    public TMP_InputField idInput;
    public TMP_InputField pwInput;

    [Header("Error Message")]
    public TextMeshProUGUI errorMessage;   // 빨간 에러 메시지 표시용 Text (TMP)

    private LoadingController LC;  // 씬 로딩 컨트롤러
    private GameObject LoadingController_obj;

    void Start()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "myDB.db");
        db = new SQLiteConnection(dbPath);

        LoadingController_obj = GameObject.Find("LoadingObject");
        if (LoadingController_obj != null)
        {
            LC = LoadingController_obj.GetComponent<LoadingController>();
        }
    }

    // -----------------------------
    // 온라인 로그인 + 검증
    // -----------------------------
    /*public void OnLoginButtonClick()
    {
        // 서버 연결 세팅
        serverpanel.SetActive(true);
        PlayerPrefs.SetInt("Login_State", 1);
        PlayerPrefs.Save();

        if (db.Table<users>().Count() == 0)
        {
            db.Insert(new users
            {
                user_id = "GuestID",
                username = "Guest",
                password = "GuestPW",
                email = "???@example.com",
                birth_date = "1990-01-01",
                department_name = "???과",
                grade = "?학년",
                created_at = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        // 로그인 검증
        string id = idInput.text.Trim();
        string pw = pwInput.text.Trim();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            ShowError("아이디 또는 비밀번호를 입력해주세요.");
            return;
        }

        var user = db.Table<users>().FirstOrDefault(u => u.user_id == id && u.password == pw);
        if (user == null)
        {
            ShowError("아이디 또는 비밀번호가 틀렸습니다. 다시 입력해주세요.");
        }
        else
        {
            ClearError();
            Debug.Log("로그인 성공: " + user.username);

            // -----------------------------
            // 로그인 성공 → 메인 씬 전환
            // -----------------------------
            Scene scene = SceneManager.GetSceneByName("Main_Simulation_Scene");

            if (!scene.IsValid() || !scene.isLoaded)
            {
                if (LC != null)
                    LC.StartCoroutine(LC.LoadSceneAndActivate("Main_Simulation_Scene")); 
            }
            else
            {
                SceneManager.SetActiveScene(scene);
            }
        }
    }*/
    public void InsertOnLineUsers(string UserId, string Username, string Email)
    {
        //serverpanel.SetActive(true);
        PlayerPrefs.SetInt("Login_State", 1);
        PlayerPrefs.Save();
        if (!db.Table<users>().Any(u => u.user_id == UserId))
        {
            db.DeleteAll<users>();
            db.Insert(new users
            {
                user_id = UserId,
                username = Username,
                password = "######",
                email = Email,
                birth_date = "1990-01-01",
                department_name = "소프트웨어학과",
                grade = "1학년",
                created_at = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        umbc.HomeBtn();
    }
    public void InsertOFFLineUsers()
    {
        PlayerPrefs.SetInt("Login_State", 0);
        PlayerPrefs.Save();
        String UserId = "GuestID";
        if (!db.Table<users>().Any(u => u.user_id == UserId))
        {
            db.DeleteAll<users>();
            db.Insert(new users
            {
                user_id = UserId,
                username = "Guest",
                password = "GuestPW",
                email = "???@example.com",
                birth_date = "1990-01-01",
                department_name = "???과",
                grade = "?학년",
                created_at = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        umbc.HomeBtn();
    }

    // -----------------------------
    // 서버 주소 저장 (직접 입력)
    // -----------------------------
    public void InsertOnLineServer()
    {
        PlayerPrefs.SetString("ServerUrl", serverTextInput.text);
        PlayerPrefs.Save();
        serverpanel.SetActive(false);
    }

    public void InsertFakeOnLineServer()
    {
        PlayerPrefs.SetInt("Login_State", 2);
        PlayerPrefs.Save();

        String UserId = "GuestID";
        if (!db.Table<users>().Any(u => u.user_id == UserId))
        {
            db.DeleteAll<users>();
            db.Insert(new users
            {
                user_id = UserId,
                username = "Guest",
                password = "GuestPW",
                email = "???@example.com",
                birth_date = "1990-01-01",
                department_name = "???과",
                grade = "?학년",
                created_at = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        umbc.HomeBtn();

        if(serverTextInput.text == "")
        {
            PlayerPrefs.SetString("ServerUrl", "http://localhost:8000");
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetString("ServerUrl", serverTextInput.text);
            PlayerPrefs.Save();
        }
        //umbc.HomeBtn();
    }

    public void serverUrlPanel()
    {
        serverpanel.SetActive(true);
    }

    // -----------------------------
    // 에러 메시지 제어
    // -----------------------------
    private void ShowError(string message)
    {
        if (errorMessage != null)
        {
            errorMessage.text = message;
            errorMessage.gameObject.SetActive(true);
        }
    }

    private void ClearError()
    {
        if (errorMessage != null)
        {
            errorMessage.text = "";
            errorMessage.gameObject.SetActive(false);
        }
    }
}
