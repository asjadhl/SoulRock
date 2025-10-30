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
		if (cursorRect == null)
			Debug.LogError("Cursor RectTransform이 할당되지 않음!");
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
		//else // Screen Space - Camera
		//{
		//	Vector2 pos;
		//	RectTransformUtility.ScreenPointToLocalPointInRectangle(
		//		canvas.transform as RectTransform,
		//		Input.mousePosition,
		//		canvas.worldCamera,
		//		out pos
		//	);
		//	cursorRect.localPosition = pos;
		//}
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
	// 클릭 이벤트와 연동
	//UICursor uiCursor = FindObjectOfType<UICursor>();
	//uiCursor.SetScale(Vector2.one* 2f);
}
