using System;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    //스테이지 1시작후 1분뒤에 보스소환
    //보스소환후 맵 스테이지 2로 변경
    //보스가 죽으면 맵 스테이지 3으로 변경
    //플레이어 앞에 소환

    public Transform player;               // 플레이어 (고정)
    public GameObject[] bossPrefab;          // 보스 프리팹
    private GameObject currentBoss;        // 현재 소환된 보스
    private bool bossSpawned = false;      // 보스가 소환되었는지 여부
    private float spawnDelay = 5f;        // 보스 소환 지연 시간 (초)
    private float timer = 0f;              // 타이머
    private int currentStage = 1;          // 현재 스테이지 번호 (1부터 시작)
    private int oldStage = 1;              // 이전 스테이지 번호
    private string oldTag;                 // 이전 태그
    public string bossSpawnerTag = "Boss"; // 보스 스포너 태그
    public CorridorSpawner corridorSpawner; // 코리도어 스포너 참조

    void Start()
    {
        timer = 0f;
        bossSpawned = false;
        currentStage = 1;
        oldStage = 1;
    }
    void Update()
    {
        if (currentStage == 1 && !bossSpawned)
        {
            timer += Time.deltaTime;
            if (timer >= spawnDelay)
            {
                SpawnBoss();
                bossSpawned = true;
                currentStage = 2;
                corridorSpawner.currentStage = 2; // 코리도어 스테이지 변경
            }
        }
        if (currentBoss != null && !currentBoss.activeInHierarchy)
        {
            currentStage = 3;
            corridorSpawner.currentStage = 3; // 코리도어 스테이지 변경
            bossSpawned = false; // 보스가 죽었으므로 다시 소환 가능
            timer = 0f; // 타이머 초기화
        }
    }
    public void SpawnBoss()
    {
        if (bossPrefab.Length == 0) return;

        int bossIndex = UnityEngine.Random.Range(0, bossPrefab.Length);
        Vector3 spawnPosition = new Vector3(0, 1, player.position.z + 13f); // 플레이어 앞에 소환
        currentBoss = Instantiate(bossPrefab[bossIndex], spawnPosition, Quaternion.identity);
        currentBoss.SetActive(true);
    }





}
