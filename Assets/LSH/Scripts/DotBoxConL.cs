using UnityEngine;

public class DotBoxConL : MonoBehaviour
{
    [Header("도트 속도")]
    public float moveSpeed = 400f;
    RectTransform dotboxImage;
    DeleteboxCon deleteboxCon;

    void Start()
    {
        dotboxImage = GetComponent<RectTransform>();
        deleteboxCon = FindAnyObjectByType<DeleteboxCon>();
    }

    void Update()
    {
        MoveToTarget();
    }

    void MoveToTarget()
    {
        if (dotboxImage == null || deleteboxCon.targetImage == null) return;

        Vector3 targetPos = deleteboxCon.targetImage.position; // world 기준
        dotboxImage.position = Vector3.MoveTowards(
            dotboxImage.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }
}
