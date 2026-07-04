using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UnderMenuBtnController : MonoBehaviour
{
    private LoadingController LC;
    private GameObject LoadingController_obj;

    void Start()
    {
        LoadingController_obj = GameObject.Find("LoadingObject");
        if (LoadingController_obj != null)
        {
            LC = LoadingController_obj.GetComponent<LoadingController>();
        }
    }

    public void HomeBtn()
    {
        if (LC != null) LC.mainRoot.SetActive(true);
        if (LC != null) LC.LoginRoot.SetActive(false);
        if (LC != null) LC.thesisRoot.SetActive(false);
        if (LC != null) LC.favoriteRoot.SetActive(false);
        if (LC != null) LC.AdministratorRoot.SetActive(false);
        if (LC != null) LC.DrawRoot.SetActive(false);
        if (LC != null) LC.SearchRoot.SetActive(false);
        if (LC != null) LC.OptionRoot.SetActive(false);

        Scene scene = SceneManager.GetSceneByName("Main_Simulation_Scene");

        if (!scene.IsValid() || !scene.isLoaded)
        {
            if (LC != null)
                LC.StartCoroutine(LC.LoadSceneAndActivate("Main_Simulation_Scene")); // 코루틴 실행을 LC에서!
        }
        else
        {
            SceneManager.SetActiveScene(scene);
        }
    }

    public void ThesisBtn()
    {
        if(PlayerPrefs.GetInt("Login_State") != 0)
        {
            if (LC != null) LC.mainRoot.SetActive(false);
            if (LC != null) LC.thesisRoot.SetActive(true);
            if (LC != null) LC.favoriteRoot.SetActive(false);
            if (LC != null) LC.AdministratorRoot.SetActive(false);
            if (LC != null) LC.DrawRoot.SetActive(false);
            if (LC != null) LC.SearchRoot.SetActive(false);
            if (LC != null) LC.OptionRoot.SetActive(false);

            Scene scene = SceneManager.GetSceneByName("Thesis_Scene");

            if (!scene.IsValid() || !scene.isLoaded)
            {
                if (LC != null)
                    LC.StartCoroutine(LC.LoadSceneAndActivate("Thesis_Scene")); // 코루틴 실행을 LC에서!
            }
            else
            {
                SceneManager.SetActiveScene(scene);
            }
        }
    }
    public void FavoriteBtn()
    {
        if (LC != null) LC.mainRoot.SetActive(false);
        if (LC != null) LC.thesisRoot.SetActive(false);
        if (LC != null) LC.favoriteRoot.SetActive(true);
        if (LC != null) LC.AdministratorRoot.SetActive(false);
        if (LC != null) LC.DrawRoot.SetActive(false);
        if (LC != null) LC.SearchRoot.SetActive(false);
        if (LC != null) LC.OptionRoot.SetActive(false);

        Scene scene = SceneManager.GetSceneByName("Favorite_Model_Scene");

        if (!scene.IsValid() || !scene.isLoaded)
        {
            if (LC != null)
                LC.StartCoroutine(LC.LoadSceneAndActivate("Favorite_Model_Scene")); // 코루틴 실행을 LC에서!
        }
        else
        {
            SceneManager.SetActiveScene(scene);
        }
    }

    public void DrawBtn()
    {
        if (PlayerPrefs.GetInt("Login_State") != 0)
        {
            if (LC != null) LC.mainRoot.SetActive(false);
            if (LC != null) LC.thesisRoot.SetActive(false);
            if (LC != null) LC.favoriteRoot.SetActive(false);
            if (LC != null) LC.AdministratorRoot.SetActive(false);
            if (LC != null) LC.DrawRoot.SetActive(true);
            if (LC != null) LC.SearchRoot.SetActive(false);
            if (LC != null) LC.OptionRoot.SetActive(false);

            Scene scene = SceneManager.GetSceneByName("Draw_Scene");

            if (!scene.IsValid() || !scene.isLoaded)
            {
                if (LC != null)
                    LC.StartCoroutine(LC.LoadSceneAndActivate("Draw_Scene")); // 코루틴 실행을 LC에서!
            }
            else
            {
                SceneManager.SetActiveScene(scene);
            }
        } 
    }

    public void SearchBtn()
    {
        if (PlayerPrefs.GetInt("Login_State") != 0)
        {
            if (LC != null) LC.mainRoot.SetActive(false);
            if (LC != null) LC.thesisRoot.SetActive(false);
            if (LC != null) LC.favoriteRoot.SetActive(false);
            if (LC != null) LC.AdministratorRoot.SetActive(false);
            if (LC != null) LC.DrawRoot.SetActive(false);
            if (LC != null) LC.SearchRoot.SetActive(true);
            if (LC != null) LC.OptionRoot.SetActive(false);

            Scene scene = SceneManager.GetSceneByName("Search_3D");

            if (!scene.IsValid() || !scene.isLoaded)
            {
                if (LC != null)
                    LC.StartCoroutine(LC.LoadSceneAndActivate("Search_3D")); // 코루틴 실행을 LC에서!
            }
            else
            {
                SceneManager.SetActiveScene(scene);
            }
        }
    }

    public void AdministratorBtn()
    {
        if (LC != null) LC.mainRoot.SetActive(false);
        if (LC != null) LC.thesisRoot.SetActive(false);
        if (LC != null) LC.favoriteRoot.SetActive(false);
        if (LC != null) LC.AdministratorRoot.SetActive(false);
        if (LC != null) LC.DrawRoot.SetActive(false);
        if (LC != null) LC.SearchRoot.SetActive(false);
        if (LC != null) LC.OptionRoot.SetActive(true);

        Scene scene = SceneManager.GetSceneByName("Option_Scene");

        if (!scene.IsValid() || !scene.isLoaded)
        {
            if (LC != null)
                LC.StartCoroutine(LC.LoadSceneAndActivate("Option_Scene")); // 코루틴 실행을 LC에서!
        }
        else
        {
            SceneManager.SetActiveScene(scene);
        }
    }
}
