using Unity.VisualScripting;
using UnityEngine;

public class HitboxCon : MonoBehaviour
{
    [Header("도트박스")]
    [SerializeField] RectTransform dotboxImage;
    [Header("히트박스")]
    [SerializeField] RectTransform hitboxImage;
    [Header("판정속도")]
    public float moveSpeed = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    void Update()
    {
        if (dotboxImage != null)
        {
            MoveToHitbox();
            CheckColBox();
        }
        
    }
    void MoveToHitbox()
    {
        dotboxImage.anchoredPosition += new Vector2(-moveSpeed *Time.deltaTime, 0);
    }



    void CheckColBox()
    {
        if (IsOverlapping(dotboxImage, hitboxImage))
        {
            Debug.Log("도트박스 삭제");
            Destroy(dotboxImage.gameObject);
        }
    }

    //곂치는지 확인
    bool IsOverlapping(RectTransform a, RectTransform b)
    {
        Vector3[] aCorner = new Vector3[4]; 
        Vector3[] bCorner = new Vector3[4];

        a.GetWorldCorners(aCorner); //화면 상 위치를 알기위해 월드 좌표 얻는거
        b.GetWorldCorners(bCorner); 

        Rect aRect = new Rect(aCorner[0], aCorner[2] - aCorner[0]); //new Rect(position, size)
        Rect bRect = new Rect(bCorner[0], bCorner[2] - bCorner[0]);

        return aRect.Overlaps(bRect); //곂치면 True
    }
}
