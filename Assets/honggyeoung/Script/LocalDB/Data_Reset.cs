using SQLite; // 반드시 추가해야 SQLiteConnection 사용 가능
using System.IO;
using UnityEngine;
using static CreateTable;
using static SQLiteExample2;

public class Data_Reset : MonoBehaviour
{
    public void ResetAppData()
    {
        // ✅ 1. PlayerPrefs 초기화
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // ✅ 2. DB 열기
        string dbPath = Path.Combine(Application.persistentDataPath, "myDB.db");
        using (var db = new SQLiteConnection(dbPath))
        {
            db.DeleteAll<user_favorite>();
            db.DeleteAll<quiz_log>();
            db.Execute("DELETE FROM sqlite_sequence;"); // 자동 증가 초기화
        }

        Debug.Log("📦 모든 데이터 초기화 완료");
    }
}
