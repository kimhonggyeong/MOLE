using SQLite;          // SQLite.cs 포함
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;  // 또는 TextMeshProUGUI 사용 시 using TMPro;
using static CreateTable;

public class FavoriteDB_Controller : MonoBehaviour
{
    private SQLiteConnection db;
    public Button FavoriteButton;         // Inspector에서 연결
    public Sprite Sprite1;        // 바꿀 이미지 Sprite
    public Sprite Sprite2;        // 바꿀 이미지 Sprite
    public string NowChapterName;

    private string serverUrl;

    void Start()
    {
        //string NameDB = "hkData"+".db";
        string dbPath = Path.Combine(Application.persistentDataPath, "myDB.db");
        db = new SQLiteConnection(dbPath);

        //db.Execute("PRAGMA foreign_keys = ON;");

        //db.CreateTable<User>();
        //db.CreateTable<UserFavorite>();
        //db.CreateTable<FavoriteSelect>();
        //ShowFavorites();
    }

    // ✅ 버튼에 연결할 함수
    public void SQLiteDataAdd()
    {
        //InsertSampleUsers();
        InsertAminoFavorites();
        if (PlayerPrefs.GetInt("Login_State") == 1)
        {
            var User = db.Table<users>()
                  .OrderBy(u => u.created_at)
                  .FirstOrDefault();

            string userId = User.user_id;

            StartCoroutine(CheckFavoriteClientSide(userId, NowChapterName, exists =>
            {
                if(!exists)
                {
                    StartCoroutine(AddFavoriteRequest(userId, NowChapterName));
                }
                else
                {
                    StartCoroutine(DeleteFavoritePost(userId, NowChapterName));
                }
            }));
        }
        //ShowFavorites(); // UI 출력
    }

/*    void InsertSampleUsers()
    {
        if (db.Table<users>().Count() == 0)
        {
            db.Insert(new users
            {
                user_id = "localhost",
                username = "호스트",
                password = "localpass",
                email = "???@example.com",
                birth_date = "1990-01-01",
                department_name = "???과",
                grade = "?학년",
                created_at = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
    }*/

    void InsertAminoFavorites()
    {
        var User = db.Table<users>()
                  .OrderBy(u => u.created_at)
                  .FirstOrDefault();

        string userId = User.user_id;      // 🔁 외부 입력값 사용
        string chapter = NowChapterName;

        var exists = db.Table<user_favorite>()
            .Where(f => f.user_id == userId && f.chapter_id == chapter)
            .FirstOrDefault();

        if (exists == null)
        {
            db.Insert(new user_favorite
            {
                user_id = userId,
                chapter_id = chapter
            });
            FavoriteButton.image.sprite = Sprite2;
        }
        else
        {
            Debug.Log("⚠️ 이미 해당 즐겨찾기가 존재합니다.");
            DeleteFavorite(userId, chapter);
            FavoriteButton.image.sprite = Sprite1;
        }
    }

    public void CheckAminoFavorites(string ChapterName)
    {
        var User = db.Table<users>()
                  .OrderBy(u => u.created_at)
                  .FirstOrDefault();

        NowChapterName = ChapterName;
        string userId = User.user_id;      // 🔁 외부 입력값 사용
        string chapter = ChapterName;

        var exists = db.Table<user_favorite>()
            .Where(f => f.user_id == userId && f.chapter_id == chapter)
            .FirstOrDefault();

        if (exists == null)
        {
            FavoriteButton.image.sprite = Sprite1;
        }
        else
        {
            FavoriteButton.image.sprite = Sprite2;
        }
    }

/*    void ShowFavorites()
    {
        List<UserFavorite> list = db.Table<UserFavorite>().ToList();
        string result = "즐겨찾기 목록\n";

        foreach (var fav in list)
        {
            result += $"{fav.user_id} | 챕터: {fav.chapter_num} | 페이지: {fav.page_id} | {fav.created_at}\n";
        }

        //uiText.text = result;
    }*/

    void DeleteFavorite(string userId, string chapter)
    {
        var favorite = db.Table<user_favorite>()
            .Where(f => f.user_id == userId && f.chapter_id == chapter)
            .FirstOrDefault();

        if (favorite != null)
        {
            db.Delete(favorite);
            Debug.Log("✅ 즐겨찾기 삭제 완료");
        }
        else
        {
            Debug.LogWarning("⚠️ 삭제할 즐겨찾기 항목이 없습니다.");
        }

        //ShowFavorites(); // UI 갱신
    }

