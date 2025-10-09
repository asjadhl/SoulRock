using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class StageInfo
{
    public string normalTag;
    public List<string> trapTags;
    [Range(0f, 1f)] public float trapChance;
    public float corridorLength = 88; // 각 스테이지 길이
}

public class CorridorSpawner : MonoBehaviour
{
    [Header("Stage 설정")]
    public int currentStage = 1;           // 현재 스테이지 번호 (1부터 시작)
    public List<StageInfo> stages;         // Stage별 정보 (인스펙터에서 설정)

    [Header("Stage Timing 설정")]
    public float bossStageDelay = 60f;     // 1분(60초) 뒤 자동 전환

    private bool stageTimerRunning = false;
    private Queue<GameObject> corridors = new Queue<GameObject>();

    [Header("Corridor 설정")]
    public int corridorCount = 5;
    public float corridorLength = 88.5f;
    public float corridorWidth = 60f;

    [Header("Player Reference")]
    public Transform player;

    void Start()
    {
        float startZ = 0f;
        for (int i = 0; i < corridorCount; i++)
        {
            string tag = GetStageCorridorTag();
            GameObject corridor = PoolingManager.Instance.SpawnFromPool(
                tag,
                new Vector3(player.position.x, 0, startZ),
                Quaternion.identity
            );
            corridors.Enqueue(corridor);
            startZ += corridorLength;
        }

        // 시작 스테이지에서 타이머 실행 여부 확인
        TryStartStageTimer();
    }

    void Update()
    {
        ManageCorridors();
    }

    // 복도 재활용
    void ManageCorridors()
    {
        if (corridors.Count == 0) return;

        GameObject first = corridors.Peek();
        if (first.transform.position.z < player.position.z - corridorLength)
        {
            GameObject old = corridors.Dequeue();
            old.SetActive(false);

            GameObject last = null;
            foreach (var c in corridors) last = c;

            float lastLength = GetPrefabLength(last);
            Vector3 newPos = last.transform.position + new Vector3(0, 0, lastLength);

            string tag = GetStageCorridorTag();
            GameObject newCorridor = PoolingManager.Instance.SpawnFromPool(tag, newPos, Quaternion.identity);
            corridors.Enqueue(newCorridor);
        }
    }

    float GetPrefabLength(GameObject obj)
    {
        Renderer rend = obj.GetComponentInChildren<Renderer>();
        return rend != null ? rend.bounds.size.z : corridorLength;
    }

    string GetStageCorridorTag()
    {
        if (currentStage < 1 || currentStage > stages.Count)
            return "Corridor";

        StageInfo stage = stages[currentStage - 1];
        if (Random.value < stage.trapChance && stage.trapTags.Count > 0)
        {
            int randIndex = Random.Range(0, stage.trapTags.Count);
            return stage.trapTags[randIndex];
        }
        return stage.normalTag;
    }

    // 스테이지에 따라 타이머 작동
    void TryStartStageTimer()
    {
        if (!stageTimerRunning)
        {
            // 1, 3일 때만 자동 전환 타이머 작동
            if (currentStage == 1 || currentStage == 3)
                StartCoroutine(AutoNextStageAfterDelay());
        }
    }

    // 1분 후 다음 스테이지(보스)로 자동 전환
    IEnumerator AutoNextStageAfterDelay()
    {
        stageTimerRunning = true;
        yield return new WaitForSeconds(bossStageDelay);

        // 스테이지 1→2, 3→4만 자동 전환
        if (currentStage == 1) currentStage = 2;
        else if (currentStage == 3) currentStage = 4;

        Debug.Log($"[Auto Stage Change] currentStage = {currentStage}");

        stageTimerRunning = false;
        TryStartStageTimer(); // 4에서 5로는 자동 변경 없음
    }
}
