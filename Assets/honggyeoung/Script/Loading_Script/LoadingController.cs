using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    public GameObject LoginRoot;
    public GameObject thesisRoot;
    public GameObject mainRoot;
    public GameObject favoriteRoot;
    public GameObject AdministratorRoot;
    public GameObject DrawRoot;
    public GameObject SearchRoot;
    public GameObject OptionRoot;
    private string[] scenesToLoad = { "Loading_End_Scene", "Login_Scene", "Main_Simulation_Scene", "Thesis_Scene", "Favorite_Model_Scene", "Draw_Scene", "Search_3D", "Option_Scene", "Administrator_Scene" };

    // 각 씬의 루트 오브젝트를 저장해둘 딕셔너리
    private Dictionary<string, GameObject[]> sceneRootObjects = new Dictionary<string, GameObject[]>();

    void Start()
    {
        int refreshRate = Screen.currentResolution.refreshRate;

        Application.targetFrameRate = refreshRate;
        QualitySettings.vSyncCount = 0;

        PlayerPrefs.SetInt("Login_State", -1);
        PlayerPrefs.Save();

        StartCoroutine(LoadScenes());
    }

    IEnumerator LoadScenes()
    {
        foreach (string sceneName in scenesToLoad)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
                yield return null;

            op.allowSceneActivation = true;
            yield return new WaitUntil(() => op.isDone);

            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            GameObject[] rootObjects = loadedScene.GetRootGameObjects();

            // 루트 오브젝트 저장 및 비활성화
            sceneRootObjects[sceneName] = rootObjects;
            foreach (GameObject obj in rootObjects)
            {
                obj.SetActive(false);
            }
        }

        /*// Thesis/Simulation 루트 직접 찾아두기
        thesisRoot = GameObject.Find("Thesis_Root");
        mainRoot = GameObject.Find("Main_Simulation_Root");*/

        // 비활성화 (혹시라도 Find 된 경우)
        /*if (thesisRoot != null) thesisRoot.SetActive(false);
        if (mainRoot != null) mainRoot.SetActive(false);*/

        // 로딩 끝 → Loading_End_Scene 활성화
        ActivateScene("Loading_End_Scene");

        // 로딩씬 제거
        SceneManager.UnloadSceneAsync("LoadingScene");
    }

    // ▶ 특정 씬의 루트 오브젝트 전체 활성화 및 ActiveScene 설정
    public void ActivateScene(string sceneName)
    {
        if (sceneRootObjects.TryGetValue(sceneName, out GameObject[] roots))
        {
            foreach (GameObject obj in roots)
            {
                obj.SetActive(true);
            }
        }

        Scene targetScene = SceneManager.GetSceneByName(sceneName);
        if (targetScene.IsValid() && targetScene.isLoaded)
        {
            SceneManager.SetActiveScene(targetScene);
        }
        else
        {
            Debug.LogError($"[ERROR] SetActiveScene 실패: {sceneName}");
        }
    }

    // ▶ 외부에서 특정 씬 동적으로 불러오고 활성화할 때
    public IEnumerator LoadSceneAndActivate(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => asyncLoad.isDone);

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        GameObject[] roots = loadedScene.GetRootGameObjects();
        sceneRootObjects[sceneName] = roots;

        ActivateScene(sceneName);
    }
}
