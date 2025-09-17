using System.Collections.Generic;
using UnityEngine;

public class CorridorSpawner : MonoBehaviour
{
    public string[] corridorTag = { "Corridor", "Trap1" }; // PoolingManager 태그
    public int corridorCount = 5;           // 유지할 복도 개수
    public float corridorLength = 61f;      // 복도 길이(Z축)
    public float corridorWidth = 10f;       // 복도 폭(X축)

    DoorTrap doorTrap;

    [Header("Player Reference")]
    public Transform player; // 플레이어 (고정)

    [Header("Moster Spawner prefab")]
    public string mosterSpawnerTag = "Spawner";

    private Queue<GameObject> corridors = new Queue<GameObject>();
    string GetRandomCorridorTag()
    {
        int rand = Random.Range(0, 100);
        if (rand < 10) return "Trap1";   // 10%
        return "Corridor";               // 90%
    }
    void Start()
    {
        // 초기 복도 배치
        float startZ = 0f;
        for (int i = 0; i < corridorCount; i++)
        {
            string tag = GetRandomCorridorTag();

            GameObject corridor = PoolingManager.Instance.SpawnFromPool(
                tag,
                new Vector3(corridorWidth / 2f, 0, startZ),
                Quaternion.identity
            );

            corridors.Enqueue(corridor);
            startZ += corridorLength;
        }


        doorTrap = GameObject.FindObjectOfType<DoorTrap>();
    }

    void Update()
    {
        ManageCorridors();
  
    }



    // 플레이어 뒤로 넘어간 복도는 재사용
    void ManageCorridors()
    {
        GameObject first = corridors.Peek();

        if (first.transform.position.z < player.position.z - corridorLength)
        {
            GameObject old = corridors.Dequeue();
            old.SetActive(false);

            GameObject last = null;
            foreach (var c in corridors) last = c;

            Vector3 newPos = last.transform.position + new Vector3(0, 0, corridorLength);
            string tag = GetRandomCorridorTag();

            GameObject newCorridor = PoolingManager.Instance.SpawnFromPool(
                tag,
                newPos,
                Quaternion.identity
            );

            GameObject mosterSpawner = PoolingManager.Instance.SpawnFromPool(
                mosterSpawnerTag,
                newPos,
                Quaternion.identity
            );

            corridors.Enqueue(newCorridor);
        }
    }

}