    private IEnumerator AddFavoriteRequest(string userId, string chapterId)
    {
        serverUrl = PlayerPrefs.GetString("ServerUrl");
        string url = $"{serverUrl}/favorites/add";

        // JSON 바디 만들기
        string jsonBody = JsonUtility.ToJson(new FavoriteData
        {
            user_id = userId,
            chapter_id = chapterId
        });

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            // 인증서 무시용 (개발 환경일 때만!)
            req.certificateHandler = new BypassCertificate();

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("서버 응답: " + req.downloadHandler.text);
                // 응답 JSON 파싱
                FavoriteResponse res = JsonUtility.FromJson<FavoriteResponse>(req.downloadHandler.text);
                if (res.ok)
                {
                    Debug.Log("즐겨찾기 추가 성공");
                    FavoriteButton.image.sprite = Sprite2;
                }
                else
                {
                    Debug.LogWarning("추가 실패: " + res.error);
                }
            }
            else
            {
                Debug.LogError("요청 실패: " + req.error);
            }
        }
    }

    private IEnumerator DeleteFavoritePost(string userId, string chapterId)
    {
        // /favorites/remove 엔드포인트로 고정
        serverUrl = PlayerPrefs.GetString("ServerUrl");
        string url = (serverUrl.EndsWith("/") ? serverUrl : serverUrl + "/") + "favorites/remove";

        var bodyObj = new FavoriteData { user_id = userId, chapter_id = chapterId };
        string json = JsonUtility.ToJson(bodyObj);
        byte[] raw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(raw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            // 개발용 HTTPS 우회 (운영에선 제거!)
            req.certificateHandler = new BypassCertificate();

            yield return req.SendWebRequest();

            string text = req.downloadHandler.text;
            FavoriteResponse parsed = null;
            try { parsed = JsonUtility.FromJson<FavoriteResponse>(text); } catch { /* ignore */ }

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("즐겨찾기 삭제 성공");
                FavoriteButton.image.sprite = Sprite1;
            }
            else
            {
                // 서버가 {"ok":false,"error":"..."}를 준 경우 그 메시지 우선
                if (parsed != null && !parsed.ok && !string.IsNullOrEmpty(parsed.error))
                    Debug.Log(parsed.error);
                else
                    Debug.Log(req.error);
            }
        }
    }
    IEnumerator CheckFavoriteClientSide(string userId, string chapterId, System.Action<bool> done)
    {
        serverUrl = PlayerPrefs.GetString("ServerUrl");
        string url = $"{serverUrl}/favorites?user_id={UnityWebRequest.EscapeURL(userId)}";
        using (var req = UnityWebRequest.Get(url))
        {
            req.certificateHandler = new BypassCertificate();
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<FavoriteListRes>(req.downloadHandler.text);
                bool exists = res.ok && System.Array.Exists(res.items, it => it.chapter_id == chapterId);
                done?.Invoke(exists);
            }
            else { done?.Invoke(false); }
        }
    }
    // 테이블 클래스는 동일하므로 그대로 유지
    /*    public class User
        {
            [PrimaryKey]
            public string user_id { get; set; }

            [Indexed]
            public string username { get; set; }
            public string password { get; set; }
            public string email { get; set; }
            public string phone_number { get; set; }
            public string birth_date { get; set; }
            public string school_name { get; set; }
            public string department_name { get; set; }
            public bool auto_login { get; set; }
            public string created_at { get; set; }
        }

        public class UserFavorite
        {
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }

            public string user_id { get; set; }
            public string chapter_num { get; set; }
            public string page_id { get; set; }
            public string created_at { get; set; }
        }

        public class FavoriteSelect
        {
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }
            public string selectedText { get; set; }
        }*/
    [System.Serializable]
    public class FavoriteData
    {
        public string user_id;
        public string chapter_id;
    }

    [System.Serializable]
    public class FavoriteResponse
    {
        public bool ok;
        public string error;
    }
    [System.Serializable] 
    class FavoriteItem 
    { 
        public int favorite_id; 
        public string user_id; 
        public string chapter_id; 
    }

    [System.Serializable] 
    class FavoriteListRes 
    { 
        public bool ok; 
        public FavoriteItem[] items; 
    }

    // 개발용: HTTPS 인증서 무시
    class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
}
