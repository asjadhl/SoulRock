 
using UnityEngine;
using UnityEngine.EventSystems;
 

public class ScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IPointerExitHandler
{

  
    public bool IsDown = false;
 

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDown = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    { IsDown = false; }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsDown = false;
    }
}
