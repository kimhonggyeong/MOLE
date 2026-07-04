using SQLite;
using System.IO;
using UnityEngine;

public class Import_Quiz : MonoBehaviour
{
    void Start()
    {
        string fileName = "quiz.db";
        string localPath = Path.Combine(Application.persistentDataPath, fileName);
        string sourcePath = Path.Combine(Application.streamingAssetsPath, fileName);
        // ✅ 조건: 없거나, StreamingAssets의 DB가 더 최신이면 덮어쓰기
        bool shouldCopy = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        WWW reader = new WWW(sourcePath);
        while (!reader.isDone) { }
        byte[] newDbBytes = reader.bytes;

        if (!File.Exists(localPath) || !CompareBytes(File.ReadAllBytes(localPath), newDbBytes))
        {
            File.WriteAllBytes(localPath, newDbBytes);
            Debug.Log("📦 DB 복사 또는 업데이트 완료 (Android)");
        }
#else
        if (!File.Exists(localPath) || File.GetLastWriteTime(sourcePath) > File.GetLastWriteTime(localPath))
        {
            File.Copy(sourcePath, localPath, true);
            Debug.Log("📦 DB 복사 또는 업데이트 완료 (PC/iOS)");
        }
#endif

        var db = new SQLiteConnection(localPath);
        Debug.Log("📂 DB 연결 완료");
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    bool CompareBytes(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }
#endif
}
