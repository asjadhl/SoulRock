using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class LastBossMove : MonoBehaviour
{
    public float moveSpeed = 4f;
    public bool canRun = false;
    BossMove temp;
    //[Header("이동 속도")]
    //public float moveSpeed = 3.8f;

    //   [Header("회전 속도")]
    //   public float rotationSpeed = 5f;


    //public bool canRun = false;
    //bool isChasing = false;

    private void Start()
    {
        temp = GameObject.FindAnyObjectByType<BossMove>();
    }
    private void FixedUpdate()
    {
        if (temp.canRun)
            UpdateBossRun();
    }

    //private void UpdateBossRun()
    //{
    //	transform.position += moveSpeed * Time.fixedDeltaTime * -transform.forward;
    //}

    public void HitGhostBoss()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 6f);
	}
    private void UpdateBossRun()
    {
        transform.position += moveSpeed * Time.fixedDeltaTime * -transform.forward;
    }

	private void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag("Player"))
		{
			GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPBigMinus().Forget();
			gameObject.SetActive(false);
		}
	}

	//private void ChasePlayer()
	//{
	//	float distance = Vector3.Distance(transform.position, player.position);

	//	if (distance > chaseStartDistance)
	//	{
	//		isChasing = false;
	//		return;
	//}
	//	isChasing = true;

	//	Vector3 dirToPlayer = player.position - transform.position;
	//	dirToPlayer.y = 0f;

	//	if (dirToPlayer != Vector3.zero)
	//	{
	//		Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
	//		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
	//	}

	//	if (distance > disableDistance)
	//	{
	//		transform.position += dirToPlayer.normalized * moveSpeed * Time.deltaTime;
	//	}
	//	else
	//	{
	//		// 근접 시 멈춤 또는 공격 등
	//		isChasing = false;
	//	}
	//}

	//private void ChasePlayer()
	//   {
	//       Vector3 directionToPlayer = player.position - transform.position;
	//       directionToPlayer.y = 0f; // 수평 회전

	//       if (directionToPlayer != Vector3.zero)
	//       {
	//           // 플레이어를 바라보게 회전
	//           Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
	//           transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

	//           // LookRotation에서 계산한 방향 벡터의 반대로 이동
	//           Vector3 fleeDirection = -(targetRotation * Vector3.forward);
	//           transform.position += fleeDirection * moveSpeed * Time.deltaTime;
	//       }
	//   }

	//private void bossMove()
	//{
	//    Vector3 directionToPlayer = player.position - transform.position;
	//    directionToPlayer.y = 0f;

	//    if (directionToPlayer != Vector3.zero)
	//    {
	//        // 회전
	//        transform.rotation = Quaternion.LookRotation(directionToPlayer);

	//        // 플레이어 방향으로 이동
	//        transform.position += directionToPlayer.normalized * moveSpeed * Time.deltaTime;
	//    }
	//}
}


