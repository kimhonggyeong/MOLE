using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// TMP_InputField에 '마지막 글자 잠시 노출 후 자동 마스킹' 기능을 제공합니다.
/// - contentType은 Standard로 둡니다(Password 사용 X).
/// - 실제 비밀번호는 realText에 저장되며, InputField에는 마스킹된 텍스트만 표시됩니다.
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class CustomPasswordMaskWithDelay : MonoBehaviour
{
    [Header("Targets")]
    public TMP_InputField input;              // 비밀번호 입력칸(TMP)

    [Header("Mask Options")]
    public char maskChar = '●';               // 마스킹 문자
    [Tooltip("마지막 글자 노출 시간(초)")]
    public float revealDuration = 1.0f;       // 마지막 글자 보여주는 시간

    private string realText = "";             // 실제 비밀번호
    private bool suppressCallback = false;    // 이벤트 루프 방지
    private Coroutine hideCoroutine;          // 지연 마스킹 코루틴

    void Reset()
    {
        input = GetComponent<TMP_InputField>();
    }

    void Awake()
    {
        if (!input) input = GetComponent<TMP_InputField>();

        // 반드시 Standard로 둬야 우리가 직접 마스킹 가능
        input.contentType = TMP_InputField.ContentType.Standard;
        input.onValueChanged.AddListener(OnValueChanged);
        input.onDeselect.AddListener(_ => ForceFullMask()); // 포커스 잃으면 즉시 전체 마스킹
    }

    /// <summary>사용자 입력 변화 처리</summary>
    private void OnValueChanged(string displayed)
    {
        if (suppressCallback) return;

        // IME/붙여넣기/삭제 등 모든 케이스를 흡수: 표시된 글자 길이에 맞춰 실제 텍스트 갱신
        // 단, 우리는 항상 "표시는 (마스크...+마지막글자)" 형태를 만들기 때문에
        // 사용자가 입력한 '새 글자'를 추출하는 로직이 필요.
        // 가장 안전한 방법: input.caretPosition 기준으로 마지막 글자를 가져오고, 나머지는 이전 realText 유지.
        // 단순하고 견고하게: 표시된 문자열과 realText 길이를 비교해 변화량만 반영.

        // 1) 길이가 줄어든 경우(백스페이스/삭제)
        if (displayed.Length < realText.Length)
        {
            realText = realText.Substring(0, Mathf.Max(0, displayed.Length));
        }
        // 2) 길이가 늘어난 경우(추가/붙여넣기)
        else if (displayed.Length > realText.Length)
        {
            // 새로 들어온 덩어리
            string added = displayed.Substring(realText.Length);
            realText += added;
        }
        // 3) 길이가 같은데 편집된 경우(가운데 수정 등) → 가장 단순하게 마지막 글자만 신뢰
        else if (displayed.Length > 0)
        {
            // 마지막 문자만 갱신했다고 가정(사용자 편집을 최소 가정)
            realText = realText.Substring(0, realText.Length - 1) + displayed[^1];
        }

        // 표시 문자열 만들기: 전체 마스킹 + 마지막 1글자만 평문
        string toShow = BuildMaskedWithLastVisible();

        // UI에 반영 (콜백 루프 방지)
        suppressCallback = true;
        input.SetTextWithoutNotify(toShow);
        input.caretPosition = toShow.Length;
        suppressCallback = false;

        // 이전 코루틴이 있으면 중단하고 새로 시작
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        if (realText.Length > 0)
            hideCoroutine = StartCoroutine(HideLastCharAfterDelay());
    }

    /// <summary>revealDuration 경과 후 마지막 글자도 마스킹</summary>
    private IEnumerator HideLastCharAfterDelay()
    {
        yield return new WaitForSeconds(revealDuration);
        ForceFullMask();
    }

    /// <summary>모든 글자를 마스킹(마지막 글자 포함)</summary>
    private void ForceFullMask()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        string toShow = new string(maskChar, realText.Length);

        suppressCallback = true;
        input.SetTextWithoutNotify(toShow);
        input.caretPosition = toShow.Length;
        suppressCallback = false;
    }

    /// <summary>마스킹 문자열(마지막 글자만 평문) 생성</summary>
    private string BuildMaskedWithLastVisible()
    {
        int n = realText.Length;
        if (n == 0) return "";
        if (n == 1) return realText; // 글자 1개면 그대로 표시(잠시 후 전체 마스킹됨)
        return new string(maskChar, n - 1) + realText[n - 1];
    }

    // ===== 외부 연동용 유틸 =====

    /// <summary>실제 비밀번호 가져오기</summary>
    public string GetPassword() => realText;

    /// <summary>실제 비밀번호 설정(외부에서 불러올 때)</summary>
    public void SetPassword(string plain)
    {
        realText = plain ?? "";
        // 설정 직후에는 마지막 글자만 보여주고, 딜레이 후 마스킹
        string toShow = BuildMaskedWithLastVisible();
        suppressCallback = true;
        input.SetTextWithoutNotify(toShow);
        input.caretPosition = toShow.Length;
        suppressCallback = false;

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        if (realText.Length > 0)
            hideCoroutine = StartCoroutine(HideLastCharAfterDelay());
    }

    /// <summary>초기화</summary>
    public void Clear()
    {
        realText = "";
        if (hideCoroutine != null) { StopCoroutine(hideCoroutine); hideCoroutine = null; }
        suppressCallback = true;
        input.SetTextWithoutNotify("");
        input.caretPosition = 0;
        suppressCallback = false;
    }
}
