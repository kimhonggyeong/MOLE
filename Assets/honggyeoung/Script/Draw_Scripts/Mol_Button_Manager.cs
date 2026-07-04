using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mol_Button_Manager : MonoBehaviour
{
    public GameObject URLInputPage;
    public TMP_InputField inputField; // 인스펙터에 연결
    public MoleculeSender moleculeSender; // 값을 넘겨줄 대상 컴포넌트
    public GameObject Buttons;
    public GameObject Mol_Texts; 
    public GameObject BackDrawButton;

    public Sprite BlueSprite;
    public Sprite YelloSprite;

    public GameObject buttonObj;
    public Image buttonImg;

    public void InputUrl()
    {
        URLInputPage.SetActive(true);
    }

    public void ApplyUrl()
    {
/*        string inputUrl = inputField.text;

        moleculeSender.serverUrl = inputUrl;
        URLInputPage.SetActive(false);*/
    }

    public void BackDrawPanel()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("DrawModel");
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
        Buttons.SetActive(true);
        BackDrawButton.SetActive(false);
        Mol_Texts.SetActive(false);
    }

    public void clearBtnColor()
    {
        // 이전 버튼을 파란색으로 되돌림
        if (buttonImg != null)
        {
            buttonImg.sprite = BlueSprite;
            buttonImg = null;
        }
        buttonObj = null;
    }

    public void changeBtnColor(GameObject clickedButton)
    {
        // 이미 같은 버튼이면 → 선택 해제
        if (buttonObj == clickedButton)
        {
            clearBtnColor();
            return;
        }

        // 다른 버튼 클릭 시 이전 것 초기화
        clearBtnColor();

        // 새 버튼 선택
        buttonObj = clickedButton;
        buttonImg = buttonObj.GetComponent<Image>();

        if (buttonImg != null)
        {
            buttonImg.sprite = YelloSprite;
        }
    }
}
