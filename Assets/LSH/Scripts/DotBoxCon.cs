using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DotBoxCon : MonoBehaviour
{
    
    [Header("도트속도")]
    public float moveSpeed = 100f;
    RectTransform dotboxImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (dotboxImage != null)
        {
            MoveToDotbox();
        }
    }
    void MoveToDotbox()
    {
        dotboxImage.anchoredPosition += new Vector2(-moveSpeed * Time.deltaTime, 0);
    }

}
