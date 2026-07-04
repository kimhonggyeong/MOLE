using UnityEngine;
using UnityEngine.UI;  // 또는 TextMeshProUGUI 사용 시 using TMPro;
using SQLite;          // SQLite.cs 포함
using System.IO;
using System.Collections.Generic;
using TMPro;
using System;
using static MainBtnController;

public class CreateTable : MonoBehaviour
{
    private SQLiteConnection db;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "myDB.db");
        db = new SQLiteConnection(dbPath);

        db.Execute("PRAGMA foreign_keys = ON;");

        db.CreateTable<users>();
        db.CreateTable<user_favorite>();
        db.CreateTable<quiz_log>();
        db.CreateTable<OverallProgress>();
    }

    public class users
    {
        [PrimaryKey]
        public string user_id { get; set; }

        [Indexed]
        public string username { get; set; }

        public string password { get; set; }

        [Indexed]
        public string email { get; set; }

        public string birth_date { get; set; }  // SQLite.NET에서 DATE는 string으로 다룸

        public string department_name { get; set; }

        public string grade { get; set; }

        public string created_at { get; set; }  // TIMESTAMP도 string으로 받음
    }
    public class quiz
    {
        [PrimaryKey, AutoIncrement]
        public int quiz_id { get; set; }

        public string amino_acid { get; set; }
        public string topic { get; set; }
        public string question { get; set; }
        public string options { get; set; }  // JSON처럼 저장
        public string answer { get; set; }
        public string grade { get; set; }
    }
    public class user_favorite
    {
        [PrimaryKey, AutoIncrement]
        public int Favorite_id { get; set; }

        public string user_id { get; set; }
        public string chapter_id { get; set; }
    }

    public class quiz_log
    {
        [PrimaryKey, AutoIncrement]
        public int Progress_id { get; set; }

        public string user_id { get; set; }

        public string chapter { get; set; }
        /*public int question_number { get; set; }*/
        public int quiz_id { get; set; }

        public string status { get; set; } // "unanswered", "correct", "wrong"
        public double answered_at { get; set; } // 🕒 추가
    }
    public class OverallProgress
    {
        [PrimaryKey]
        public string user_id { get; set; }

        public int total_solved { get; set; } = 0;

        public double total_progress_percent { get; set; } = 0.0;
    }
}
