using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FindAccountUIManager : MonoBehaviour
{
    public enum Mode { FindID, FindPW }
    public Mode currentMode;

    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject findIDPanel;
    public GameObject findPWPanel;
    public GameObject resultIDPanel;
    public GameObject resultPWPanel;

    [Header("Tabs")]
    public Button tabIDInIDPanel;
    public Button tabPWInIDPanel;
    public Button tabIDInPWPanel;
    public Button tabPWInPWPanel;

    [Header("ID Elements")]
    public TMP_InputField emailIDInput;
    public TMP_InputField codeIDInput;
    public Button sendCodeIDBtn;
    public Button verifyIDBtn;
    public Button findIDBtn;

    [Header("PW Elements")]
    public TMP_InputField emailPWInput;
    public TMP_InputField codePWInput;
    public Button sendCodePWBtn;
    public Button verifyPWBtn;
    public Button findPWBtn;

    [Header("Result ID Texts")]
    public TMP_Text resultIDText;
    public TMP_Text joinIDText;

    [Header("PW Reset Panel")]
    public TMP_InputField newPWInput;
    public TMP_InputField confirmPWInput;
    public Button loginFromPWResultBtn;

    [Header("Colors")]
    public Color activeTextColor = Color.white;
    public Color activeBgColor = new Color32(0x3C, 0x78, 0xF2, 255);
    public Color inactiveTextColor = new Color32(0x75, 0x75, 0x75, 255);
    public Color inactiveBgColor = Color.white;

    private void Start()
    {
        SwitchMode(currentMode);
        SetupListeners();
    }

    void SetupListeners()
    {
        // ID 인증 절차
        emailIDInput.onValueChanged.AddListener(_ =>
            SetButtonState(sendCodeIDBtn, !string.IsNullOrWhiteSpace(emailIDInput.text))
        );
        codeIDInput.onValueChanged.AddListener(_ =>
            SetButtonState(verifyIDBtn, !string.IsNullOrWhiteSpace(codeIDInput.text))
        );
        verifyIDBtn.onClick.AddListener(() =>
            SetButtonState(findIDBtn, true)
        );

        // PW 인증 절차
        emailPWInput.onValueChanged.AddListener(_ =>
            SetButtonState(sendCodePWBtn, !string.IsNullOrWhiteSpace(emailPWInput.text))
        );
        codePWInput.onValueChanged.AddListener(_ =>
            SetButtonState(verifyPWBtn, !string.IsNullOrWhiteSpace(codePWInput.text))
        );
        /*verifyPWBtn.onClick.AddListener(() =>
            SetButtonState(findPWBtn, true)
        );*/

        // 비밀번호 재설정 확인 로직
        newPWInput.onValueChanged.AddListener(_ => CheckResetPwInputs());
        confirmPWInput.onValueChanged.AddListener(_ => CheckResetPwInputs());

        // 탭 전환
        tabPWInIDPanel.onClick.AddListener(() => SwitchMode(Mode.FindPW));
        tabIDInPWPanel.onClick.AddListener(() => SwitchMode(Mode.FindID));
    }

    public void SwitchMode(Mode mode)
    {
        currentMode = mode;

        bool isID = mode == Mode.FindID;
        findIDPanel.SetActive(isID);
        findPWPanel.SetActive(!isID);

        SetTabColors(tabIDInIDPanel, tabPWInIDPanel, isID);
        SetTabColors(tabIDInPWPanel, tabPWInPWPanel, isID);

        ResetButtons();
    }

    void SetTabColors(Button idTab, Button pwTab, bool isID)
    {
        var idText = idTab.GetComponentInChildren<TMP_Text>();
        var pwText = pwTab.GetComponentInChildren<TMP_Text>();

        if (idText) idText.color = isID ? activeBgColor : inactiveTextColor;
        if (pwText) pwText.color = isID ? inactiveTextColor : activeBgColor;
    }

    public void SetButtonState(Button button, bool isActive)
    {
        if (button == null) return;

        button.interactable = isActive;

        var text = button.GetComponentInChildren<TMP_Text>();
        var image = button.GetComponent<Image>();

        if (text != null) text.color = isActive ? activeTextColor : inactiveTextColor;
        if (image != null) image.color = isActive ? activeBgColor : inactiveBgColor;
    }

    void ResetButtons()
    {
        SetButtonState(sendCodeIDBtn, false);
        SetButtonState(verifyIDBtn, false);
        SetButtonState(findIDBtn, false);

        SetButtonState(sendCodePWBtn, false);
        SetButtonState(verifyPWBtn, false);
        SetButtonState(findPWBtn, false);

        SetButtonState(loginFromPWResultBtn, false);
    }

    void CheckResetPwInputs()
    {
        bool filled = string.Equals(newPWInput.text, confirmPWInput.text)
              && (!string.IsNullOrWhiteSpace(newPWInput.text)
              && !string.IsNullOrWhiteSpace(confirmPWInput.text));

        SetButtonState(loginFromPWResultBtn, filled);
    }

    // ID 찾기 → 결과 패널
    public void OnClickFindID(string resultID, string joinID)
    {
        findIDPanel.SetActive(false);
        resultIDPanel.SetActive(true);
        resultIDText.text = resultID;    // 예시
        joinIDText.text = joinID;     // 예시
    }

    // PW 찾기 → 재설정 패널
    public void OnClickFindPW()
    {
        findPWPanel.SetActive(false);
        resultPWPanel.SetActive(true);
    }

    // 로그인으로 돌아가기
    public void OnClickBackToLogin()
    {
        resultIDPanel.SetActive(false);
        resultPWPanel.SetActive(false);
        findIDPanel.SetActive(false);
        findPWPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    // ID 결과 패널에서 → 비밀번호 찾기 화면으로
    public void OnClickGoToFindPWFromResult()
    {
        resultIDPanel.SetActive(false);
        findPWPanel.SetActive(true);
        SwitchMode(Mode.FindPW);
    }

    // ID 패널에서 직접 비밀번호 찾기로 이동
    public void OnClickGoToFindPWFromIDPanel()
    {
        findIDPanel.SetActive(false);
        findPWPanel.SetActive(true);
        SwitchMode(Mode.FindPW);
    }
}
