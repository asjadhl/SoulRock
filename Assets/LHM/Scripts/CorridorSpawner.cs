using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public string normalTag;        // 일반 맵 태그
    public List<string> trapTags;   // 트랩 맵 태그들
    [Range(0f, 1f)]
    public float trapChance;        // 트랩 나올 확률 (0~1)
}

public class CorridorSpawner : MonoBehaviour
{
    [Header("Stage 설정")]
    public int currentStage = 1;           // 현재 스테이지 번호 (1부터 시작)
    public List<StageInfo> stages;         // Stage별 정보 (인스펙터에서 설정)

    private int oldstage = 1;
    private string oldTag;
    [Header("Corridor 설정")]
    public int corridorCount = 5;          // 유지할 복도 개수
    public float corridorLength = 88.5f;     // 복도 길이(Z축)
    public float corridorWidth = 60f;      // 복도 폭(X축)

    [Header("Player Reference")]
    public Transform player;               // 플레이어 (고정)

    [Header("Monster Spawner prefab")]
    public string mosterSpawnerTag = "Spawner";

    private Queue<GameObject> corridors = new Queue<GameObject>();
    BossSpawner bossSpawner;

    void Start()
    {
        float playerX = player.position.x +5;
        // 초기 복도 배치
        float startZ = 0f;
        float newstartz = -corridorLength;
        for (int i = 0; i < corridorCount; i++)
        {
            string tag = GetStageCorridorTag();

            GameObject corridor = PoolingManager.Instance.SpawnFromPool(
                tag,
                new Vector3(playerX, 0, startZ),
                Quaternion.identity
            );


            corridors.Enqueue(corridor);
            startZ += corridorLength;
        }
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
            string tag = GetStageCorridorTag();

            Vector3 newPos1 = last.transform.position + new Vector3(0, 0, corridorLength - 88.5f);
            //스테이지 변경시
            if (oldstage == 1 && currentStage == 2)
            {
                newPos = last.transform.position + new Vector3(0, 0, corridorLength - 88.5f);
        
                 GameObject newCorridor1 = PoolingManager.Instance.SpawnFromPool(
                    tag,
                    newPos,
                    Quaternion.identity
                );
                oldstage = currentStage;
                corridors.Enqueue(newCorridor1);
            }
            
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

    // 현재 스테이지에 맞는 프리펩 태그 선택
    string GetStageCorridorTag()
    {
        if (currentStage < 1 || currentStage > stages.Count)
            return "Corridor"; // 예외 시 기본값

        StageInfo stage = stages[currentStage - 1];

        // 확률 체크
        if (Random.value < stage.trapChance && stage.trapTags.Count > 0)
        {
            // 트랩맵 중 하나 랜덤 선택
            int randIndex = Random.Range(0, stage.trapTags.Count);
            return stage.trapTags[randIndex];
        }
        else
        {
            return stage.normalTag;
        }
    }
}
