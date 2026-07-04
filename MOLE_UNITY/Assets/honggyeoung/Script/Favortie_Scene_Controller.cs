using SQLite;        
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CreateTable;
using static Quiz_Controller;
using static UnityEngine.Rendering.DebugUI;

public class Favorite_Scene_Controller : MonoBehaviour
{
    public GameObject Favorite_Group_Prefab;
    public Transform Content;
    public GameObject FavoriteCount;
    public GameObject FavoriteScrollRect;
    public GameObject ProgressScrollRect;
    public TextMeshProUGUI Favorite_Count_Text;
    public Sprite Progress_0;
    public Sprite Progress_50;
    public Sprite Progress_100;
    public Sprite Progress_Blue_Btn;
    public Sprite Progress_Red_Btn;

    public ButtonGroupController buttonGroupController;

    public List<GameObject> prefabList = new List<GameObject>();

    private string[] aminoAcids = {
            "Alanine", "Valine", "Leucine", "Isoleucine", "Proline",
            "Phenylalanine", "Tryptophan", "Methionine", "Glycine",
            "Serine", "Threonine", "Tyrosine", "Cysteine",
            "Glutamine", "Asparagine", "Asparticacid", "Glutamicacid",
            "Histidine", "Lysine", "Arginine"};

    //public Color buttonColor = Color.green;

    private SQLiteConnection db;
    //private SQLiteConnection quiz_db;

    private int quiz_count = 0;
    private int quiz_log_count = 0;

    private Dictionary<string, GameObject> progressMap = new();
    private Dictionary<string, GameObject> buttonMap = new();

