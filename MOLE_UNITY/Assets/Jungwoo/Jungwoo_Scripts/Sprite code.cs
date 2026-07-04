using UnityEngine;
using UnityEngine.UI;

public class ToggleSprite : MonoBehaviour
{
    public GameObject targetObject;    // SpriteRenderer가 붙은 오브젝트 (화학식)
    public Sprite sprite1;             // 원래 스프라이트 (예: 화학식)
    public Sprite sprite2;             // 바꿀 스프라이트 (예: 화학식2)
    public Button toggleButton;        // 클릭할 버튼

    private bool isOriginal = true;    // 현재 어떤 스프라이트가 적용 중인지 저장

    void Start()
    {
        toggleButton.onClick.AddListener(ToggleSpriteImage);
    }

    public void ToggleSpriteImage()
    {
        if (targetObject != null && sprite1 != null && sprite2 != null)
        {
            SpriteRenderer renderer = targetObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = isOriginal ? sprite2 : sprite1;
                isOriginal = !isOriginal;  // 상태 반전
            }
        }
    }
}
