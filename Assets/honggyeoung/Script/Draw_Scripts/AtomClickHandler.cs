using UnityEngine;
using UnityEngine.EventSystems;

public class AtomClickHandler : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    private BondDrawer bondDrawer;
    private EraserManager eraserManager;

    void Start()
    {
        GameObject bondDrawerObj = GameObject.Find("BondDrawer");
        GameObject EraserObj = GameObject.Find("EraserManager");

        if (bondDrawerObj != null)
        {
            bondDrawer = bondDrawerObj.GetComponent<BondDrawer>();
        }
        if (EraserObj != null)
        {
            eraserManager = EraserObj.GetComponent<EraserManager>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (bondDrawer != null && bondDrawer.isDrawingBond && bondDrawer.startAtom == null)
        {
            bondDrawer.startAtom = gameObject;
            Debug.Log("Start Atom 지정됨: " + gameObject.name);
        }
        if (eraserManager != null && eraserManager.isEraseMode)
        {
            eraserManager.EraseAtomByGameObject(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eraserManager != null && eraserManager.isEraseMode && Input.GetMouseButton(0))
        {
            eraserManager.EraseAtomByGameObject(gameObject);
        }
    }
}
