using Cysharp.Threading.Tasks;
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
    public Image rawImage;
    Color originalColor;
    [Tooltip("투명화 걸리는시간")]
    public float fadeDuration = 0.1f;
    private bool colorChanging = false;
    public Color cirlceColor;
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

        if (transform.localScale.x <= 10 && !colorChanging)
        {
            colorChanging = true;
            ChangeColorSmooth(cirlceColor, 0.5f).Forget();
        }
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

    public async UniTask ChangeColorSmooth(Color targetColor, float duration)
    {
        Color startColor = rawImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rawImage.color = Color.Lerp(startColor, targetColor, elapsed / duration);
            await UniTask.Yield();
        }
        rawImage.color = targetColor;
        colorChanging = false;
    }

    public void SetColor()
    {
        rawImage.color = originalColor;
    }
}
