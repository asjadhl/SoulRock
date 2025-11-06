using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public string normalTag;              
    public List<string> trapTags;//트랩x 지금은 그냥 맵 분위기 전환용      
    [Range(0f, 1f)] public float trapChance;
    public float corridorLength = 85;     
}

public class CorridorSpawner : MonoBehaviour
{
    [Header("Stage")]
    public int currentStage = 1;//현 스테이지
    public List<StageInfo> stages;

    [Header("Stage Timing")]
    public float emptyDelay = 5f;           
    public float normalDelay = 60f;         

    private bool isTransitionRunning = false;

    [Header("Corridor")]
    public int corridorCount = 5;//뒤에 이어지는 맵의 수
    public float corridorLength = 85f;//세로
    public float corridorWidth = 60f;//가로

    [Header("Player")]
    public Transform player;

    private Queue<GameObject> corridors = new Queue<GameObject>();
    private Coroutine stageCoroutine;

    void Start()
    { 
        //시작시 맵 설치
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

        StartStageTimer();
    }

    void Update()
    {
        ManageCorridors();
    }

    void ManageCorridors()
    {
        if (corridors.Count == 0) return;

        GameObject first = corridors.Peek();
        if (first.transform.position.z + corridorLength < player.position.z - 5f)//플레이어 기준 
        {
            GameObject old = corridors.Dequeue();
            old.SetActive(false);

            GameObject last = null;
            foreach (var c in corridors) last = c;

            
            Renderer[] renderers = last.GetComponentsInChildren<Renderer>();
            float endZ = 0f;
            foreach (Renderer r in renderers)
            {
                endZ = Mathf.Max(endZ, r.bounds.max.z);
            }

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
    //    return rend.bounds.size.z;
    //}


    string GetStageCorridorTag()
    {
        if (currentStage < 1 || currentStage > stages.Count)
            return "Corridor";//둘다없으면 이걸로 들어감

        StageInfo stage = stages[currentStage - 1];
        if (Random.value < stage.trapChance && stage.trapTags.Count > 0)
        {
            int randIndex = Random.Range(0, stage.trapTags.Count);
            return stage.trapTags[randIndex];
        }
        return stage.normalTag;
    }

    // 타이머 관리
    void StartStageTimer()
    {
        if (stageCoroutine != null)
        {
            StopCoroutine(stageCoroutine);
            stageCoroutine = null;
        }
        //if (IsAutoStage(currentStage))
        //{
        //    float delay = GetStageDelay(currentStage);
        //    stageCoroutine = StartCoroutine(StageTimer(delay));
        //}
    }
    //아래는 전부 보스스테이지가 따로 있었을때
    //IEnumerator StageTimer(float delay)
    //{
    //    isTransitionRunning = true;
    //    yield return new WaitForSeconds(delay);

    //    currentStage++;

    //    isTransitionRunning = false;
    //    StartStageTimer(); 
    //}

    //bool IsAutoStage(int stage)
    //{
    //    return stage == 1 || stage == 2 /*|| stage == 4 || stage == 5*/;
    //}

    //float GetStageDelay(int stage)
    //{
    //    if (stage == 1 /*|| stage == 4*/)
    //        return emptyDelay;
    //    else if (stage == 2 /*|| stage == 5*/)
    //        return normalDelay;
    //    return 0f;
    //}
}
