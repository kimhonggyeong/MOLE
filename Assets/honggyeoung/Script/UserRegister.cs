using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.Rendering.DebugUI;

public class UserRegister : MonoBehaviour
{

    //public GameObject FindAccountUIManagerObj;
    public FindAccountUIManager find_account_uI_manage;

    /*public string serverUrl = "https://15.164.210.13:8000/users";*/
    //private string serverUrl = PlayerPrefs.GetString("ServerUrl");
    public TMP_InputField IDInputField;
    public TMP_InputField PWInputField;
    public TMP_InputField EmailInputField;
    public TMP_InputField NicknameInputField;
    public TMP_InputField Num1InputField;
    public TMP_InputField Num2InputField;
    public TMP_InputField Num3InputField;
    public TMP_Dropdown Year;
    public TMP_Dropdown Month;
    public TMP_Dropdown Day;
    public TMP_InputField Department;
    public TMP_Dropdown Grade;

    private string serverUrl;


    public void OnRegisterTest()
    {
        Debug.Log("입력된 IDInputField: " + IDInputField.text);
        Debug.Log("입력된 PWInputField: " + PWInputField.text);
        Debug.Log("입력된 EmailInputField: " + EmailInputField.text);
        Debug.Log("입력된 NicknameInputField: " + NicknameInputField.text);
        Debug.Log("입력된 전화: " + Num1InputField.text+"-"+Num2InputField.text+"-"+Num3InputField.text);
        Debug.Log("입력된 날짜: " + Year.options[Year.value].text+"년"+ Month.options[Month.value].text+"월"+ Day.options[Day.value].text+"일");
        Debug.Log("입력된 Department: " + Department.text);
        Debug.Log("입력된 Grade: " + Grade.options[Grade.value].text);
    }


    public void OnRegisterButtonClicked()
    {
        StartCoroutine(SendUserData());
    }

    public void OnDeleteUserButtonClicked(string userId)
    {
        StartCoroutine(DeleteUser(userId));
    }

    /*public void OnGetUserInfoButtonClicked(string userId)
    {
        StartCoroutine(GetUserInfo(userId));
    }*/

    public void OnGetUserIDButtonClicked()
    {
        StartCoroutine(GetUserIdByEmail());
    }

