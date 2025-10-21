using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LastBossMove : MonoBehaviour
{
    [Header("타겟 (플레이어)")]
    public Transform player;

    [Header("이동 속도")]
    public float moveSpeed = 3.8f;

    [Header("회전 속도")]
    public float rotationSpeed = 5f;

    public bool canRun = false;
    private void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (player != null && canRun)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f; // 수평 회전

        if (directionToPlayer != Vector3.zero)
        {
            // 플레이어를 바라보게 회전
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // LookRotation에서 계산한 방향 벡터의 반대로 이동
            Vector3 fleeDirection = -(targetRotation * Vector3.forward);
            transform.position += fleeDirection * moveSpeed * Time.deltaTime;
        }
    }


    public void HitGhostBoss()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f);
    }
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


