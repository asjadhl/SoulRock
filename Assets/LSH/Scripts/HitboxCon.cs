using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HitboxCon : MonoBehaviour
{
    void Update()
    {  
        DeleteCol(); 
    }
    

    void DeleteCol()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        // RectTransform 크기 BoxCollider2D에 같게
        collider.size = rectTransform.rect.size/10;
        collider.offset = rectTransform.rect.center;
    }

    
}
