using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;

public class ChatClient : MonoBehaviour
{
    public GameObject chatBubbleUserPrefab; // 프리팹 드래그
    public GameObject chatBubbleBotPrefab; // 프리팹 드래그
    public Transform chatContent; // ScrollView의 Content 드래그
    public ScrollRect scrollRect;
    public TMP_InputField inputField;
    //private string serverUrl = "http://localhost:8000";
    //private string serverUrl = PlayerPrefs.GetString("ServerUrl");
    private string serverUrl;
    private string answerText;

    public void OnSendClicked()
    {
        StartCoroutine(SendQuestion(inputField.text));
    }
    public void AddUserChat()
    {
        if (inputField.text != "")
        {
            GameObject newUserBubble = Instantiate(chatBubbleUserPrefab, chatContent);
            // 🔥 레이아웃 즉시 갱신 (이게 핵심)
            LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());

            // 프리팹 안에 있는 TMP_Text 컴포넌트 찾아서 텍스트 변경
            TMP_Text Usertext = newUserBubble.GetComponentInChildren<TMP_Text>();
            Usertext.text = inputField.text;
            StartCoroutine(SendQuestion(inputField.text)); //AI 모델에 메시지 보내기
            inputField.text = "";

            // 🔽 스크롤을 맨 아래로 내리는 것도 같이 하면 자연스러움
            ScrollToBottom();
            Canvas.ForceUpdateCanvases();
        }
    }
    void AddBotChat()
    {
        GameObject newBotBubble = Instantiate(chatBubbleBotPrefab, chatContent);

        // 🔥 레이아웃 즉시 갱신 (이게 핵심)
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());

        TMP_Text Bottext = newBotBubble.GetComponentInChildren<TMP_Text>();
        Bottext.text = answerText;


        // 🔽 스크롤을 맨 아래로 내리는 것도 같이 하면 자연스러움
        ScrollToBottom();
        Canvas.ForceUpdateCanvases();
    }
    public void ScrollToBottom()
    {
        StartCoroutine(ScrollToBottomNextFrame());
    }
    IEnumerator SendQuestion(string message)
    {
        serverUrl = PlayerPrefs.GetString("ServerUrl");
        Debug.Log(serverUrl);
        //serverUrl = "https://15.165.159.228:8000";
        string json = JsonUtility.ToJson(new Message(message));
        UnityWebRequest request = new UnityWebRequest(serverUrl+ "/chat/generate", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.certificateHandler = new BypassCertificate(); // ✅ 여기만 추가!

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string rawJson = request.downloadHandler.text;
            Debug.Log("🔍 응답 원문: " + rawJson);

            var parsedJson = JSON.Parse(rawJson);           // 충돌 없는 변수명
            string answer = parsedJson["response"];         // JSON에서 응답 추출

            answerText = answer;
            AddBotChat();
        }
        else
        {
            answerText = "Error: " + request.error;
            AddBotChat();
        }
    }
    IEnumerator ScrollToBottomNextFrame()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Canvas.ForceUpdateCanvases(); // 캔버스 갱신
        scrollRect.verticalNormalizedPosition = 0f;
    }

}

[System.Serializable]
public class Message
{
    public string message;
    public Message(string msg) { message = msg; }
}


class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; // 인증서 무시하고 통과시킴
    }
}