using TMPro;
using UnityEngine;

public class RotateToggleButton : MonoBehaviour
{

    public bool isRotationOn = false;

    public void RotationToggleBtn()
    {
        isRotationOn = !isRotationOn;
        
    }
}
