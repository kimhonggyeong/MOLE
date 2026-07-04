using UnityEngine;
using UnityEngine.UIElements;

public class ScrollHeightControll : MonoBehaviour
{
    public RectTransform scrollView;      // Scroll View의 RectTransform
    public RectTransform content;         // Content의 RectTransform
    public RectTransform canvasRect;      // Canvas 또는 부모 RectTransform

    [Range(0.1f, 1f)]
    public float canvasRatio = 0.8f;       // 전체 캔버스 높이 중 몇 %를 쓸지

    void Start()
    {
        AdjustHeight();
    }

    void AdjustHeight()
    {
        if (scrollView == null || content == null || canvasRect == null)
        {
            Debug.LogWarning("ScrollView / Content / CanvasRect 가 빠졌습니다.");
            return;
        }

        float canvasHeight = canvasRect.rect.height;
        float contentHeight = content.sizeDelta.y;

        // 캔버스 높이의 일부만 쓸 경우
        float maxVisibleHeight = canvasHeight * canvasRatio;

        // 콘텐츠보다 작으면 줄이고, 크면 콘텐츠 높이에 맞춤
        float targetHeight = Mathf.Min(contentHeight, maxVisibleHeight);

        scrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
    }
}
