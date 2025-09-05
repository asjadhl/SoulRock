using UnityEngine;

public class DotBoxConB : MonoBehaviour
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
        }
    }
    void MoveToDotbox()
    {
        dotboxImage.anchoredPosition += new Vector2(moveSpeed * Time.deltaTime, 0);
    }
    void DeleteDotbox()
    {
        DotBoxGeneratorL.Instance.ReturnDot(this.gameObject);
    }
}
