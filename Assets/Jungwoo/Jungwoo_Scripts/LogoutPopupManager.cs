using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutPopupManager : MonoBehaviour
{
    public GameObject logoutPopupPanel;  // 팝업 패널
    public GameObject backgroundBlocker; // 팝업 외부 클릭 방지용 (Optional)

    // 팝업 열기
    public void OpenPopup()
    {
        logoutPopupPanel.SetActive(true);
        if (backgroundBlocker != null)
            backgroundBlocker.SetActive(true);
        Time.timeScale = 0f; // 게임 정지 (선택적)
    }

    // 돌아가기
    public void ClosePopup()
    {
        logoutPopupPanel.SetActive(false);
        if (backgroundBlocker != null)
            backgroundBlocker.SetActive(false);
        Time.timeScale = 1f;
    }

    // 로그아웃 (다른 씬으로 이동)
    public void Logout()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Login_Scene");  // 이동할 씬 이름을 여기에 입력
    }
}