    IEnumerator SendUserData()
    {
        // 서버에서 salt를 생성하므로 클라이언트에서는 평문 비밀번호만 전송
        User user = new User
        {
            user_id = IDInputField.text,
            username = NicknameInputField.text,
            password = PWInputField.text,  // 평문 그대로 전송
            email = EmailInputField.text,
            birth_date =  Year.options[Year.value].text + "-" 
            + Month.options[Month.value].text + "-" 
            + Day.options[Day.value].text

        };

        string json = JsonUtility.ToJson(user);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        serverUrl = PlayerPrefs.GetString("ServerUrl");

        using (UnityWebRequest request = UnityWebRequest.Put(serverUrl+ "/users/register", bodyRaw))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new BypassCertificate();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ 등록 실패: {request.error}");
                ServerResponse response = JsonUtility.FromJson<ServerResponse>(request.downloadHandler.text);
                Debug.LogError(response.message);
            }
            else
            {
                Debug.Log("✅ 유저 등록 성공");
                ServerResponse response = JsonUtility.FromJson<ServerResponse>(request.downloadHandler.text);
                Debug.Log(response.message); // 서버 응답 확인
            }
        }
    }

    IEnumerator DeleteUser(string userId)
    {
        // JSON 생성
        DeleteRequest deleteReq = new DeleteRequest { user_id = userId };
        string json = JsonUtility.ToJson(deleteReq);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        serverUrl = PlayerPrefs.GetString("ServerUrl");

        using (UnityWebRequest request = UnityWebRequest.Put(serverUrl+ "/users/delete", bodyRaw))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new BypassCertificate();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ 회원 삭제 실패: {request.error}");
                Debug.LogError(request.downloadHandler.text);
            }
            else
            {
                ServerResponse response = JsonUtility.FromJson<ServerResponse>(request.downloadHandler.text);
                Debug.Log($"✅ 회원 삭제 성공: {response.message}");
            }
        }
    }

    IEnumerator GetUserInfo(string userId)
    {
        serverUrl = PlayerPrefs.GetString("ServerUrl");

        string url = serverUrl+$"/users/info?user_id={userId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.certificateHandler = new BypassCertificate();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            string json = request.downloadHandler.text;
            Debug.Log($"[서버 응답] {json}");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ 유저 정보 조회 실패: {request.error}");
            }
            else
            {
                try
                {
                    UserWrapper wrapper = JsonUtility.FromJson<UserWrapper>(json);
                    UserInfo user = wrapper.user;

                    Debug.Log($"✅ 유저 정보\nID: {user.user_id}\n이름: {user.name}\n이메일: {user.email}\n");
                }
                catch
                {
                    Debug.LogError("⚠️ JSON 파싱 실패");
                }
            }
        }
    }
    IEnumerator GetUserIdByEmail()
    {
        string serverUrl = PlayerPrefs.GetString("ServerUrl");
        string raw = find_account_uI_manage.emailIDInput.text ?? "";
        string norm = raw.Trim().ToLower();                      // 서버도 소문자/트림 가정
        if (string.IsNullOrEmpty(norm))
        {
            Debug.LogError("이메일을 입력하세요.");
            yield break;
        }

        // ❗ 서버 라우트에 맞게 변경: /users/info?email=...
        string url = $"{serverUrl}/users/info?email={UnityWebRequest.EscapeURL(norm)}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.certificateHandler = new BypassCertificate();            // 개발용
            request.disposeCertificateHandlerOnDispose = true;
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Accept", "application/json");
            request.timeout = 15;

            yield return request.SendWebRequest();

            string body = request.downloadHandler?.text ?? "";
            long code = request.responseCode;
            Debug.Log($"[GetUserIdByEmail] url={url}\ncode={code}\nerr={request.error}\nbody={body}");

            // 2xx 성공
            if (request.result == UnityWebRequest.Result.Success &&
                code >= 200 && code < 300)
            {
                var wrapper = JsonUtility.FromJson<UserWrapper>(body);
                if (wrapper != null && wrapper.ok && wrapper.user != null && !string.IsNullOrEmpty(wrapper.user.user_id))
                {
                    Debug.Log($"✅ 조회 성공: {wrapper.user.email} → ID {wrapper.user.user_id}");
                    find_account_uI_manage.OnClickFindID(wrapper.user.user_id, DateTime.Today.ToString("yyyy-MM-dd"));
                    yield break;
                }

                // 200이지만 ok=false 또는 파싱 실패
                string msg = wrapper?.error ?? "응답 파싱 실패 또는 user 필드 없음";
                Debug.LogError($"⚠️ {msg}");
                yield break;
            }

            // 404: 서버에서 ‘미존재’를 404로 주는 설계
            if (code == 404)
            {
                Debug.LogError("❌ 해당 이메일로 가입된 사용자가 없습니다. 회원가입을 진행해 주세요.");
                // 필요하면: find_account_uI_manage.ShowSignupPrompt();
                yield break;
            }

            // 그 외 에러
            Debug.LogError($"❌ 유저 ID 조회 실패: HTTP/{code} {request.error}\n{body}");
        }
    }

    class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // 인증서 무시하고 통과시킴 (주의: 운영 환경에선 제거)
        }
    }

    [System.Serializable]
    public class User
    {
        public string user_id;
        public string username;
        public string password;
        public string email;
        public string birth_date;
    }

    [System.Serializable]
    public class ServerResponse
    {
        public string message;
    }

    [System.Serializable]
    public class DeleteRequest
    {
        public string user_id;
    }

    [Serializable]
    class UserWrapper
    {
        public bool ok;
        public string error;   // _err(...) 쓰면 내려올 수 있음
        public string code;    // "NOT_FOUND" 등
        public UserInfo user;  // _ok({ user: {...} })
    }
    [Serializable]
    class UserInfo
    {
        public string user_id;
        public string email;   // 서버에서 내려주면 매핑됨
        public string name;    // 선택
    }

}
