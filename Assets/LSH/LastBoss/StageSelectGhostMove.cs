using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectGhostMove : MonoBehaviour
{
	public float moveSpeed = 4f;
	public Transform playerPos;
	public float chaseStartDistance = 10f;
	public float disableDistance = 1f;
	bool lastStageOn = false;
	void Awake()
    {
		if (playerPos == null)
			playerPos = GameObject.FindWithTag("Player").transform;
	}

	// Update is called once per frame
	void Update()
	{

		if (Input.GetKeyDown(KeyCode.R))
		{
			BossState.isBoss1Dead = true;
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			BossState.isBoss2Dead = true;
		}
		if (BossState.isBoss1Dead && BossState.isBoss2Dead)
		{
			GhostMove();
			if(!lastStageOn)
			{
				LastStage().Forget();
            }
		}
	}
    void GhostMove()
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

	
    private async UniTask LastStage()
	{
        lastStageOn = true;
		Debug.Log("Last Stage±îÁö 5ÃÊ");
        await UniTask.Delay(5000);
        SceneManager.LoadScene("LastStage");
    }
}
