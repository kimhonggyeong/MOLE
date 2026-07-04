using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위한 네임스페이스

public class SceneController : MonoBehaviour
{
    // 로그인 패널에서 회원가입 버튼 클릭 시 호출
    public void OnSignUpButtonClick()
    {
        Debug.Log("회원가입 버튼 클릭됨");
        // 예시: 회원가입 씬으로 전환
        SceneManager.LoadScene("SignUpScene");
    }

    // 로그인 패널에서 아이디/비밀번호 찾기 버튼 클릭 시 호출
    public void OnFindIDPWButtonClick()
    {
        Debug.Log("아이디/비밀번호 찾기 버튼 클릭됨");
        // 예시: 아이디/비밀번호 찾기 씬으로 전환
        SceneManager.LoadScene("FindIDPWScene");
    }

    // Permission 패널에서 계속하기 버튼 클릭 시 호출
    public void OnContinueButtonClick()
    {
        Debug.Log("계속하기 버튼 클릭됨");
        // 예시: SignUp 씬으로 전환
        SceneManager.LoadScene("SignupScene");
    }

    // Signup 패널에서 가입하기 버튼 클릭 시 호출
    public void OnSignUpButtonClick_SignUp()
    {
        Debug.Log("가입하기 버튼 클릭됨");
        // 예시: 로그인 씬으로 전환
        SceneManager.LoadScene("LoginScene");
    }
}
