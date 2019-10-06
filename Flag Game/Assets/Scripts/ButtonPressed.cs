using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool IsPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
    }

    public bool IsButtonPressed()
    {
        return IsPressed;
    }

}
