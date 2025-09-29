using System;
using UnityEngine;

public class CircleHit : MonoBehaviour
{
	[SerializeField] Transform targetImage;
	[SerializeField] Transform hitRect;

	private void Update()
	{
		Debug.LogWarning(Vector2.Distance(targetImage.position, hitRect.position));
	}

	void CheckCol()
	{
		float circleDistance = Vector2.Distance(targetImage.position, hitRect.position);
		
		if(20 <circleDistance&&  circleDistance < 40)
		{
			
		}
	}
}
