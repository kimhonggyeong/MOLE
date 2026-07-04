using UnityEngine;
using UnityEngine.UI;

public class PaperCardHandler : MonoBehaviour
{
    public Paper paper;

    public void SetPaper(Paper p)
    {
        paper = p;
        Debug.Log("[📄 SetPaper] 논문 제목: " + p.title_en);  // 또는 p.@abstract

        // 버튼 클릭 이벤트 연결
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners(); // 중복 방지
            btn.onClick.AddListener(OnCardClicked);
        }
    }

    public void OnCardClicked()
    {
        Debug.Log("[🖱 버튼 클릭됨]");  // 클릭 시 무조건 출력

        if (paper != null)
        {
            Debug.Log("📦 응답 JSON: " + JsonUtility.ToJson(paper)); // 실제 데이터 로그 출력
            PaperDetailUI.Instance.ShowPaperDetail(paper);
        }
        else
        {
            Debug.LogWarning("[⚠️ Paper가 비어 있음] 클릭은 되었지만 paper가 null입니다.");
        }
    }

}



