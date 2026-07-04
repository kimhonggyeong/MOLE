using System.IO;
using UnityEngine;
using SQLite;
using static CreateTable;
using TMPro;

public class Progress_Select_Btn : MonoBehaviour
{
    private SQLiteConnection db;
    private UnderMenuBtnController SC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string page;
    public int quizNum;

    public void InsertProgresssSelect()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "myDB.db");
        db = new SQLiteConnection(dbPath);

        GameObject obj = GameObject.Find("SceneChange_Controller");
        if (obj != null)
        {
            SC = obj.GetComponent<UnderMenuBtnController>();
        }
        else
        {
            Debug.LogWarning("오브젝트를 찾을 수 없습니다!");
        }

        /* var exists = db.Table<ProgressSelect>()
             .Where(f => f.selectedText == page && f.QuizNumber == quizNum)
             .FirstOrDefault();

         if (exists == null)
         {
             db.Insert(new ProgressSelect
             {
                 id = 1,
                 selectedText = page,
                 QuizNumber = quizNum,
             });
             SC.HomeBtn();
         }
         else
         {
             Debug.Log("⚠️ 이미 해당 즐겨찾기가 존재합니다.");
         }*/

        PlayerPrefs.SetString("ProgressSelectText", page);
        PlayerPrefs.SetInt("ProgressSelectNumber", quizNum);
        PlayerPrefs.Save();
        SC.HomeBtn();
    }
}
