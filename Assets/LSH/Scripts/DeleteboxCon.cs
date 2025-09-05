using Unity.VisualScripting;
using UnityEngine;

public class DeleteboxCon : MonoBehaviour
{
    [SerializeField] RectTransform rectA;
    [SerializeField] RectTransform rectB;
    void Update()
    {
        DistCheck();
    }

    void DistCheck()
    {
        float dist = Vector2.Distance(rectA.position, rectB.position);
        Debug.Log($"Distance: {dist}");
        if (dist < 10f) // 임계값(겹침 판단 거리)
        {
            DotBoxGeneratorL.Instance.ReturnDot(rectA.gameObject); //비활성화
            DotBoxGeneratorR.Instance.ReturnDot(rectB.gameObject); //비활성화
            Debug.Log("히트박스 겹침 → 비활성화");
        }
    }

}
