using System.IO;
using UnityEngine;
using SQLite;
using static CreateTable;
using TMPro;

public class Favorite_Select_Btn : MonoBehaviour
{
    private SQLiteConnection db;
    private UnderMenuBtnController SC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void InsertFavoriteSelect()
    {
        TextMeshProUGUI tmp = GetComponentInChildren<TextMeshProUGUI>();
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

        string page = tmp.text;

        /*        var exists = db.Table<FavoriteSelect>()
                    .Where(f => f.selectedText == page)
                    .FirstOrDefault();


                if (exists == null)
                {
                    db.Insert(new FavoriteSelect
                    {
                        id = 1,
                        selectedText = page,
                    });
                    SC.HomeBtn();
                }
                else
                {
                    Debug.Log("⚠️ 이미 해당 즐겨찾기가 존재합니다.");
                }*/
        if(page != "")
        {
            PlayerPrefs.SetString("FavoriteSelect", page);
            PlayerPrefs.Save();
            SC.HomeBtn();
        }
    }
}
