using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignupValidator : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField idField;
    public TMP_InputField pwField;
    public TMP_InputField emailField;
    public TMP_InputField phoneField1;
    public TMP_InputField phoneField2;
    public TMP_InputField phoneField3;
    public TMP_InputField departmentField;

    [Header("Dropdowns")]
    public TMP_Dropdown birthYearDropdown;
    public TMP_Dropdown birthMonthDropdown;
    public TMP_Dropdown birthDayDropdown;
    public TMP_Dropdown gradeDropdown;

    [Header("Signup Button")]
    public Button signupButton;

    private void Start()
    {
        signupButton.interactable = false;

        // 입력 필드 변경 감지
        idField.onValueChanged.AddListener(delegate { Validate(); });
        pwField.onValueChanged.AddListener(delegate { Validate(); });
        emailField.onValueChanged.AddListener(delegate { Validate(); });
        phoneField1.onValueChanged.AddListener(delegate { Validate(); });
        phoneField2.onValueChanged.AddListener(delegate { Validate(); });
        phoneField3.onValueChanged.AddListener(delegate { Validate(); });
        departmentField.onValueChanged.AddListener(delegate { Validate(); });

        // 드롭다운 변경 감지
        birthYearDropdown.onValueChanged.AddListener(delegate { Validate(); });
        birthMonthDropdown.onValueChanged.AddListener(delegate { Validate(); });
        birthDayDropdown.onValueChanged.AddListener(delegate { Validate(); });
        gradeDropdown.onValueChanged.AddListener(delegate { Validate(); });
    }

    private void Validate()
    {
        bool allInputFilled =
            !string.IsNullOrWhiteSpace(idField.text) &&
            !string.IsNullOrWhiteSpace(pwField.text) &&
            !string.IsNullOrWhiteSpace(emailField.text) &&
            !string.IsNullOrWhiteSpace(phoneField1.text) &&
            !string.IsNullOrWhiteSpace(phoneField2.text) &&
            !string.IsNullOrWhiteSpace(phoneField3.text) &&
            !string.IsNullOrWhiteSpace(departmentField.text);

        bool birthSelected =
            birthYearDropdown.value != 0 &&
            birthMonthDropdown.value != 0 &&
            birthDayDropdown.value != 0;

        bool gradeSelected = gradeDropdown.value != 0;

        bool isValid = allInputFilled && birthSelected && gradeSelected;

        signupButton.interactable = isValid;

        // 색상도 변경
        Image btnImage = signupButton.GetComponent<Image>();
        if (btnImage != null)
        {
            Color activeColor = new Color32(0x3C, 0x78, 0xF2, 255);
            Color disabledColor = Color.gray;
            btnImage.color = isValid ? activeColor : disabledColor;
        }
    }
}
