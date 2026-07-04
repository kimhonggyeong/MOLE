using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static CreateTable;
using static UnityEngine.Analytics.IAnalytic;

[Serializable]
public class RecommendResponse
{
    public string user_id;
    public int[] recommended_quizzes;
}

public class QuizRecommender : MonoBehaviour
{
    private SQLiteConnection db;
    private SQLiteConnection quiz_db;
    private int QuizAnswerNum;

    public GameObject MultipleChoiceQuiz;
    public GameObject QuizPanel;

    public GameObject AnswerUI;

    public Image AnswerImage;
    public Sprite AnswerImage_O;
    public Sprite AnswerImage_X;

    public TextMeshProUGUI Quiz_Answer_Text;

    public TextMeshProUGUI QuizText1;
    public Button QuizAnswerBtn1_1;
    public Button QuizAnswerBtn1_2;
    public Button QuizAnswerBtn1_3;
    public Button QuizAnswerBtn1_4;

    public Favorite_Scene_Controller favorite_Scene_Controller;
    public QuizTabSpriteSelector quizTabSpriteSelector;


    private int quiz_id_save;

    private string Quiz_chapter;
    private string Quiz_status;

    private int quiz_solved = 0;

    private DateTime startTime;
    private DateTime endTime;

    private string serverBaseUrl;

    RecommendResponse res = null;

    void Start()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "myDB.db");
        db = new SQLiteConnection(dbPath);

        string quiz_dbPath = Path.Combine(Application.persistentDataPath, "quiz.db");
        quiz_db = new SQLiteConnection(quiz_dbPath);
    }
    // 유니티 시작 시 자동 테스트

    public void clickQuizRecommender()
    {
        StartCoroutine(GetRecommendations("GuestID", 3));
    }

    IEnumerator GetRecommendations(string userId, int topN = 3)
    {
        serverBaseUrl = PlayerPrefs.GetString("ServerUrl");
        string url = $"{serverBaseUrl}/recommend?user_id={UnityWebRequest.EscapeURL(userId)}&top_n={topN}";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            bool hasError = req.result != UnityWebRequest.Result.Success;
#else
            bool hasError = req.isHttpError || req.isNetworkError;
#endif
            if (hasError)
            {
                Debug.LogError($"❌ Recommend API Error: {req.responseCode} - {req.error}\n{req.downloadHandler.text}");
                yield break;
            }

            string json = req.downloadHandler.text;
            Debug.Log($"📦 Raw JSON: {json}");
            
            try
            {
                res = JsonUtility.FromJson<RecommendResponse>(json);
                Debug.Log($"✅ user={res.user_id}, recommended=[{string.Join(", ", res.recommended_quizzes)}]");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ JSON Parse Error: {e.Message}\n{json}");
            }
            setQuiz(0);
        }
    }

    public void setQuiz(int data_i)
    {
        AnswerUI.SetActive(false);
        MultipleChoiceQuiz.SetActive(true);

        startTime = DateTime.Now;

        if (quiz_solved == 0)
        {
            quiz_solved = quiz_db.Table<quiz>().Count();
        }

        //favorite_Scene_Controller.OpenQuizPopup();

        int quizId = res.recommended_quizzes[data_i];
        var quiz_data = quiz_db.Find<quiz>(quizId);

        QuizText1.text = $"Q : {quiz_data.question}";

        quiz_id_save = quiz_data.quiz_id;

        Quiz_chapter = quiz_data.amino_acid;

        List<string> options = ParseOptions(quiz_data.options);

        QuizAnswerBtn1_1.GetComponentInChildren<TextMeshProUGUI>().text = "A : " + options[0];
        QuizAnswerBtn1_2.GetComponentInChildren<TextMeshProUGUI>().text = "B : " + options[1];
        QuizAnswerBtn1_3.GetComponentInChildren<TextMeshProUGUI>().text = "C : " + options[2];
        QuizAnswerBtn1_4.GetComponentInChildren<TextMeshProUGUI>().text = "D : " + options[3];

        QuizAnswerNum = GetAnswerIndex(options, quiz_data.answer);
    }

    public void AnswerChoice(int n)
    {
        MultipleChoiceQuiz.SetActive(false);
        AnswerUI.SetActive(true);

        if (QuizAnswerNum == n)
        {
            AnswerImage.sprite = AnswerImage_O;
            Quiz_Answer_Text.text = "정답입니다!";
            Quiz_status = "correct";
        }
        else
        {
            AnswerImage.sprite = AnswerImage_X;
            Quiz_Answer_Text.text = "오답입니다.";
            Quiz_status = "wrong";
        }
        endTime = DateTime.Now;
        SaveQuizStatus();
    }

    public void OpenQuizPopup()
    {
        quizTabSpriteSelector.Select(0);
        QuizPanel.SetActive(true);
    }

    public void CloseQuizPopup()
    {
        favorite_Scene_Controller.LoadProgressData();
        QuizPanel.SetActive(false);
        MultipleChoiceQuiz.SetActive(true);
        AnswerUI.SetActive(false);
    }

    public void SaveQuizStatus()
    {
        var existing = db.Table<quiz_log>().Where(q => q.chapter == Quiz_chapter && q.quiz_id == quiz_id_save).FirstOrDefault();
        var User = db.Table<users>()
                  .OrderBy(u => u.created_at)
                  .FirstOrDefault();

        if (existing != null)
        {
            existing.status = Quiz_status;
            existing.answered_at = (endTime - startTime).TotalSeconds;
            db.Update(existing);
        }
        else
        {
            db.Insert(new quiz_log
            {
                user_id = User.user_id,
                quiz_id = quiz_id_save,
                chapter = Quiz_chapter,
                //question_number = Quiz_questionNumber,
                status = Quiz_status,
                answered_at = (endTime - startTime).TotalSeconds
            });
            db.Delete<OverallProgress>(User.user_id);
            db.Insert(new OverallProgress
            {
                user_id = User.user_id,
                total_solved = quiz_solved,
                total_progress_percent = ((double)db.Table<quiz_log>().Count() / quiz_solved * 100)
            });
        }
    }

    List<string> ParseOptions(string raw)
    {
        string cleaned = raw.Replace("[", "").Replace("]", "").Replace("'", "").Replace("\"", "");
        return new List<string>(cleaned.Split(','));
    }

    int GetAnswerIndex(List<string> options, string answer)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].Trim() == answer.Trim())
                return i + 1;  // 버튼 번호는 1부터 시작
        }
        return -1;
    }
}
