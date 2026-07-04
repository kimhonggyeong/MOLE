using TMPro;
using UnityEngine;

public class MoleculeHeaderController : MonoBehaviour
{
    public TMP_Text headerText;   // 왼쪽 위 라벨(지금 Proline 나오는 곳)

    public void SetHeader(string name)
    {
        if (headerText != null) headerText.text = name;
    }
}


