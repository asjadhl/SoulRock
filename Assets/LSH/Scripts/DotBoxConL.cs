using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DotBoxConL : MonoBehaviour
{
    [Header("도트 속도")]
    public float moveSpeed = 400f;
    RectTransform dotboxImage;
    DeleteboxCon deleteboxCon;
    RawImage rawImage;
    
    float fadeDuration = 0.9f; // 투명화까지 걸리는 시간
    Color originalColor;
    void Awake()
    {
        dotboxImage = GetComponent<RectTransform>();
        deleteboxCon = FindAnyObjectByType<DeleteboxCon>();
        rawImage = GetComponent<RawImage>();
        originalColor = rawImage.color;
    }
   
    void Update()
    {
        MoveToTarget();
        //실험용
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
    //public void ChangeColor()
    //{

    //    rawImage.color = new Color(1f, 1f, 1f, 0.5f);
    //}
    public async UniTask ChangeColor()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            Color newColor = originalColor;
            newColor.a = alpha;
            rawImage.color = newColor;

            await UniTask.Yield();
        }
    }
    public void SetColor()
    {
        rawImage.color = originalColor;
    }
}
