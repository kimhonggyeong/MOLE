using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene("NextScene"); // 여기에 이동할 씬 이름 입력
    }

}
