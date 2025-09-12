using UnityEngine;
using UnityEngine.UI;

public class DotBoxConL : MonoBehaviour
{
    [Header("도트 속도")]
    public float moveSpeed = 400f;
    RectTransform dotboxImage;
    DeleteboxCon deleteboxCon;
    RawImage rawImage;
    bool test = false;
    void Start()
    {
        dotboxImage = GetComponent<RectTransform>();
        deleteboxCon = FindAnyObjectByType<DeleteboxCon>();
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        MoveToTarget();
        //실험용
        if (Input.GetKeyDown(KeyCode.W))
        {
            test = true; Debug.Log("W누름 test True");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            test = false; Debug.LogError("S누름 test false");
        }
        OnInvisible();
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

    public void OnInvisible()
    {
        if(test)
        rawImage.color = new Color(255f, 255f, 255f, 0f);
    }
}
