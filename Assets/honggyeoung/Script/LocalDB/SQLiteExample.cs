using UnityEngine;
using SQLite; // SQLite.cs에 포함된 네임스페이스
using System.IO;

public class SQLiteExample : MonoBehaviour
{
    private SQLiteConnection db;

    void Start()
    {
        // 📌 DB 경로 설정
        string dbPath = Path.Combine(Application.persistentDataPath, "mydata.db");
        db = new SQLiteConnection(dbPath);

        // 📌 테이블 생성
        db.CreateTable<User>();

        // 📌 데이터 삽입
        db.Insert(new User { Name = "홍길동", Age = 23 });

        // 📌 데이터 조회
        var users = db.Table<User>().ToList();
        foreach (var user in users)
        {
            Debug.Log($"🧑‍💼 이름: {user.Name}, 나이: {user.Age}");
        }
    }

    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }
    }
}
