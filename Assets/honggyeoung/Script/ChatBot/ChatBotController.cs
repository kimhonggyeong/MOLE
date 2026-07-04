using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;

public class ChatBot : MonoBehaviour
{
    public GameObject chatBubbleUserPrefab; // 프리팹 드래그
    public GameObject chatBubbleBotPrefab; // 프리팹 드래그
    public Transform chatContent; // ScrollView의 Content 드래그
    public ScrollRect scrollRect;
    public TMP_InputField inputField;
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
            inputField.text = "";

            // 🔽 스크롤을 맨 아래로 내리는 것도 같이 하면 자연스러움
            ScrollToBottom();
            Canvas.ForceUpdateCanvases();
            Invoke("AddBotChat", 1f); // 1초 뒤에 SayHello 함수 실행
        }
    }
    public void AddBotChat()
    {
        GameObject newBotBubble = Instantiate(chatBubbleBotPrefab, chatContent);

        // 🔥 레이아웃 즉시 갱신 (이게 핵심)
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());

        TMP_Text Bottext = newBotBubble.GetComponentInChildren<TMP_Text>();
        Bottext.text = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";


        // 🔽 스크롤을 맨 아래로 내리는 것도 같이 하면 자연스러움
        ScrollToBottom();
        Canvas.ForceUpdateCanvases();
    }
    public void ScrollToBottom()
    {
        StartCoroutine(ScrollToBottomNextFrame());
    }
    IEnumerator ScrollToBottomNextFrame()
    {
        yield return new WaitForEndOfFrame(); 
        yield return new WaitForEndOfFrame();

        Canvas.ForceUpdateCanvases(); // 캔버스 갱신
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
