using UnityEngine;

public class Move3D : MonoBehaviour
{
    private float rotationSpeed = 300f;
    public bool isDragging = false;
    private Vector3 lastMousePosition;

    private MainBtnController MBC; // 회전 토글 버튼 스크립트 참조

    private Quaternion initialRotation; // 초기 회전값 저장용

    void Start()
    {
        //초기 회전값 지정하고 회전여부 확인을 위한 코드 불러오기
        initialRotation = transform.rotation;

        GameObject obj = GameObject.Find("MainBtnController");
        if (obj != null)
        {
            MBC = obj.GetComponent<MainBtnController>();
        }
        else
        {
            Debug.LogWarning("RotateToggleButton 오브젝트를 찾을 수 없습니다!");
        }
    }
    void Update()
    {
        // 회전이 꺼져 있으면 회전 기능 실행 안 함
        if (MBC != null && !MBC.isRotationOn)
        {
            isDragging = false;
            transform.rotation = initialRotation;
            return;
        }

        //클릭시에만 드래그 true 손을 땔 시 false
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        //드래그 중이라면..
        if (isDragging) // 현재 pc기준으로만 만들었지만 모바일 작동은 된다.
        {
            float deltaX = (Input.mousePosition.x - lastMousePosition.x) / Screen.width;
            float deltaY = (Input.mousePosition.y - lastMousePosition.y) / Screen.height;

            float rotX = deltaY * rotationSpeed * 100f * Time.deltaTime;
            float rotY = -deltaX * rotationSpeed * 100f * Time.deltaTime;

            //y값은 X축을 통해 x값은 Y축을 통해 월드 좌표계를 기준으로 오브젝트를 회전시킨다.
            transform.Rotate(Vector3.right, rotX, Space.World);
            transform.Rotate(Vector3.up, rotY, Space.World);

            lastMousePosition = Input.mousePosition; //마지막 위치 업데이트
        }
    }

}
