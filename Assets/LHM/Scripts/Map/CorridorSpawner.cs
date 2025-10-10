using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public string normalTag;                // 일반 복도 태그
    public List<string> trapTags;           // 트랩 복도 태그
    [Range(0f, 1f)] public float trapChance;
    public float corridorLength = 88;       // 복도 길이
}

public class CorridorSpawner : MonoBehaviour
{
    [Header("Stage 설정")]
    public int currentStage = 1;
    public List<StageInfo> stages;

    [Header("Stage Timing 설정")]
    public float emptyDelay = 5f;           // 빈 맵 대기 시간
    public float normalDelay = 60f;         // 일반 스테이지  보스 대기 시간

    private bool stageTimerRunning = false;

    [Header("Corridor 설정")]
    public int corridorCount = 5;
    public float corridorLength = 88.5f;
    public float corridorWidth = 60f;

    [Header("Player Reference")]
    public Transform player;

    private Queue<GameObject> corridors = new Queue<GameObject>();

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

        TryStartStageTimer();
    }

    void Update()
    {
        ManageCorridors();
    }

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
            Vector3 newPos = last.transform.position + new Vector3(0, 0, lastLength+87F);

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
            return "Corridor"; // 기본값

        StageInfo stage = stages[currentStage - 1];
        if (Random.value < stage.trapChance && stage.trapTags.Count > 0)
        {
            int randIndex = Random.Range(0, stage.trapTags.Count);
            return stage.trapTags[randIndex];
        }
        return stage.normalTag;
    }

    // 스테이지 타이머 시작
    void TryStartStageTimer()
    {
        if (stageTimerRunning) return;

        // 자동 변경 패턴
        if (currentStage == 1 || currentStage == 4)
        {
            StartCoroutine(AutoNextStage(emptyDelay));   // 빈 맵 다음
        }
        else if (currentStage == 2 || currentStage == 5)
        {
            StartCoroutine(AutoNextStage(normalDelay));  // 일반  보스
        }
        // 3, 6, 7은 자동 변경 없음 (보스 구간)
    }

    IEnumerator AutoNextStage(float delay)
    {
        stageTimerRunning = true;
        yield return new WaitForSeconds(delay);

        // 다음 스테이지로 전환
        currentStage++;
        Debug.Log($"[Auto Stage Change] currentStage = {currentStage}");

        stageTimerRunning = false;
        TryStartStageTimer(); // 다음 단계도 자동일 경우 이어서 실행
    }
}
