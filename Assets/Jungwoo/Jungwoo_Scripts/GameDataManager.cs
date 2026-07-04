using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    public int currentStage = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("Stage", currentStage);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        currentStage = PlayerPrefs.GetInt("Stage", 1);
    }
}
