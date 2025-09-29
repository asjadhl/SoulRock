 using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeleteboxCon : MonoBehaviour
{
    [Header("충돌 거리")]
    public float hitThreshold = 5f;

    [Header("중앙 목표")]
    public RectTransform targetImage;

    [Header("좌/우 도트 부모")]
    public Transform leftParent;
    public Transform rightParent;
    [SerializeField] HitBoxCon hitBoxCon;
    private void Update()
    {
        if (targetImage == null) return;
        CheckCollision();
    }

    private void CheckCollision()
    {
        RectTransform[] leftDots = leftParent.GetComponentsInChildren<RectTransform>();
        RectTransform[] rightDots = rightParent.GetComponentsInChildren<RectTransform>();
        Vector2 targetPos = targetImage.position;

        for (int i = leftDots.Length - 1; i >= 0; i--)
        {
            RectTransform left = leftDots[i];
            if (!left.gameObject.activeSelf) continue;

            for (int j = rightDots.Length - 1; j >= 0; j--)
            {
                RectTransform right = rightDots[j];
                if (!right.gameObject.activeSelf) continue;

                float distLeftRight = Vector2.Distance(left.position, right.position);
                float distLeftTarget = Vector2.Distance(left.position, targetPos);
                float distRightTarget = Vector2.Distance(right.position, targetPos);

                if (distLeftRight <= hitThreshold &&
                    distLeftTarget <= hitThreshold &&
                    distRightTarget <= hitThreshold)
                {
                    DotBoxGeneratorL.Instance.ReturnDot(left.gameObject);
                    DotBoxGeneratorR.Instance.ReturnDot(right.gameObject);

                    //Debug.Log("좌/우 도트 충돌 + 중앙 목표 근접 → 비활성화");
                    hitBoxCon.combo = 0;
                    if(hitBoxCon.combo == 0)
                    {
                        for(int k = 0; k < hitBoxCon.comboImage.Length; k++)
                        {
                            hitBoxCon.comboImage[k].SetActive(false);
                        }
                    }
                    break;
                }
            }
        }
    }

}
