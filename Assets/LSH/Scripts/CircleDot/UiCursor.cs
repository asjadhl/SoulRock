using UnityEngine;

public class UiCursor : MonoBehaviour
{
	[Header("커서 이미지 (UI)")]
	[SerializeField] private RectTransform cursorRect;
	[SerializeField] private Canvas canvas;

	[Header("클릭 시 크기")]
	[SerializeField] private Vector2 normalScale = Vector2.one * 1.8f;
	[SerializeField] private Vector2 clickScale = Vector2.one * 3f;

	[Header("크기 전환 속도")]
	[SerializeField] private float scaleSpeed = 10f;

	CircleHit circleHit;
	private Vector2 targetScale;

	void Start()
	{
		circleHit = GetComponent<CircleHit>();
		Cursor.visible = false; 
		targetScale = normalScale;
		cursorRect.localScale = normalScale;
	}

	void Update()
	{
		if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			cursorRect.position = Input.mousePosition;
		}
		if (circleHit.isScale)
			targetScale = clickScale;
		if (!circleHit.isScale)
			targetScale = normalScale;

		cursorRect.localScale = Vector2.Lerp(cursorRect.localScale, targetScale, Time.deltaTime * scaleSpeed);
	}

	public void ToggleScale()
	{
		targetScale = (targetScale == normalScale) ? clickScale : normalScale;
	}

	public void SetScale(Vector2 newScale)
	{
		targetScale = newScale;
	}
}
