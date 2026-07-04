using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource sfxSource;
    public AudioClip clickSound;

    public AudioSource bgmSource;
    public AudioClip bgmClip;

    private bool bgmStarted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ✅ 여기에 강제 활성화 코드 추가
        gameObject.SetActive(true);
        sfxSource?.gameObject.SetActive(true);
        bgmSource?.gameObject.SetActive(true);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log($"[SoundManager] 씬 로드됨: {scene.name}");

        if (!bgmStarted && scene.name == "Main_Simulation_Scene")
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
            bgmStarted = true;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        int boundCount = 0;

        GameObject obj = GameObject.Find("SoundManager(Clone)"); // 오브젝트 이름
        if (obj != null)
        {
            SoundManager script = obj.GetComponent<SoundManager>();

            foreach (GameObject root in roots)
            {
                Button[] buttons = root.GetComponentsInChildren<Button>(true); // 비활성 포함
                foreach (Button btn in buttons)
                {
                    btn.onClick.AddListener(script.PlayClickSound);
                    boundCount++;
                }
            }
        }
        //Debug.Log($"[SoundManager] {scene.name} 씬의 버튼 {boundCount}개에 클릭 사운드 연결 완료");
    }

    public void PlayClickSound()
    {
        /*Debug.Log("[SoundManager] PlayClickSound 호출됨");
        Debug.Log(" - clickSound 있음? " + (clickSound != null));
        Debug.Log(" - sfxSource 있음? " + (sfxSource != null));
        Debug.Log(" - sfxSource 컴포넌트 enabled? " + (sfxSource?.enabled ?? false));
        Debug.Log(" - sfxSource 오브젝트 activeInHierarchy? " + (sfxSource?.gameObject.activeInHierarchy ?? false));*/

        if (clickSound != null && sfxSource != null && sfxSource.enabled && sfxSource.gameObject.activeInHierarchy)
        {
            sfxSource.PlayOneShot(clickSound);
            //Debug.Log("[SoundManager] 🎵 사운드 재생 시도됨!");
        }
        else
        {
            //Debug.LogWarning("[SoundManager] ❌ 조건 불충분 - 재생되지 않음");
        }
    }


    public void ToggleBGM(bool on)
    {
        if (bgmSource == null) return;
        if (on) bgmSource.Play();
        else bgmSource.Pause();
    }
}




