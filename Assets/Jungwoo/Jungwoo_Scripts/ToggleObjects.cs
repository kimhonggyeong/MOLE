using UnityEngine;

public class ToggleObjects : MonoBehaviour
{
    public string targetTag = "3D_Model";        // 비활성화할 태그
    public GameObject objectToActivate;       // 활성화할 오브젝트
    public RotateToggleButton RTB;

    private Vector3 originalScale;

    void Start()
    {
        if (objectToActivate != null)
        {
            originalScale = objectToActivate.transform.localScale; // 처음 스케일 저장
        }

        if (RTB == null) // 수동 연결이 안 되어 있으면
        {
            GameObject obj = GameObject.Find("RotateToggleButton"); // 오브젝트 이름으로 찾기
            if (obj != null)
            {
                RTB = obj.GetComponent<RotateToggleButton>();
            }
            else
            {
                Debug.LogWarning("RotateToggleButton 오브젝트를 찾을 수 없습니다!");
            }
        }
    }

    public void Alanine_ModelBtn()
    {
        RTB.isRotationOn = false;

        // 태그가 targetTag인 모든 오브젝트 비활성화
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in taggedObjects)
        {
            obj.SetActive(false);
        }

        // 특정 오브젝트 활성화
        if (objectToActivate != null)
        {
            objectToActivate.transform.localScale = originalScale;
            objectToActivate.SetActive(true);

        }
    }
}
