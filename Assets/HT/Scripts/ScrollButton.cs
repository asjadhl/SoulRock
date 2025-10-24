 
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public bool IsDown = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDown = true;
        Debug.Log("A");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsDown = false;
    }
}
