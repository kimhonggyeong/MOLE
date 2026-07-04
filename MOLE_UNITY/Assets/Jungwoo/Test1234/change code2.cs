using TMPro;
using UnityEngine;

public class MoleculeRow : MonoBehaviour
{
    public MoleculeHeaderController header;   // A에서 만든 컨트롤러 넣기
    public TMP_Text rowText;                  // 이 버튼에 보이는 텍스트

    public void OnClick()
    {
        if (header != null && rowText != null)
            header.SetHeader(rowText.text);
    }
}

