using UnityEngine;

public class ClickableButton : MonoBehaviour
{
    public SpriteRenderer targetRenderer;
    public Sprite sprite1;
    public Sprite sprite2;

    private bool isOriginal = true;

    void OnMouseDown()
    {
        Debug.Log("버튼 클릭됨!"); // 👈 로그 찍어서 확인
        if (targetRenderer != null)
        {
            targetRenderer.sprite = isOriginal ? sprite2 : sprite1;
            isOriginal = !isOriginal;
        }
    }
}
