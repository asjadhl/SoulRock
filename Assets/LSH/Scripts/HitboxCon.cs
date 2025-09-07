using Unity.VisualScripting;
using UnityEngine;

//일단 해야할 것.
//1. 거리로 제거랑 클릭판정만들기.
public class HitBoxCon: MonoBehaviour
{
    [Header("클릭 가능 거리 범위")]
    public float minClick = 100f;
    public float maxClick = 200f;

    [Header("좌/우 도트 부모")]
    public Transform leftDotBox;
    public Transform rightDotBox;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            TryClick();
        }
    }

    private void TryClick() //거리 구해서 
    {
        RectTransform[] leftDots = leftDotBox.GetComponentsInChildren<RectTransform>();
        RectTransform[] rightDots = rightDotBox.GetComponentsInChildren<RectTransform>();

        for (int i = leftDots.Length - 1; i >= 0; i--)
        {
            RectTransform left = leftDots[i];
            if (!left.gameObject.activeSelf) continue;

            for (int j = rightDots.Length - 1; j >= 0; j--)
            {
                RectTransform right = rightDots[j];
                if (!right.gameObject.activeSelf) continue;

                float distLeftRight = Vector2.Distance(left.position, right.position);

                if (distLeftRight >= minClick && distLeftRight <= maxClick)
                {
                    OnClickSuccess();
                    DotBoxGeneratorL.Instance.ReturnDot(left.gameObject);
                    DotBoxGeneratorR.Instance.ReturnDot(right.gameObject);
                    break;
                }
                else
                {
                    Debug.Log($"클릭 실패. 현재 거리: {distLeftRight:F1}");
                }
            }
        }
    }

    private void OnClickSuccess()
    {
        
        Debug.Log("클릭 성공!");
        // 클릭 성공 시 처리할 로직
        // 예: 좌/우 도트 비활성화, 점수 증가 등
    }
}