    private string serverUrl;
    void Start()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "myDB.db");
        db = new SQLiteConnection(dbPath);

        /*string quiz_dbPath = Path.Combine(Application.persistentDataPath, "quiz.db");
        quiz_db = new SQLiteConnection(quiz_dbPath);*/
        //LoadAminoAcidFavorites();
    }
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

        LoadAminoAcidFavorites();
        LoadProgressData();
        SaveOpen();
        buttonGroupController.StartButtonState();
    }
    public void LoadAminoAcidFavorites()
    {

        // ✅ 기존 콘텐츠 초기화
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }

        if (PlayerPrefs.GetInt("Login_State") == 0)
        {
            // ✅ 0. 테이블 존재 여부 확인
            var tableExists = db.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='user_favorite';"
            );

            if (tableExists == 0)
            {
                Debug.LogWarning("user_favorite 테이블이 존재하지 않아 즐겨찾기를 불러올 수 없습니다.");
                return;
            }

            // 1. 아미노산 챕터 page_id 가져오기
            var pages = db.Table<CreateTable.user_favorite>()
                .Select(f => f.chapter_id)
                .ToList();

            int totalPages = pages.Count;
            int groupCount = Mathf.CeilToInt(totalPages / 4f);
            int pageIndex = 0;

            Favorite_Count_Text.text = "20/" + totalPages;

            for (int i = 0; i < groupCount; i++)
            {
                GameObject group = Instantiate(Favorite_Group_Prefab, Content);

                for (int j = 1; j <= 4; j++)
                {
                    if (pageIndex >= totalPages)
                        break;

                    string btnName = $"Favorite_Btn{j}";
                    Transform btnTransform = group.transform.Find(btnName);

                    if (btnTransform != null)
                    {
                        UnityEngine.UI.Button btn = btnTransform.GetComponent<UnityEngine.UI.Button>();
                        if (btn != null)
                        {
                            TextMeshProUGUI tmpText = btn.GetComponentInChildren<TextMeshProUGUI>();
                            if (tmpText != null)
                                tmpText.text = pages[pageIndex];

                            foreach (var item in prefabList)
                            {
                                if (item.name == pages[pageIndex])
                                {
                                    GameObject obj = Instantiate(item, btn.transform);
                                    obj.transform.localPosition = new Vector3(0f, 0f, -20f);
                                    obj.transform.localScale = new Vector3(5f, 5f, 5f);
                                    obj.transform.SetAsLastSibling();
                                }
                            }

                            Image img = btn.GetComponent<Image>();
                            if (img != null)
                                img.color = new Color(1f, 1f, 1f, 1f);

                            Transform child = btn.transform.Find("Image");
                            if (child != null)
                            {
                                Image childImage = child.GetComponent<Image>();
                                childImage.color = new Color(1f, 1f, 1f, 1f);
                            }
                        }
                    }

                    pageIndex++;
                }
            }
        }
        else if (PlayerPrefs.GetInt("Login_State") == 1)
        {
            StartCoroutine(GetFavoriteChaptersFromServer("player01", resultPages =>
            {
                var pages = new List<string>(resultPages); // 복사 권장

                int totalPages = pages.Count;
                int groupCount = Mathf.CeilToInt(totalPages / 4f);
                int pageIndex = 0;

                Favorite_Count_Text.text = "20/" + totalPages;

                for (int i = 0; i < groupCount; i++)
                {
                    GameObject group = Instantiate(Favorite_Group_Prefab, Content);

                    for (int j = 1; j <= 4; j++)
                    {
                        if (pageIndex >= totalPages)
                            break;

                        string btnName = $"Favorite_Btn{j}";
                        Transform btnTransform = group.transform.Find(btnName);

                        if (btnTransform != null)
                        {
                            UnityEngine.UI.Button btn = btnTransform.GetComponent<UnityEngine.UI.Button>();
                            if (btn != null)
                            {
                                TextMeshProUGUI tmpText = btn.GetComponentInChildren<TextMeshProUGUI>();
                                if (tmpText != null)
                                    tmpText.text = pages[pageIndex];

                                foreach (var item in prefabList)
                                {
                                    if (item.name == pages[pageIndex])
                                    {
                                        GameObject obj = Instantiate(item, btn.transform);
                                        obj.transform.localPosition = new Vector3(0f, 0f, -20f);
                                        obj.transform.localScale = new Vector3(5f, 5f, 5f);
                                        obj.transform.SetAsLastSibling();
                                    }
                                }

                                Image img = btn.GetComponent<Image>();
                                if (img != null)
                                    img.color = new Color(1f, 1f, 1f, 1f);

                                Transform child = btn.transform.Find("Image");
                                if (child != null)
                                {
                                    Image childImage = child.GetComponent<Image>();
                                    childImage.color = new Color(1f, 1f, 1f, 1f);
                                }
                            }
                        }

                        pageIndex++;
                    }
                }
            }));
        }

    }

    public void SaveOpen()
    {
        FavoriteScrollRect.SetActive(true);
        FavoriteCount.SetActive(true);
        ProgressScrollRect.SetActive(false);
        QuizView.SetActive(false);
    }
    public void ProgressOpen()
    {
        var User = db.Table<users>()
                  .OrderBy(u => u.created_at)
                  .FirstOrDefault();

        FavoriteScrollRect.SetActive(false);
        FavoriteCount.SetActive(true);
        ProgressScrollRect.SetActive(true);
        QuizView.SetActive(false);
        Favorite_Count_Text.text = db.Table<OverallProgress>()
            .Where(f => f.user_id == User.user_id)
        .Select(f => f.total_progress_percent)
        .FirstOrDefault() + "%";
        if (PlayerPrefs.GetInt("Login_State") == 1)
        {
            StartCoroutine(FetchOverallProgress("player01", overall =>
            {
                Debug.Log($"유저 {overall.user_id}: 푼 문제 {overall.total_solved}, 진행도 {overall.total_progress_percent}%");
                Favorite_Count_Text.text = overall.total_progress_percent + "%";
            }));
        }
        else if (PlayerPrefs.GetInt("Login_State") == 2)
        {
            StartCoroutine(FetchOverallProgress("player01", overall =>
            {
                Debug.Log($"유저 {overall.user_id}: 푼 문제 {overall.total_solved}, 진행도 {overall.total_progress_percent}%");
                Favorite_Count_Text.text = overall.total_progress_percent+"%";
            }));
        }
    }
    public GameObject QuizView;

    public void OpenQuizPanel()
    {
        QuizView.SetActive(true);
        FavoriteScrollRect.SetActive(false);
        FavoriteCount.SetActive(false);
        ProgressScrollRect.SetActive(false);
    }
    public GameObject PopupQuizPanel;  // 인스펙터에 연결

    /*    public void OpenQuizPopup()
        {
            PopupQuizPanel.SetActive(true);
            Time.timeScale = 0; // UI 외부 인터랙션 막기
        }
        public Favorite_Scene_Controller controller;

        public void OnClick()
        {
            controller.OpenQuizPopup();
        }
    */

    /*public void CloseQuizPopup()
    {
        PopupQuizPanel.SetActive(false);
        Time.timeScale = 1;
    }*/
    void CacheProgressObjects()
    {
        foreach (string aa in aminoAcids)
        {
            string progressName = aa + "_Progress";
            string buttonName = aa + "_Progress_Btn";

            GameObject progressObj = GameObject.Find(progressName);
            GameObject btnObj = GameObject.Find(buttonName);

            if (progressObj != null)
            {
                progressMap[aa] = progressObj;
            }
            else
            {
                Debug.LogWarning($"{progressName} 못찾음");
            }

            if (btnObj != null)
            {
                buttonMap[aa] = btnObj;
            }
            else
            {
                Debug.LogWarning($"{buttonName} 못찾음");
            }
        }
    }

    public void LoadProgressData()
    {
        ProgressScrollRect.SetActive(true);
        CacheProgressObjects();
        if (PlayerPrefs.GetInt("Login_State") == 0)
        {
            foreach (string aa in aminoAcids)
            {
                if (progressMap.TryGetValue(aa, out var progressObj) &&
                buttonMap.TryGetValue(aa, out var btnObj))
                {
                    Debug.Log("진도율 오브젝트: " + progressObj.name);
                    Debug.Log("버튼 오브젝트: " + btnObj.name);

                    // 버튼 관련 컴포넌트들
                    Progress_Select_Btn psb = btnObj.GetComponent<Progress_Select_Btn>();
                    if (psb == null) psb = btnObj.AddComponent<Progress_Select_Btn>();

                    TextMeshProUGUI text = btnObj.GetComponentInChildren<TextMeshProUGUI>(true);
                    Image btnImg = btnObj.GetComponentInChildren<Image>(true);

                    // 👉 이후 UI 갱신 코드 작성
                    var MoleQuestions = db.Table<quiz_log>()
                                     .Where(q => q.chapter == aa)
                                     .ToList();

                    int totalQuestionsInChapter = 8;

                    if (MoleQuestions.Count == 0)
                    {
                        btnImg.sprite = Progress_Blue_Btn;
                        text.text = "문제풀러가기";

                        psb.quizNum = 1;
                    }
                    else if (MoleQuestions.Count < totalQuestionsInChapter)
                    {
                        btnImg.sprite = Progress_Blue_Btn;
                        text.text = "이어하기";

                        //var answeredNumbers = MoleQuestions.Select(q => q.question_number).ToList();
                        var answeredNumbers = MoleQuestions
                        .Select(q => {
                            int local = q.quiz_id % totalQuestionsInChapter;
                            return local == 0 ? totalQuestionsInChapter : local;
                        })
                        .ToList();
                        var totalSet = Enumerable.Range(1, totalQuestionsInChapter);
                        var notSolved = totalSet.Except(answeredNumbers);

                        if (notSolved.Any())
                        {
                            int minUnsolved = notSolved.Min();

                            psb.quizNum = minUnsolved;
                            //Debug.Log($"{aa}: 아직 안 푼 가장 낮은 문제 번호는 {minUnsolved}");
                        }

                    }
                    else if (MoleQuestions.Count == totalQuestionsInChapter)
                    {
                        bool hasWrong = MoleQuestions.Any(q => q.status == "wrong");
                        bool allCorrect = MoleQuestions.All(q => q.status == "correct");

                        if (hasWrong)
                        {
                            //progressImg.sprite = Progress_100;
                            btnImg.sprite = Progress_Red_Btn;
                            text.text = "오답 확인하러가기";

                            // ✅ 여기서 가장 낮은 틀린 문제 번호 출력
                            /*var wrongMin = MoleQuestions
                                .Where(q => q.status == "wrong")
                                .Min(q => q.question_number);*/

                            var wrongMin = MoleQuestions
                            .Where(q => q.status == "wrong")
                            .Select(q => {
                                int local = q.quiz_id % totalQuestionsInChapter;
                                return local == 0 ? totalQuestionsInChapter : local;
                            })
                            .Min();

                            psb.quizNum = wrongMin;

                            //Debug.Log($"{aa}: 가장 먼저 틀린 문제 번호 = {wrongMin}");
                        }
                        else if (allCorrect)
                        {
                            btnImg.sprite = Progress_Blue_Btn;
                            text.text = "완료";

                            psb.quizNum = 1;
                        }
                    }
                    Slider slider = progressObj.GetComponentInChildren<Slider>();
                    if (slider != null)
                    {
                        slider.minValue = 0f;
                        slider.maxValue = 1f;
                        slider.value = MoleQuestions.Count / (float)totalQuestionsInChapter;
                    }
                }

                
            }
            ProgressScrollRect.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Login_State") == 1)
        {
            string userId = "player01";
            StartCoroutine(UpdateProgressUI(userId));

            /*StartCoroutine(FetchQuizLogsRequest(userId, logs =>
            {
                foreach (string aa in aminoAcids)
                {
                    string progressName = aa + "_Progress";
                    string buttonName = aa + "_Progress_Btn";
                    Debug.Log("진도율이름:" + progressName);
                    Debug.Log("버튼이름:" + buttonName);
                    GameObject progressObj = GameObject.Find(progressName);
                    Transform btnTr = progressObj.transform.Find(buttonName);
                    GameObject btnObj = btnTr.gameObject;
                    Progress_Select_Btn psb = btnObj.GetComponent<Progress_Select_Btn>();


                    TextMeshProUGUI text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                    Image btnImg = btnObj.GetComponentInChildren<Image>();

                    var MoleQuestions = logs.Where(q => q.chapter == aa).ToList();

                    int totalQuestionsInChapter = 8;

                    if (MoleQuestions.Count == 0)
                    {
                        btnImg.sprite = Progress_Blue_Btn;
                        text.text = "문제풀러가기";

                        psb.quizNum = 1;
                    }
                    else if (MoleQuestions.Count < totalQuestionsInChapter)
                    {
                        btnImg.sprite = Progress_Blue_Btn;
                        text.text = "이어하기";

                        //var answeredNumbers = MoleQuestions.Select(q => q.question_number).ToList();
                        var answeredNumbers = MoleQuestions
                        .Select(q => {
                            int local = q.quiz_id % totalQuestionsInChapter;
                            return local == 0 ? totalQuestionsInChapter : local;
                        })
                        .ToList();
                        var totalSet = Enumerable.Range(1, totalQuestionsInChapter);
                        var notSolved = totalSet.Except(answeredNumbers);

                        if (notSolved.Any())
                        {
                            int minUnsolved = notSolved.Min();

                            psb.quizNum = minUnsolved;
                            //Debug.Log($"{aa}: 아직 안 푼 가장 낮은 문제 번호는 {minUnsolved}");
                        }

                    }
                    else if (MoleQuestions.Count == totalQuestionsInChapter)
                    {
                        bool hasWrong = MoleQuestions.Any(q => q.status == "wrong");
                        bool allCorrect = MoleQuestions.All(q => q.status == "correct");

                        if (hasWrong)
                        {
                            //progressImg.sprite = Progress_100;
                            btnImg.sprite = Progress_Red_Btn;
                            text.text = "오답 확인하러가기";

                            // ✅ 여기서 가장 낮은 틀린 문제 번호 출력
                            *//*var wrongMin = MoleQuestions
                                .Where(q => q.status == "wrong")
                                .Min(q => q.question_number);*//*

                            var wrongMin = MoleQuestions
                            .Where(q => q.status == "wrong")
                            .Select(q => {
                                int local = q.quiz_id % totalQuestionsInChapter;
                                return local == 0 ? totalQuestionsInChapter : local;
                            })
                            .Min();

                            psb.quizNum = wrongMin;

                            //Debug.Log($"{aa}: 가장 먼저 틀린 문제 번호 = {wrongMin}");
                        }
                        else if (allCorrect)
                        {
                            btnImg.sprite = Progress_Blue_Btn;
                            text.text = "완료";

                            psb.quizNum = 1;
                        }
                    }
                    Slider slider = progressObj.GetComponentInChildren<Slider>();
                    if (slider != null)
                    {
                        slider.minValue = 0f;
                        slider.maxValue = 1f;
                        slider.value = MoleQuestions.Count / (float)totalQuestionsInChapter;
                    }
                }
                ProgressScrollRect.SetActive(false);
            }));*/
        }
    }
    
    private IEnumerator FetchQuizLogsRequest(string userId, System.Action<List<GetQuizLog>> onSuccess)
    {
        serverUrl = PlayerPrefs.GetString("ServerUrl");
        string url = $"{serverUrl}/quiz/logs/{UnityWebRequest.EscapeURL(userId)}";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {

            req.certificateHandler = new BypassCertificate(); // 개발용만!
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;
                QuizLogResponse res = JsonUtility.FromJson<QuizLogResponse>(json);
                if (res != null && res.ok)
                {
                    onSuccess?.Invoke(new List<GetQuizLog>(res.logs));
                }
            }
            else
            {
                Debug.LogError("Error fetching logs: " + req.error);
            }
        }
    }
    private IEnumerator FetchOverallProgress(string userId, System.Action<OverallData> onSuccess)
    {
        string serverUrl = PlayerPrefs.GetString("ServerUrl");
        string url = $"{serverUrl}/progress/overall?user_id={UnityWebRequest.EscapeURL(userId)}";

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.certificateHandler = new BypassCertificate(); // 개발용만!

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;
                Debug.Log("Progress JSON: " + json);

                OverallResponse res = JsonUtility.FromJson<OverallResponse>(json);
                if (res != null && res.ok && res.overall != null)
                {
                    onSuccess?.Invoke(res.overall);
                }
                else
                {
                    Debug.LogWarning("응답 파싱 실패");
                }
            }
            else
            {
                Debug.LogError("Progress fetch error: " + req.error);
            }
        }
    }

    public IEnumerator GetFavoriteChaptersFromServer(string userId, System.Action<List<string>> done)
    {
        serverUrl = PlayerPrefs.GetString("ServerUrl");
        string url = $"{serverUrl}/favorites?user_id={UnityWebRequest.EscapeURL(userId)}";
        using (var req = UnityWebRequest.Get(url))
        {
            req.certificateHandler = new BypassCertificate(); // 개발용만
            yield return req.SendWebRequest();

            var result = new List<string>();
            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<FavoriteListRes>(req.downloadHandler.text);
                if (res != null && res.ok && res.items != null)
                {
                    foreach (var it in res.items) result.Add(it.chapter_id);
                    // 중복 제거 + 정렬 원하면:
                    result = result.Distinct().OrderBy(x => x).ToList();
                }
            }
            else
            {
                Debug.LogError("favorites GET 실패: " + req.error);
            }
            done?.Invoke(result);
        }
    }

    IEnumerator UpdateProgressUI(string userId)
    {
        List<GetQuizLog> logs = null;

        // 1) 네트워크 먼저 끝낸다
        yield return StartCoroutine(FetchQuizLogsRequest(userId, l => logs = l));

        // 2) UI가 준비될 때까지 대기 (루트 오브젝트/플래그 기준)
        //yield return new WaitUntil(() => GameObject.Find("ProgressRoot")?.activeInHierarchy == true);
        // 또는 yield return new WaitUntil(() => ProgressUIBuiltAndActive);

        // 3) 이제 안전하게 Find
        foreach (string aa in aminoAcids)
        {
            if (progressMap.TryGetValue(aa, out var progressObj) &&
                buttonMap.TryGetValue(aa, out var btnObj))
            {

                // 버튼 관련 컴포넌트들
                Progress_Select_Btn psb = btnObj.GetComponent<Progress_Select_Btn>();
                if (psb == null) psb = btnObj.AddComponent<Progress_Select_Btn>();

                TextMeshProUGUI text = btnObj.GetComponentInChildren<TextMeshProUGUI>(true);
                Image btnImg = btnObj.GetComponentInChildren<Image>(true);

                var MoleQuestions = logs.Where(q => q.chapter == aa).ToList();
                foreach (var q in MoleQuestions)
                {
                    Debug.Log($"quiz_id={q.quiz_id}, chapter={q.chapter}, status={q.status}");
                }

                int totalQuestionsInChapter = 8;

                if (MoleQuestions.Count == 0)
                {
                    btnImg.sprite = Progress_Blue_Btn;
                    text.text = "문제풀러가기";

                    psb.quizNum = 1;
                }
                else if (MoleQuestions.Count < totalQuestionsInChapter)
                {
                    btnImg.sprite = Progress_Blue_Btn;
                    text.text = "이어하기";

                    //var answeredNumbers = MoleQuestions.Select(q => q.question_number).ToList();
                    var answeredNumbers = MoleQuestions
                    .Select(q => {
                        int local = q.quiz_id % totalQuestionsInChapter;
                        return local == 0 ? totalQuestionsInChapter : local;
                    })
                    .ToList();
                    var totalSet = Enumerable.Range(1, totalQuestionsInChapter);
                    var notSolved = totalSet.Except(answeredNumbers);

                    if (notSolved.Any())
                    {
                        int minUnsolved = notSolved.Min();

                        psb.quizNum = minUnsolved;
                        //Debug.Log($"{aa}: 아직 안 푼 가장 낮은 문제 번호는 {minUnsolved}");
                    }

                }
                else if (MoleQuestions.Count == totalQuestionsInChapter)
                {
                    bool hasWrong = MoleQuestions.Any(q => q.status == "wrong");
                    bool allCorrect = MoleQuestions.All(q => q.status == "correct");

                    if (hasWrong)
                    {
                        //progressImg.sprite = Progress_100;
                        btnImg.sprite = Progress_Red_Btn;
                        text.text = "오답 확인하러가기";

                        // ✅ 여기서 가장 낮은 틀린 문제 번호 출력
                        /*var wrongMin = MoleQuestions
                            .Where(q => q.status == "wrong")
                            .Min(q => q.question_number);*/

                        var wrongMin = MoleQuestions
                        .Where(q => q.status == "wrong")
                        .Select(q => {
                            int local = q.quiz_id % totalQuestionsInChapter;
                            return local == 0 ? totalQuestionsInChapter : local;
                        })
                        .Min();

                        psb.quizNum = wrongMin;

                        //Debug.Log($"{aa}: 가장 먼저 틀린 문제 번호 = {wrongMin}");
                    }
                    else if (allCorrect)
                    {
                        btnImg.sprite = Progress_Blue_Btn;
                        text.text = "완료";

                        psb.quizNum = 1;
                    }
                }
                Slider slider = progressObj.GetComponentInChildren<Slider>();
                if (slider != null)
                {
                    slider.minValue = 0f;
                    slider.maxValue = 1f;
                    slider.value = MoleQuestions.Count / (float)totalQuestionsInChapter;
                }
            }

            
        }
        ProgressScrollRect.SetActive(false);
    }

    public class NamedPrefab
    {
        public string name;       // 프리팹 이름 (키값)
        public GameObject prefab; // 실제 프리팹
    }

    [System.Serializable]
    public class GetQuizLog
    {
        public int Progress_id;
        public string user_id;
        public string chapter;
        public int quiz_id;
        public string status;
        public string answered_at;
    }

    [System.Serializable]
    public class QuizLogResponse
    {
        public bool ok;
        public GetQuizLog[] logs;
    }

    [System.Serializable] 
    public class FavoriteItem 
    { 
        public int favorite_id; 
        public string user_id; 
        public string chapter_id; 
    }

    [System.Serializable] 
    public class FavoriteListRes 
    { 
        public bool ok; 
        public FavoriteItem[] items; 
    }

    public class OverallResponse
    {
        public bool ok;
        public OverallData overall;
    }

    class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // 인증서 무시하고 통과시킴
        }
    }
}
