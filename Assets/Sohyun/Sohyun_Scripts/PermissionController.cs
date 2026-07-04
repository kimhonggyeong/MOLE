using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PermissionController : MonoBehaviour
{
    [Header("체크 아이콘 및 텍스트")]
    public Image fullImage, repoImage, personalImage;
    public TMP_Text fullText, repoText, personalText;

    [Header("계속하기 버튼")]
    public Button continueButton;

    [Header("체크 / 해제 상태 스프라이트")]
    public Sprite checkedSprite;   // 파란 체크 아이콘
    public Sprite uncheckedSprite; // 회색 체크 아이콘

    private bool repoChecked = false;
    private bool personalChecked = false;

    private Color activeColor = new Color32(0x3C, 0x78, 0xF2, 255);  // 파란색
    private Color defaultColor = Color.gray;                        // 회색

    private void Start()
    {
        // 초기화 상태 설정
        ApplyState(fullImage, fullText, false);
        ApplyState(repoImage, repoText, false);
        ApplyState(personalImage, personalText, false);
        continueButton.interactable = false;
        SetContinueButtonColor(false);
    }

    // 전체 동의 클릭
    public void OnClickFullAgreement()
    {
        bool willCheck = !(repoChecked && personalChecked); // 둘 다 true일 때만 해제

        repoChecked = willCheck;
        personalChecked = willCheck;

        ApplyState(fullImage, fullText, willCheck);
        ApplyState(repoImage, repoText, willCheck);
        ApplyState(personalImage, personalText, willCheck);

        continueButton.interactable = willCheck;
        SetContinueButtonColor(willCheck);
    }

    // 저장소 권한 동의 클릭
    public void OnClickRepository()
    {
        repoChecked = !repoChecked;
        ApplyState(repoImage, repoText, repoChecked);
        UpdateFullAgreementAndButton();
    }

    // 개인기록 동의 클릭
    public void OnClickPersonal()
    {
        personalChecked = !personalChecked;
        ApplyState(personalImage, personalText, personalChecked);
        UpdateFullAgreementAndButton();
    }

    // 공통 상태 적용 함수 (스프라이트 & 텍스트 색상)
    private void ApplyState(Image img, TMP_Text txt, bool isChecked)
    {
        if (img != null) img.sprite = isChecked ? checkedSprite : uncheckedSprite;
        if (txt != null) txt.color = isChecked ? activeColor : defaultColor;
    }

    // 부분 동의 시 FullAgreement 상태 동기화 + 버튼 상태 갱신
    private void UpdateFullAgreementAndButton()
    {
        bool all = repoChecked && personalChecked;
        ApplyState(fullImage, fullText, all);
        continueButton.interactable = all;
        SetContinueButtonColor(all);
    }

    // 버튼 색상 전환 (Button → Image 컴포넌트 필요)
    private void SetContinueButtonColor(bool active)
    {
        Image btnImage = continueButton.GetComponent<Image>();
        if (btnImage != null)
        {
            btnImage.color = active ? activeColor : defaultColor;
        }
    }
}
