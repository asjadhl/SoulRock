using Cysharp.Threading.Tasks;
using System.Security.Cryptography;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class CircleMove : MonoBehaviour
{
	public Transform hitRect;
	[SerializeField] float smallDuration = 5f;
	[SerializeField] Vector3 targetScale = Vector3.zero;

	public Vector3 startScale;
	private double startTime;
	private bool returned = false;
	private CircleHit circleHit;

    Image rawImage;
    Color originalColor;
    [Tooltip("투명화 걸리는시간")]
    public float fadeDuration = 0.1f;

    private void Awake()
    { 
		rawImage = GetComponent<Image>();
        originalColor = rawImage.color;
    }
    public void Initialize(CircleHit hit)
	{
		circleHit = hit;
		startTime = CheckRealTime.inGamerealTime;
		startScale = Vector3.one * hit.circleBig;
		returned = false;
		transform.localScale = startScale;
	}
	void Update()
	{
		float t = Mathf.Clamp01((float)((CheckRealTime.inGamerealTime - startTime) / smallDuration));
		transform.localScale = Vector3.Lerp(startScale, targetScale, t);

        if (t >= 1f && !returned)
        {
            circleHit.combo = 0;
            returned = true;
            circleHit.ReturnCircle(this.gameObject);
        }

        if (transform.localScale.x <= 3)
            ChangeColor_();
        
    }
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

    public void ChangeColor_()
    {
        rawImage.color = Color.cyan;
    }

    public void SetColor()
    {
        rawImage.color = originalColor;
    }
}
