using System.Collections;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AuthEmail : MonoBehaviour
{
    public GameObject FindAccountUIManagerObj;
    FindAccountUIManager find_account_uI_manage;
    public UIFlowController UI_Flow_Controller;
    public Login_Btn_Controller login_btn_controller;



    /*[Header("서버 설정")]
    [Tooltip("예: https://your-domain-or-ip:8000")]
    private string baseUrl = "https://15.165.159.228:8000";*/
    private string serverUrl;

    [Tooltip("개발 환경에서 자가서명 인증서 허용 (배포시 반드시 false)")]
    public bool allowSelfSigned = true;

    //public TMP_Text statusText;

    public TMP_InputField IDNameInput;
    public TMP_InputField IDEmailInput;
    public TMP_InputField IDNumberInput;

    public TMP_InputField PWNameInput;
    public TMP_InputField PWEmailInput;
    public TMP_InputField PWNumberInput;

    public TMP_InputField IDLoginInput;
    public TMP_InputField PWLoginInput;

    // ★ 테스트용 하드코드 데이터 (원하면 인스펙터에서 바꿔도 됨)
    [Header("테스트 데이터(하드코드)")]
    private string testEmail = "testmole@gmail.com";
    private string testCode = "000000";           // 메일 서버 없이 테스트 시, 서버에서 000000 같은 고정코드로 설정해두면 편함
    private string testNewPw = "NewP@ssw0rd!";

    void Start()
    {
        find_account_uI_manage = FindAccountUIManagerObj.GetComponent<FindAccountUIManager>();
    }

    public void IDInputCheck()
    {
        Debug.Log("testName" + IDNameInput.text);
        Debug.Log("testEmailI" + IDEmailInput.text);
        Debug.Log("testNumber" + IDNumberInput.text);
    }

    public void PWInputCheck()
    {
        Debug.Log("testName" + PWNameInput.text);
        Debug.Log("testEmailI" + PWEmailInput.text);
        Debug.Log("testNumber" + PWNumberInput.text);
    }

    // -------------------------
    // 버튼 핸들러 (코루틴 호출)
    // -------------------------
    public void IDEmailSendCode()
    {
        Debug.Log("이메일"+ IDEmailInput.text);
        StartCoroutine(PostJson(
            Url("/users/send-code"),
            Json($"{{\"email\":\"{Escape(IDEmailInput.text)}\"}}"),
            onOk: (txt) => Log($"[send-code OK]\n{txt}"),
            onErr: (err) => LogError($"[send-code ERR] {err}")
        ));
    }
    public void PWEmailSendCode()
    {
        Debug.Log("이메일" + PWEmailInput.text);
        StartCoroutine(PostJson(
            Url("/users/send-code"),
            Json($"{{\"email\":\"{Escape(PWEmailInput.text)}\"}}"),
            onOk: (txt) => Log($"[send-code OK]\n{txt}"),
            onErr: (err) => LogError($"[send-code ERR] {err}")
        ));        
    }

    /*public void TestVerify()
    {
        Debug.Log("이메일" + testEmail + "코드" + testCode);
        StartCoroutine(PostJson(
            Url("/users/verify-code"),
            Json($"{{\"email\":\"{Escape(testEmail)}\",\"code\":\"{Escape(testCode)}\"}}"),
            onOk: (txt) => Log($"[verify-code OK]\n{txt}"),
            onErr: (err) => LogError($"[verify-code ERR] {err}")
        ));
    }*/

    /*    public void IdEmailVerify()
        {
            Debug.Log("이메일" + IDEmailInput.text + "코드" + IDNumberInput.text);
            StartCoroutine(PostJson(
                Url("/users/verify-code"),
                Json($"{{\"email\":\"{Escape(IDEmailInput.text)}\",\"code\":\"{Escape(IDNumberInput.text)}\"}}"),
                        onOk: (txt) =>
                        {
                            Debug.Log($"[verify-code OK]\n{txt}");
                            bool isVerified = false;

                            // 1) JSON에 verified 필드가 있는 경우
                            try
                            {
                                var res = JsonUtility.FromJson<VerifyRes>(txt);
                                isVerified = res.verified;
                                if (!string.IsNullOrEmpty(res.message))
                                    Debug.Log($"server msg: {res.message}");
                            }
                            catch { *//* JSON 형태가 다르면 아래 2)로 처리 *//* }

                            // 2) 백엔드가 다른 포맷이면 키워드로 보조 판정 (임시)
                            if (!isVerified)
                                isVerified = txt.Contains("verified") && txt.Contains("true")
                                          || txt.Contains("인증 성공") || txt.Contains("OK");

                            if (isVerified)
                                find_account_uI_manage.SetButtonState(find_account_uI_manage.findIDBtn, true);
                            else
                                Debug.LogError("인증 실패로 판단됨");
                        },
                        onErr: (err) =>
                        {
                            LogError($"[verify-code ERR] {err}");
                        }
        ));
        }*/
    public void IdEmailVerify()
    {
        Debug.Log("이메일" + IDEmailInput.text + "코드" + IDNumberInput.text);
        StartCoroutine(PostJson(
            Url("/users/verify-code"),
            Json($"{{\"email\":\"{Escape(IDEmailInput.text)}\",\"code\":\"{Escape(IDNumberInput.text)}\"}}"),
                    onOk: (txt) =>
                    {
                        find_account_uI_manage.SetButtonState(find_account_uI_manage.findIDBtn, true);
                    },
                    onErr: (err) =>
                    {
                        LogError($"[verify-code ERR] {err}");
                    }
    ));
    }
    public void PwEmailVerify()
    {
        Debug.Log("이메일" + PWEmailInput.text + "코드" + PWNumberInput.text);
        StartCoroutine(PostJson(
            Url("/users/verify-code"),
            Json($"{{\"email\":\"{Escape(PWEmailInput.text)}\",\"code\":\"{Escape(PWNumberInput.text)}\"}}"),
                    onOk: (txt) =>
                    {
                        find_account_uI_manage.SetButtonState(find_account_uI_manage.findPWBtn, true);
                    },
                    onErr: (err) =>
                    {
                        LogError($"[verify-code ERR] {err}");
                    }
    ));
    }

    /*public void EmailResetPassword()
    {
        Debug.Log("이메일" + testEmail + "코드" + testCode + "새비번" + testNewPw);
        StartCoroutine(PostJson(
            Url("/users/reset-password"),
            Json($"{{\"email\":\"{Escape(testEmail)}\",\"code\":\"{Escape(testCode)}\",\"new_password\":\"{Escape(testNewPw)}\"}}"),
            onOk: (txt) => Log($"[reset-password OK]\n{txt}"),
            onErr: (err) => LogError($"[reset-password ERR] {err}")
        ));
    }*/
    /*public void EmailResetPassword()
    {
        Debug.Log("이메일" + PWEmailInput.text + "코드" + PWNumberInput.text + "새비번" + find_account_uI_manage.newPWInput.text);
        StartCoroutine(PostJson(
            Url("/users/reset-password"),
            Json($"{{\"email\":\"{Escape(PWEmailInput.text)}\",\"code\":\"{Escape(PWNumberInput.text)}\",\"new_password\":\"{Escape(find_account_uI_manage.newPWInput.text)}\"}}"),
            onOk: (txt) => Log($"[reset-password OK]\n{txt}"),
            onErr: (err) => LogError($"[reset-password ERR] {err}")
        ));
    }*/
    public void EmailResetPassword()
    {
        Debug.Log("이메일" + PWEmailInput.text + "새비번" + find_account_uI_manage.newPWInput.text);
        StartCoroutine(PostJson(
            Url("/users/reset-password"),
            Json($"{{\"email\":\"{Escape(PWEmailInput.text)}\",\"new_password\":\"{Escape(find_account_uI_manage.newPWInput.text)}\"}}"),
            onOk: (txt) =>
            {
                Debug.Log("야호 성공이다~~");
                UI_Flow_Controller.OnClickBackToLoginFromResult();
            },
            onErr: (err) => LogError($"[reset-password ERR] {err}")
        ));
    }

    public void Login()
    {
        Debug.Log($"로그인 시도: {IDLoginInput.text} / {PWLoginInput.text}");

        StartCoroutine(PostJson(
            Url("/users/login"),
            Json($"{{\"identifier\":\"{Escape(IDLoginInput.text)}\",\"password\":\"{Escape(PWLoginInput.text)}\"}}"),
            onOk: (txt) =>
            {
                var res = JsonUtility.FromJson<LoginResponse>(txt);
                Debug.Log($"✅ 로그인 성공\nID: {res.user.user_id}\n이름: {res.user.username}\n이메일: {res.user.email}");
                login_btn_controller.InsertOnLineUsers(res.user.user_id, res.user.username, res.user.email);
            },
            onErr: (err) =>
            {
                Debug.LogError($"[login ERR] {err}");
            }
        ));
    }

    // -------------------------
    // 공통 유틸
    // -------------------------
    string Url(string path)
    {
        serverUrl = PlayerPrefs.GetString("ServerUrl");
        if (string.IsNullOrEmpty(serverUrl)) return path;
        if (!serverUrl.EndsWith("/")) serverUrl += "/";
        return serverUrl + path.TrimStart('/');
    }

    static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");

    static byte[] Json(string s) => Encoding.UTF8.GetBytes(s);

    IEnumerator PostJson(string url, byte[] body, System.Action<string> onOk, System.Action<string> onErr)
    {
        using (var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = 15;

            req.certificateHandler = new BypassCertificate(); // 개발용만!

            /*if (allowSelfSigned)
            {
                req.certificateHandler = new BypassCertificate(); // 개발용만!
            }*/

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                string body1 = req.downloadHandler != null ? req.downloadHandler.text : "";
                onErr?.Invoke($"err={req.error}, code={req.responseCode}, body={body1}");
            }
            else
            {
                onOk?.Invoke(req.downloadHandler.text);
            }
        }
    }

    void Log(string msg)
    {
        Debug.Log(msg);
        //if (statusText) statusText.text = msg;
    }
    void LogError(string msg)
    {
        Debug.LogError(msg);
        //if (statusText) statusText.text = msg;
    }
    class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // 인증서 무시하고 통과시킴
        }
    }
    public struct VerifyRes
    {
        public bool verified;   // 서버가 내려주면 사용
        public string message;  // 로그용
    }

    [System.Serializable]
    public class LoginUser
    {
        public string user_id;
        public string username;
        public string email;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool ok;
        public string error;
        public LoginUser user;
    }
}

