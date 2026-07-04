using UnityEngine;
using UnityEngine.EventSystems;

public class HomePannelSlider : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public RectTransform panelRect; // 움직일 패널
    public float topY = 70f;       // 펼쳐졌을 때 Y (현재 오브젝트 넣은 위치가 70이니 시작위치)
    public float bottomY = -15f; // 접혔을 때 Y
    public float snapSpeed = 10f; // 스냅 애니메이션 속도
    public GameObject buttonPanel; //패널 속 버튼 패널

    private Vector2 startPos; // 드래그 시작 시 패널 위치 저장
    private bool isDragging = false;  // 현재 드래그 중인지 여부

    //Update 대신 위에 3개의 인터페이스를 붙여 EventSystem 오브젝트를 통해 드래그 인식
    public void OnBeginDrag(PointerEventData eventData) //드래그 시작 Unity 공식 함수
    {
        isDragging = true;
        startPos = panelRect.anchoredPosition; // 시작 위치 저장
    }

    public void OnDrag(PointerEventData eventData) //드래그 중
    {
        Vector2 dragDelta = eventData.delta; //드래그 방향과 거리
        Vector2 newPos = panelRect.anchoredPosition + new Vector2(0, dragDelta.y); //y축으로만 이동
        newPos.y = Mathf.Clamp(newPos.y, bottomY, topY); //위에 지정한 범위 제한
        panelRect.anchoredPosition = newPos; //새로운 위치 반영
    }

    public void OnEndDrag(PointerEventData eventData) //드래그 종료
    {
        isDragging = false;
        // 위에 지정한 topY + bottomY 의 중간값을 기준으로 위인지 아래인지 판별
        float middleY = (topY + bottomY) / 2f;
        //중간 값보다 충분히 위라면 위로 아래면 아래로 위치 지정
        float targetY = panelRect.anchoredPosition.y > middleY ? topY : bottomY;
        StartCoroutine(SmoothSnap(targetY));//IEnumerator 코루틴으로 실행
    }

    System.Collections.IEnumerator SmoothSnap(float targetY)
    {
        //타겟 위치와의 거리까지 1f미만이 될때까지 반복
        while (Mathf.Abs(panelRect.anchoredPosition.y - targetY) > 1f)
        {
            //Lerp를 이용해 해당 위치까지 부드럽게 이동
            float newY = Mathf.Lerp(panelRect.anchoredPosition.y, targetY, Time.deltaTime * snapSpeed);
            panelRect.anchoredPosition = new Vector2(panelRect.anchoredPosition.x, newY); //계산 적용
            yield return null; //다음 프레임 대기
        }
        //반복이 끝난 뒤 정확하게 목표 위치에 고정시킴
        panelRect.anchoredPosition = new Vector2(panelRect.anchoredPosition.x, targetY);

    }
}
