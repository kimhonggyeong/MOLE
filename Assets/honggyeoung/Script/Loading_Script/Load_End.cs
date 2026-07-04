using UnityEngine;
using UnityEngine.SceneManagement;

public class Load_End : MonoBehaviour
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
    public void OnLoadingAnimComplete()
    {
        //Debug.Log("애니메이션 끝남! 씬 전환 가능");
        LC.ActivateScene("Login_Scene");
        LC.ActivateScene("Main_Simulation_Scene");
        LC.ActivateScene("Thesis_Scene");
        LC.ActivateScene("Favorite_Model_Scene");
        LC.ActivateScene("Administrator_Scene");
        LC.ActivateScene("Draw_Scene");
        LC.ActivateScene("Search_3D");
        LC.ActivateScene("Option_Scene");

        LC.LoginRoot = GameObject.Find("Login_Root");
        LC.mainRoot = GameObject.Find("Main_Simulation_Root");
        LC.thesisRoot = GameObject.Find("Thesis_Root");
        LC.favoriteRoot = GameObject.Find("Favorite_root");
        LC.AdministratorRoot = GameObject.Find("Administrator_root");
        LC.DrawRoot = GameObject.Find("Draw_Root");
        LC.SearchRoot = GameObject.Find("Search_Root");
        LC.OptionRoot = GameObject.Find("Option_Root");

        if (LC != null) LC.LoginRoot.SetActive(true);
        if (LC != null) LC.mainRoot.SetActive(false);
        if (LC != null) LC.thesisRoot.SetActive(false);
        if (LC != null) LC.favoriteRoot.SetActive(false);
        if (LC != null) LC.AdministratorRoot.SetActive(false);
        if (LC != null) LC.DrawRoot.SetActive(false);
        if (LC != null) LC.SearchRoot.SetActive(false);
        if (LC != null) LC.OptionRoot.SetActive(false);

        Scene scene = SceneManager.GetSceneByName("Login_Scene");

        if (!scene.IsValid() || !scene.isLoaded)
        {
            if (LC != null)
                LC.StartCoroutine(LC.LoadSceneAndActivate("Login_Scene")); // 코루틴 실행을 LC에서!
        }
        else
        {
            SceneManager.SetActiveScene(scene);
            SceneManager.UnloadSceneAsync("Loading_End_Scene");
        }
    }
}
