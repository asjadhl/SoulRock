using System.Transactions;
using UnityEngine;

public class CircleMove : MonoBehaviour
{
	public Transform hitRect;
	[SerializeField] float smallDuration = 5f;
	[SerializeField] Vector3 targetScale = Vector3.zero;

	public Vector3 startScale;
	private double startTime;
	private bool returned = false;
	private CircleHit circleHit;

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
	}
}
