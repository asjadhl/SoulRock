using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageSelectGhostMove : MonoBehaviour
{
	public float moveSpeed = 4f;
	public Transform playerPos;
	public float chaseStartDistance = 10f;
	public float disableDistance = 1f; 
											
	void Awake()
    {
		if (playerPos == null)
			playerPos = GameObject.FindWithTag("Player").transform;
	}

    // Update is called once per frame
    void Update()
    {
		 SkullMove();
	}
	void SkullMove()
	{
		float distance = Vector3.Distance(transform.position, playerPos.position);
		if (distance > chaseStartDistance)
		{
			transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);
			return;
		}

		//if (distance <= chaseStartDistance && distance > disableDistance)
		//{
		//	Vector3 targetPos = new Vector3(
		//		playerPos.position.x,
		//		playerPos.position.y,
		//		playerPos.position.z
		//	);

		//	Vector3 direction = (targetPos - transform.position).normalized;

		//	transform.position += direction * moveSpeed * Time.deltaTime;

		//	return;
		//}

		if (distance <= disableDistance)
		{
			transform.SetParent(playerPos.transform, true);
		}
	}
}
