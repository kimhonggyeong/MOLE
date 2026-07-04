using UnityEngine;
using UnityEngine.EventSystems;

public class BondClickHandler : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    private EraserManager eraserManager;

    void Start()
    {
        GameObject EraserObj = GameObject.Find("EraserManager");

        if (EraserObj != null)
        {
            eraserManager = EraserObj.GetComponent<EraserManager>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eraserManager != null && eraserManager.isEraseMode)
        {
            eraserManager.EraseBondByGameObject(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eraserManager != null && eraserManager.isEraseMode && Input.GetMouseButton(0))
        {
            eraserManager.EraseBondByGameObject(gameObject);
        }
    }
}
