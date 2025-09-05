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
        

        // World Corners 가져오기
        Vector3[] cornersA = new Vector3[4];
        Vector3[] cornersB = new Vector3[4];
        rectA.GetWorldCorners(cornersA);
        rectB.GetWorldCorners(cornersB);

        // 중심 거리
        Vector3 centerA = (cornersA[0] + cornersA[2]) * 0.5f;
        Vector3 centerB = (cornersB[0] + cornersB[2]) * 0.5f;
        float dist = Vector3.Distance(centerA, centerB);
        Debug.Log(dist);
    }

}
