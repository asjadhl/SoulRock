using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public string normalTag;                // 일반 복도 태그
    public List<string> trapTags;           // 트랩 복도 태그
    [Range(0f, 1f)] public float trapChance;
    public float corridorLength = 85;       // 복도 길이
}

public class CorridorSpawner : MonoBehaviour
{
    [Header("Stage 설정")]
    public int currentStage = 1;
    public List<StageInfo> stages;

    [Header("Stage Timing 설정")]
    public float emptyDelay = 5f;           // 빈 맵 대기 시간
    public float normalDelay = 60f;         // 일반 스테이지 -> 보스 대기 시간

    private bool isTransitionRunning = false;

    [Header("Corridor 설정")]
    public int corridorCount = 5;
    public float corridorLength = 85f;
    public float corridorWidth = 60f;

    [Header("Player Reference")]
    public Transform player;

    private Queue<GameObject> corridors = new Queue<GameObject>();
    private Coroutine stageCoroutine; // 현재 진행 중인 코루틴 저장용

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("CorridorSpawner: player reference is not assigned");
            return;
        }

        if (PoolingManager.Instance == null)
        {
            Debug.LogError("CorridorSpawner: PoolingManager instance is missing");
            return;
        }

        float startZ = 0f;
        for (int i = 0; i < corridorCount; i++)
        {
            string tag = GetStageCorridorTag();
            GameObject corridor = PoolingManager.Instance.SpawnFromPool(
                tag,
                new Vector3(player.position.x, 0, startZ-86),
                Quaternion.identity
            );
            corridors.Enqueue(corridor);
            startZ += corridorLength;
        }

        StartStageTimer(); // 첫 스테이지 시작
    }

    void Update()
    {
        ManageCorridors();
    }

    void ManageCorridors()
    {
        if (corridors.Count == 0) return;

        GameObject first = corridors.Peek();

        // 플레이어가 첫 번째 복도를 지나쳤는지 검사
        if (first.transform.position.z + corridorLength < player.position.z - 5f)
        {
            // 가장 앞 복도 제거
            GameObject old = corridors.Dequeue();
            old.SetActive(false);

            // 마지막 복도 찾기
            GameObject last = null;
            foreach (var c in corridors) last = c;

            if (last == null)
            {
                Debug.LogWarning("No last corridor found");
                return;
            }

            // 마지막 복도의 실제 끝 Z좌표 계산
            Renderer[] renderers = last.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Debug.LogWarning("Last corridor has no renderer, using fallback length");
            }

            float endZ = 0f;
            foreach (Renderer r in renderers)
            {
                // bounds.max.z는 오브젝트의 “끝 지점”
                endZ = Mathf.Max(endZ, r.bounds.max.z);
            }

            // 다음 복도의 시작 위치를 복도 끝점에 정확히 이어붙임
            string tag = GetStageCorridorTag();
            GameObject newCorridor = PoolingManager.Instance.SpawnFromPool(
                tag,
                new Vector3(last.transform.position.x, 0, endZ-1),
                Quaternion.identity
            );
            corridors.Enqueue(newCorridor);
        }
    }



    //float GetPrefabLength(GameObject obj)
    //{
    //    if (obj == null)
    //    {
    //        Debug.LogWarning("GetPrefabLength() called with null object");
    //        return corridorLength;
    //    }

    //    Renderer rend = obj.GetComponentInChildren<Renderer>();
    //    if (rend == null)
    //        return corridorLength;

    //    // 실제 월드 스케일까지 반영
    //    return rend.bounds.size.z;
    //}


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

    // 스테이지 타이머 관리
    void StartStageTimer()
    {
        // 중복 방지
        if (stageCoroutine != null)
        {
            StopCoroutine(stageCoroutine);
            stageCoroutine = null;
        }

        //// 다음 스테이지로 넘어가는 시간이 정해진 경우에만 시작
        //if (IsAutoStage(currentStage))
        //{
        //    float delay = GetStageDelay(currentStage);
        //    stageCoroutine = StartCoroutine(StageTimer(delay));
        //}
    }

    //IEnumerator StageTimer(float delay)
    //{
    //    isTransitionRunning = true;
    //    yield return new WaitForSeconds(delay);

    //    currentStage++;

    //    isTransitionRunning = false;
    //    StartStageTimer(); // 다음 단계 자동 실행
    //}

    //bool IsAutoStage(int stage)
    //{
    //    // 1, 2, 4, 5 단계만 자동 진행
    //    return stage == 1 || stage == 2 /*|| stage == 4 || stage == 5*/;
    //}

    //float GetStageDelay(int stage)
    //{
    //    // 1,4는 빈맵, 2,5는 일반맵
    //    if (stage == 1 /*|| stage == 4*/)
    //        return emptyDelay;
    //    else if (stage == 2 /*|| stage == 5*/)
    //        return normalDelay;
    //    return 0f;
    //}
}
