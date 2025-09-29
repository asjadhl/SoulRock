using UnityEngine;

public class CircleMove : MonoBehaviour
{
	[Header("스케일 이동 시간")]
	[SerializeField] float smallDuration = 5f;

	[Header("풀링용 설정")]
	[SerializeField] Vector3 targetScale = Vector3.zero;  
	public bool usePingPong = true;            

	private Vector3 startScale;
	private double startTime;

	void Start()
	{
		startTime=CheckRealTime.inGamerealTime;
		startScale = transform.localScale;
	}

	void Update()
	{
		double elapsed = CheckRealTime.inGamerealTime - startTime;
		float t = Mathf.Clamp01((float)(elapsed / smallDuration));

		if (usePingPong)
		{
			t = Mathf.PingPong(t, 1f);
		}

		transform.localScale = Vector3.Lerp(startScale, targetScale, t);

		if(t >= 1)
		{
			transform.localScale = startScale;
			startTime = CheckRealTime.inGamerealTime; 
		}
	}
}
