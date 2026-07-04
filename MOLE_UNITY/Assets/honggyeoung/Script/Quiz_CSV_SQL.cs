using SQLite;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static CreateTable;

public class Quiz_CSV_SQL : MonoBehaviour
{
    private SQLiteConnection db;

    void Start()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "quiz.db");
        db = new SQLiteConnection(dbPath);
        db.CreateTable<quiz>();

        ImportCSV("quiz_250729");
    }

    void ImportCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            if (fields.Length < 6)
            {
                Debug.LogWarning($"줄 무시됨 (필드 부족): {lines[i]}");
                continue;
            }

            // 앞 3개 필드: amino_acid, topic, question
            string amino_acid = fields[0];
            string topic = fields[1];
            string question = fields[2];

            // 마지막 2개 필드: answer, grade
            string answer = fields[fields.Length - 2];
            string grade = fields[fields.Length - 1].Trim();

            // 중간의 옵션 필드들을 다시 합치기
            string[] optionFields = new string[fields.Length - 5]; // 3 앞 + 2 뒤 제외
            System.Array.Copy(fields, 3, optionFields, 0, optionFields.Length);
            string options = string.Join(",", optionFields);

            quiz quiz = new quiz
            {
                amino_acid = amino_acid,
                topic = topic,
                question = question,
                options = options,
                answer = answer,
                grade = grade
            };

            db.Insert(quiz);
        }

        Debug.Log("CSV → SQLite 삽입 완료");
    }

}
