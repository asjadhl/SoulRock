using System.Collections.Generic;
using UnityEngine;

public class CorridorSpawner : MonoBehaviour
{
    [Header("Corridor Settings")]
    public string corridorTag = "Corridor"; // PoolingManager 태그
    public int corridorCount = 3;           // 유지할 복도 개수
    public float corridorLength = 62f;      // 복도 길이(Z축)
    public float corridorWidth = 10f;       // 복도 폭(X축)
    public float moveSpeed = 10f;           // 맵 이동 속도

    [Header("Player Reference")]
    public Transform player; // 플레이어 (고정)

    private Queue<GameObject> corridors = new Queue<GameObject>();

    void Start()
    {
        // 초기 복도 배치
        float startZ = 0f;
        for (int i = 0; i < corridorCount; i++)
        {
            GameObject corridor = PoolingManager.Instance.SpawnFromPool(
                corridorTag,
                new Vector3(corridorWidth / 2f, 0, startZ),
                Quaternion.identity
            );

            corridors.Enqueue(corridor);
            startZ += corridorLength;
        }
    }

    void Update()
    {
        MoveCorridors();
        ManageCorridors();
    }

    // 복도들을 뒤로 이동
    void MoveCorridors()
    {
        foreach (var corridor in corridors)
        {
            corridor.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
    }

    // 플레이어 뒤로 넘어간 복도는 재사용
    void ManageCorridors()
    {
        GameObject first = corridors.Peek();

        // 복도의 끝이 플레이어 기준 충분히 뒤로 갔으면 재배치
        if (first.transform.position.z < player.position.z - corridorLength)
        {
            GameObject old = corridors.Dequeue();
            old.SetActive(false);

            // 마지막 복도 위치 기준으로 새 복도 붙이기
            GameObject last = null;
            foreach (var c in corridors) last = c; // 마지막 큐 아이템 찾기

            Vector3 newPos = last.transform.position + new Vector3(0, 0, corridorLength);
            GameObject newCorridor = PoolingManager.Instance.SpawnFromPool(
                corridorTag,
                newPos,
                Quaternion.identity
            );

            corridors.Enqueue(newCorridor);
        }
    }
}
