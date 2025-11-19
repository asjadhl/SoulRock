using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    #region ("맵 풀링")
    public static PoolingManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;//오브젝트 풀의 태그
        public GameObject prefab;//풀에 저장할 오브젝트의 프리팹
        public int size;//풀의 크기
    }
    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;//태그와 오브젝트 큐를 매핑하는 딕셔너리
    private void Awake()
    {
        if (Instance == null) Instance = this;//싱글톤 인스턴스 설정
        else Destroy(gameObject);
    }
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();//딕셔너리 초기화

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();//오브젝트 큐 초기화

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);//프리팹 인스턴스화
                obj.SetActive(false);//오브젝트 비활성화
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);//태그와 오브젝트 큐를 딕셔너리에 추가
        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)//오브젝트 풀에서 오브젝트를 꺼내 활성화하고 위치와 회전을 설정
    {
        Queue<GameObject> pool = poolDictionary[tag];//태그에 해당하는 오브젝트 큐 가져오기
        GameObject obj = pool.Dequeue();//큐에서 오브젝트 꺼내기

        obj.SetActive(true);//오브젝트 활성화
        obj.transform.SetPositionAndRotation(position, rotation);//위치와 회전 설정
        pool.Enqueue(obj);//다시 큐에 오브젝트 넣기
        return obj;
    }
    #endregion
    #region ("스테이지별 맵 스폰")
    [System.Serializable]
    public class StageInfo
    {
        public string normalTag;//일반 맵 태그              
        public List<string> trapTags;//트랩x 지금은 맵 해당 스테이지 안에서 맵 전환용      
        [Range(0f, 1f)] public float trapChance;//트랩 맵이 나올 확률
        public float corridorLength = 85;//세로     
    }

    public class CorridorSpawner : MonoBehaviour
    {
        [Header("Stage")]
        public int currentStage = 1;//현 스테이지
        public List<StageInfo> stages;//스테이지 정보 리스트

        [Header("Stage Timing")]
        public float emptyDelay = 5f;//스테이지 전환시 빈 맵 지속 시간           
        public float normalDelay = 60f;//기본 스테이지 지속 시간      

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
            //시작시 기본맵 설치
            float startZ = 0f;
            for (int i = 0; i < corridorCount; i++)
            {
                string tag = GetStageCorridorTag();//태그 얻기
                GameObject corridor = PoolingManager.Instance.SpawnFromPool(
                    tag,
                    new Vector3(player.position.x, 0, startZ - 86),//-86은 시작맵 오차 보정용
                    Quaternion.identity
                );
                corridors.Enqueue(corridor);
                startZ += corridorLength;//다음 맵 위치
            }
            StartStageTimer();
        }
        void Update()
        {
            ManageCorridors();
        }
        void ManageCorridors()//맵 관리
        {
            if (corridors.Count == 0) return;

            GameObject first = corridors.Peek();//첫번째 맵 확인
            if (first.transform.position.z + corridorLength < player.position.z - 5f)//플레이어 기준 
            {
                GameObject old = corridors.Dequeue();
                old.SetActive(false);

                GameObject last = null;
                foreach (var c in corridors) last = c;//마지막 맵 확인


                Renderer[] renderers = last.GetComponentsInChildren<Renderer>();
                float endZ = 0f;
                foreach (Renderer r in renderers)//마지막 맵의 z 최대값 찾기
                {
                    endZ = Mathf.Max(endZ, r.bounds.max.z);//끝나는 z좌표
                }

                string tag = GetStageCorridorTag();
                GameObject newCorridor = PoolingManager.Instance.SpawnFromPool(//새 맵 생성
                    tag,
                    new Vector3(last.transform.position.x, 0, endZ - 1),//-1은 오차 보정용
                    Quaternion.identity
                );
                corridors.Enqueue(newCorridor);
            }
        }
        string GetStageCorridorTag()
        {
            if (currentStage < 1 || currentStage > stages.Count)
                return "Corridor";//둘다없으면 태그 Corridor로 들어감

            StageInfo stage = stages[currentStage - 1];
            if (Random.value < stage.trapChance && stage.trapTags.Count > 0)//트랩 맵이 나올 확률 조정
            {
                int randIndex = Random.Range(0, stage.trapTags.Count);//트랩 맵 태그 랜덤 선택
                return stage.trapTags[randIndex];//트랩 맵 태그 반환
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
        }
       
    }
    #endregion
}


