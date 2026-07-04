using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class Notice
{
    public int notice_id;
    public string title;
    public string content;
    public string created_at;
}

[Serializable]
public class NoticeResponse
{
    public bool ok;
    public Notice[] notices;
    public string error;   // 에러 발생 시 서버에서 오는 메시지
}



public class NoticeFetcher : MonoBehaviour
{
    public Transform Content;
    public GameObject Notice_Group_Prefab;
    private string serverUrl;
    //private string serverUrl = "https://15.165.159.228:8000/notices"; // 서버 주소 맞게 수정

    /*void Start()
    {
        StartCoroutine(GetNotices());
    }*/

    public void SetNotice()
    {
        /*for (int i = 0; i < 5; i++)
        {
            GameObject group = Instantiate(Notice_Group_Prefab, Content);
            var titleObj = group.transform.Find("Button/TitleText");
            if (titleObj != null)
            {
                titleObj.GetComponent<TextMeshProUGUI>().text = "공지 제목 들어감!";
            }
            var detailObj = group.transform.Find("Panel/DetailText");
            if (detailObj != null)
            {
                detailObj.GetComponent<TextMeshProUGUI>().text = "공지 상세 내용 들어감!";
            }

            Transform btnTransform = group.transform.Find("Button");
            Button BtnObj = btnTransform.GetComponent<Button>();
            BtnObj.transition = Selectable.Transition.None;
            var cb = BtnObj.colors;
            cb.normalColor = cb.highlightedColor = cb.pressedColor =
            cb.selectedColor = cb.disabledColor = Color.white;
            cb.colorMultiplier = 1f;
            BtnObj.colors = cb;
        }*/
        if (PlayerPrefs.GetInt("Login_State") == 0)
        {
            foreach (Transform child in Content)
            {
                Destroy(child.gameObject);
            }
            AddNotice("공지사항 오프라인 상태", "공지사항이 보고 싶다면 인터넷을 연결해 주시기 바랍니다.");
        }
        else if (PlayerPrefs.GetInt("Login_State") == 1)
        {
            foreach (Transform child in Content)
            {
                Destroy(child.gameObject);
            }
            StartCoroutine(GetNotices());
        }
        else if (PlayerPrefs.GetInt("Login_State") == 2)
        {
            foreach (Transform child in Content)
            {
                Destroy(child.gameObject);
            }
            AddNotice("공지사항 오프라인 상태", "공지사항이 보고 싶다면 인터넷을 연결해 주시기 바랍니다.");
        }
    }

    public void AddNotice(string title, string content)
    {
        GameObject group = Instantiate(Notice_Group_Prefab, Content);
        var titleObj = group.transform.Find("Button/TitleText");
        if (titleObj != null)
        {
            titleObj.GetComponent<TextMeshProUGUI>().text = title;
        }
        var detailObj = group.transform.Find("Panel/DetailText");
        if (detailObj != null)
        {
            detailObj.GetComponent<TextMeshProUGUI>().text = content;
        }

        Transform btnTransform = group.transform.Find("Button");
        Button BtnObj = btnTransform.GetComponent<Button>();
        BtnObj.transition = Selectable.Transition.None;
        var cb = BtnObj.colors;
        cb.normalColor = cb.highlightedColor = cb.pressedColor =
        cb.selectedColor = cb.disabledColor = Color.white;
        cb.colorMultiplier = 1f;
        BtnObj.colors = cb;
    }

    IEnumerator GetNotices()
    {
        serverUrl = PlayerPrefs.GetString("ServerUrl");
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl+ "/notices"))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            request.certificateHandler = new BypassCertificate(); // ✅ 여기만 추가!

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ 요청 실패: " + request.error);
                AddNotice("서버 요청 실패", "서버 요청이 실패하였습니다..");
            }
            else
            {
                Debug.Log("✅ 서버 응답: " + request.downloadHandler.text);

                try
                {
                    NoticeResponse res = JsonUtility.FromJson<NoticeResponse>(request.downloadHandler.text);

                    if (res.ok)
                    {
                        foreach (var notice in res.notices)
                        {
                            Debug.Log($"[공지]\nID: {notice.notice_id}\n제목: {notice.title}\n내용: {notice.content}\n작성일: {notice.created_at}");
                            AddNotice(notice.title, notice.content);
                        }
                    }
                    else
                    {
                        Debug.LogError("⚠ 서버 에러: " + res.error);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("⚠ JSON 파싱 실패: " + e.Message);
                }
            }
        }
    }
    class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // 인증서 무시하고 통과시킴
        }
    }
}
