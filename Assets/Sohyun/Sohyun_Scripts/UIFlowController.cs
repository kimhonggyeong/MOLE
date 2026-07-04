using UnityEngine;

public class UIFlowController : MonoBehaviour
{
    [Header("패널들")]
    public GameObject loginPanel;
    public GameObject permissionPanel;
    public GameObject signupPanel;

    public GameObject findIDPanel;
    public GameObject findPWPanel;
    public GameObject resultIDPanel;
    public GameObject resultPWPanel;

    private void Start()
    {
        ShowOnly(loginPanel);
    }

    // 로그인 → 권한동의
    public void OnClickSignUpButton()
    {
        ShowOnly(permissionPanel);
    }

    // 권한동의 → 회원가입
    public void OnClickContinueButton()
    {
        ShowOnly(signupPanel);
    }

    // 회원가입 → 로그인으로 복귀
    public void OnClickBackToLogin()
    {
        ShowOnly(loginPanel);
    }

    // 로그인 → 아이디/비밀번호 찾기
    public void OnClickFindID()
    {
        ShowOnly(findIDPanel);
    }

    public void OnClickFindPW()
    {
        ShowOnly(findPWPanel);
    }

    // 아이디/비밀번호 찾기 결과 → 로그인 복귀
    public void OnClickBackToLoginFromResult()
    {
        resultIDPanel.SetActive(false);
        resultPWPanel.SetActive(false);
        ShowOnly(loginPanel);
    }

    // 유틸: 하나만 보여주고 나머지는 전부 꺼버림
    private void ShowOnly(GameObject panelToShow)
    {
        loginPanel.SetActive(false);
        permissionPanel.SetActive(false);
        signupPanel.SetActive(false);
        findIDPanel.SetActive(false);
        findPWPanel.SetActive(false);
        resultIDPanel.SetActive(false);
        resultPWPanel.SetActive(false);

        if (panelToShow != null)
            panelToShow.SetActive(true);
    }
}
