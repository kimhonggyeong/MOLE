using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SignupInputLimiter : MonoBehaviour
{
    [Header("일반 필드")]
    public TMP_InputField idField;       // 15자
    public TMP_InputField pwField;       // 15자
    public TMP_InputField emailField;    // 30자
    public TMP_InputField nicknameField; // 10자
    public TMP_InputField deptField;     // 학과명 15자

    [Header("전화번호 (숫자만)")]
    public TMP_InputField phone1Field;   // 3자
    public TMP_InputField phone2Field;   // 4자
    public TMP_InputField phone3Field;   // 4자

    // 숫자만 남기는 정규식
    static readonly Regex nonDigit = new Regex(@"[^0-9]", RegexOptions.Compiled);

    void Awake()
    {
        // 글자 수 제한
        if (idField)       idField.characterLimit       = 15;
        if (pwField)       pwField.characterLimit       = 15;
        if (emailField)    emailField.characterLimit    = 30;
        if (nicknameField) nicknameField.characterLimit = 10;
        if (deptField)     deptField.characterLimit     = 15;

        // 비밀번호는 마스킹
        if (pwField) pwField.contentType = TMP_InputField.ContentType.Password;

        // 전화번호: 숫자만 + 각 길이 제한
        SetupPhoneField(phone1Field, 3);
        SetupPhoneField(phone2Field, 4);
        SetupPhoneField(phone3Field, 4);

        // (선택) 아이디에 공백 금지
        if (idField) idField.onValueChanged.AddListener(v =>
        {
            // 공백 제거만, 필요하면 허용 문자 제한식으로 교체 가능
            var cleaned = v.Replace(" ", "");
            if (cleaned.Length > idField.characterLimit) cleaned = cleaned.Substring(0, idField.characterLimit);
            if (cleaned != v) idField.SetTextWithoutNotify(cleaned);
        });

        // (선택) 이메일 공백 금지
        if (emailField) emailField.onValueChanged.AddListener(v =>
        {
            var cleaned = v.Replace(" ", "");
            if (cleaned.Length > emailField.characterLimit) cleaned = cleaned.Substring(0, emailField.characterLimit);
            if (cleaned != v) emailField.SetTextWithoutNotify(cleaned);
        });
    }

    void SetupPhoneField(TMP_InputField f, int maxLen)
    {
        if (!f) return;
        f.characterLimit = maxLen;
        f.contentType = TMP_InputField.ContentType.IntegerNumber; // 숫자 키패드/IME 유도
        f.onValueChanged.AddListener(v =>
        {
            // 숫자만 남기고 길이 자르기 (IME 한글 조합 시에도 안전)
            var digits = nonDigit.Replace(v ?? "", "");
            if (digits.Length > maxLen) digits = digits.Substring(0, maxLen);
            if (digits != v) f.SetTextWithoutNotify(digits);

            // (옵션) 다 채우면 다음 칸으로 자동 포커스
            if (digits.Length == maxLen)
            {
                if (f == phone1Field && phone2Field) phone2Field.Select();
                else if (f == phone2Field && phone3Field) phone3Field.Select();
            }
        });
    }
}
