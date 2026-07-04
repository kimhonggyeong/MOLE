using UnityEngine;

public class Draw_Move3D : MonoBehaviour
{
    private float rotationSpeed = 300f;
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            float deltaX = (Input.mousePosition.x - lastMousePosition.x) / Screen.width;
            float deltaY = (Input.mousePosition.y - lastMousePosition.y) / Screen.height;

            float rotX = deltaY * rotationSpeed * 100f * Time.deltaTime;
            float rotY = -deltaX * rotationSpeed * 100f * Time.deltaTime;

            transform.Rotate(Vector3.right, rotX, Space.World);
            transform.Rotate(Vector3.up, rotY, Space.World);

            lastMousePosition = Input.mousePosition;
        }
    }
}
