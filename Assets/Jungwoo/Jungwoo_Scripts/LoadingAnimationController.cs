using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingAnimationController : MonoBehaviour
{
    public string nextSceneName = "SampleScene"; // 전환할 씬 이름
    public float animationSpeed = 3f; // 애니메이션 속도 (원하는 배수)

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = animationSpeed;

        // 애니메이션 총 길이 가져오기
        float animationLength = animator.runtimeAnimatorController.animationClips[0].length / animationSpeed;

        // 애니메이션 끝나면 다음 씬으로 넘어가기 (Invoke 사용)
        Invoke("LoadNextScene", animationLength);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
