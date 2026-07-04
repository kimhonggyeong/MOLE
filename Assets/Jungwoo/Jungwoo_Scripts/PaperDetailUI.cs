using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaperDetailUI : MonoBehaviour
{
    public static PaperDetailUI Instance;

    [Header("뒤로가기 버튼")]
    public GameObject backButton;

    [Header("패널")]
    public GameObject listPanel;
    public GameObject detailPanel;

    [Header("텍스트 필드")]
    public TMP_Text titleText;
    public TMP_Text titleEnglishText;
    public TMP_Text typeText;
    public TMP_Text authorsText;
    public TMP_Text journalText;
    public TMP_Text dateText;
    public TMP_Text pageText;
    public TMP_Text abstractText;

    [Header("고정 키워드 버튼 (최대 3개)")]
    public TMP_Text keywordText1;
    public TMP_Text keywordText2;
    public TMP_Text keywordText3;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 카드 클릭 시 상세정보 표시
    /// </summary>
    public void ShowPaperDetail(Paper paper)
    {
        Debug.Log("📦 파싱된 Paper 객체: " + JsonUtility.ToJson(paper));

        backButton.SetActive(false);

        // 패널 전환
        listPanel.SetActive(false);
        detailPanel.SetActive(true);

        // 제목
        titleText.text = !string.IsNullOrEmpty(paper.title_ko) ? paper.title_ko : "(제목 없음)";
        titleEnglishText.text = paper.title_ko;

        // 유형 (서버에서 제공 안됨 → 고정 텍스트 또는 생략)
        typeText.text = paper.type != null && paper.type.Length > 0
            ? string.Join(", ", paper.type)
            : "유형 정보 없음";

        // 저자
        authorsText.text = paper.authors_en != null && paper.authors_en.Length > 0
            ? string.Join(", ", paper.authors_en)
            : "저자 정보 없음";

        // 저널, 발행일, 페이지는 서버에 없음 → 빈칸 처리
        journalText.text = paper.journal != null && paper.journal.Length > 0
            ? string.Join(", ", paper.journal)
            : "저널 정보 없음";

        dateText.text = paper.pub_date != null && paper.pub_date.Length > 0
            ? string.Join(", ", paper.pub_date)
            : "날짜 정보 없음";

        pageText.text = paper.pages != null && paper.pages.Length > 0
            ? string.Join(", ", paper.pages)
            : "페이지 정보 없음"; ;

        // 초록
        abstractText.text = !string.IsNullOrEmpty(paper.abstract_ko)
            ? paper.abstract_ko
            : (!string.IsNullOrEmpty(paper.abstract_ko) ? paper.abstract_ko : "요약 없음");

        // 키워드 없음 → 숨김 처리
        keywordText1.text = "";
        keywordText2.text = "";
        keywordText3.text = "";

        keywordText1.transform.parent.gameObject.SetActive(false);
        keywordText2.transform.parent.gameObject.SetActive(false);
        keywordText3.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 뒤로가기 → 리스트 복귀
    /// </summary>
    public void BackToList()
    {
        detailPanel.SetActive(false);
        listPanel.SetActive(true);
        backButton.SetActive(true);
    }
}
