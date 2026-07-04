using UnityEngine;

public class UIResolutionScaler : MonoBehaviour
{
    public Camera targetCamera;
    public float referenceOrthoSize = 5f;   // 기준 Orthographic Size
    public Vector3 baseScale = Vector3.one; // 원래 오브젝트 크기

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        float scaleFactor = referenceOrthoSize / targetCamera.orthographicSize;
        transform.localScale = baseScale * scaleFactor;
    }
}
