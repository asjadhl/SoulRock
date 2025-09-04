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
        dotboxImage = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dotboxImage != null)
        {
            MoveToDotbox();
            MakeACol();
        }
    }
    void MoveToDotbox()
    {
        dotboxImage.anchoredPosition += new Vector2(-moveSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Hitbox"))
        {
            DotBoxGenerator.Instance.ReturnDot(this.gameObject); //도트박스 비활성화
        }
    }
    void MakeACol()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        // RectTransform 크기 BoxCollider2D에 같게
        collider.size = rectTransform.rect.size/10;
        collider.offset = rectTransform.rect.center;//중심임.
    }
}
