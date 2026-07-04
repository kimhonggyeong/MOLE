using UnityEngine;
using UnityEngine.UI;  // 또는 TextMeshProUGUI 사용 시 using TMPro;
using SQLite;          // SQLite.cs 포함
using System.IO;
using System.Collections.Generic;
using TMPro;
using System;

public class SQLiteExample2 : MonoBehaviour
{
    private SQLiteConnection db;
    //public TextMeshProUGUI uiText;

    void Start()
    {
        //string NameDB = "hkData"+".db";
        string dbPath = Path.Combine(Application.persistentDataPath, "myDB.db");
        db = new SQLiteConnection(dbPath);

        db.Execute("PRAGMA foreign_keys = ON;");

        db.CreateTable<User>();
        db.CreateTable<UserFavorite>();
        //ShowFavorites();
    }

    // ✅ 버튼에 연결할 함수
    public void SQLiteDataAdd(string PageName)
    {
        InsertSampleUsers();
        InsertAminoFavorites(PageName);
        //ShowFavorites(); // UI 출력
    }

    void InsertSampleUsers()
    {
        if (db.Table<User>().Count() == 0)
        {
            db.Insert(new User
            {
                user_id = "user1",
                username = "김홍경",
                password = "pass123",
                email = "hong@example.com",
                phone_number = "01012345678",
                birth_date = "1990-01-01",
                school_name = "서울고",
                department_name = "컴퓨터공학과",
                auto_login = false,
                created_at = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
    }

    void InsertAminoFavorites(string PageName)
    {
        string userId = "user1";      // 🔁 외부 입력값 사용
        string chapter = "Amino";
        string page = PageName;

        var exists = db.Table<UserFavorite>()
            .Where(f => f.user_id == userId && f.chapter_num == chapter && f.page_id == page)
            .FirstOrDefault();

        if (exists == null)
        {
            db.Insert(new UserFavorite
            {
                user_id = userId,
                chapter_num = chapter,
                page_id = page,
                created_at = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        else
        {
            Debug.Log("⚠️ 이미 해당 즐겨찾기가 존재합니다.");
            DeleteFavorite(userId,chapter,page);
        }
    }

    void ShowFavorites()
    {
        List<UserFavorite> list = db.Table<UserFavorite>().ToList();
        string result = "즐겨찾기 목록\n";

        foreach (var fav in list)
        {
            result += $"{fav.user_id} | 챕터: {fav.chapter_num} | 페이지: {fav.page_id} | {fav.created_at}\n";
        }

        //uiText.text = result;
    }

    void DeleteFavorite(string userId, string chapter, string page)
    {
        var favorite = db.Table<UserFavorite>()
            .Where(f => f.user_id == userId && f.chapter_num == chapter && f.page_id == page)
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

        ShowFavorites(); // UI 갱신
    }

    // 테이블 클래스는 동일하므로 그대로 유지
    public class User
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
}
