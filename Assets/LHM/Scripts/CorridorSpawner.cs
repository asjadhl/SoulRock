using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class StageInfo
{
    public string normalTag;
    public List<string> trapTags;
    [Range(0f, 1f)] public float trapChance;
    public float corridorLength = 88.5f; // 각 스테이지 길이
}

public class CorridorSpawner : MonoBehaviour
{
    [Header("Stage 설정")]
    public int currentStage = 1;           // 현재 스테이지 번호 (1부터 시작)
    public List<StageInfo> stages;         // Stage별 정보 (인스펙터에서 설정)

    private int oldstage = 1;
    private int oldstage2 = 2;
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

            // 마지막 복도의 실제 길이를 자동 계산
            float lastLength = GetPrefabLength(last);

            // 새 복도 위치 = 마지막 복도 끝점
            Vector3 newPos = last.transform.position + new Vector3(0, 0, lastLength);

            // 스테이지 변경 체크
            if (oldstage != currentStage)
                oldstage = currentStage;

            // 스테이지별 태그 가져오기
            string tag = GetStageCorridorTag();

            GameObject newCorridor = PoolingManager.Instance.SpawnFromPool(
                tag,
                newPos,
                Quaternion.identity
            );

            corridors.Enqueue(newCorridor);
        }
    }

    // 프리팹의 실제 길이 계산
    float GetPrefabLength(GameObject obj)
    {
        Renderer rend = obj.GetComponentInChildren<Renderer>();
        if (rend != null)
            return rend.bounds.size.z;

        return corridorLength; // 기본값 fallback
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
