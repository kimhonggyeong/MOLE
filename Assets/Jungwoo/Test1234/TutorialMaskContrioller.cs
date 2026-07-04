using UnityEngine;
using UnityEngine.UI;

public class TutorialMaskController : MonoBehaviour
{
    public Material overlayMaterial;         // TutorialOverlay_Material ����
    public GameObject targetObject;          
    public float holeRadius = 0.2f;         

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        image.material = overlayMaterial;    

    
        UpdateHoleToTarget(targetObject);
    }

    public void UpdateHoleToTarget(GameObject target)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        Vector2 uv = new Vector2(
            screenPos.x / Screen.width,
            screenPos.y / Screen.height
        );

        overlayMaterial.SetVector("_HoleCenter", new Vector4(uv.x, uv.y, 0, 0));
        overlayMaterial.SetFloat("_HoleRadius", holeRadius);
    }

}

