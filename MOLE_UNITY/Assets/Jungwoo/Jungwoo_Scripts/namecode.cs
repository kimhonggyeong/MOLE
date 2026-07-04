using UnityEngine;

public class NicknameCheckUI : MonoBehaviour
{
    public GameObject availableNicknameText;      // 사용가능닉네임
    public GameObject unavailableNicknameText;    // 사용불가능닉네임

    // 중복확인 버튼 클릭 시 호출
    public void OnCheckDuplicateClicked()
    {
        availableNicknameText.SetActive(true);
        unavailableNicknameText.SetActive(false);
    }

    // 변경 버튼 클릭 시 호출
    public void OnChangeNicknameClicked()
    {
        availableNicknameText.SetActive(false);
        unavailableNicknameText.SetActive(true);
    }
}
