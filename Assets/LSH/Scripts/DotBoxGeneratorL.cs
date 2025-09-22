using UnityEngine;
using Cysharp.Threading.Tasks;

public class DotBoxGeneratorL : MonoBehaviour
{
    public static DotBoxGeneratorL Instance { get; private set; }

    [Header("도트박스 프리팹")]
    [SerializeField] GameObject dotboxPrefabL;
    [Header("BPM (박자 속도)")]
    public double bpm = 120.0;
    [Header("풀 사이즈")]
    [SerializeField] int poolSize = 10;
    [Header("음악 시작 지연 (초)")]
    public double startDelay = 5;

    [SerializeField] SpawnTimer spawnTimer;
    public GameObject[] poolL;
    private int pivot = 0;
    bool getDamage = false;

    public double delayMs; // 디버깅용
    double secondsPerBeat;
    public double musicStartDspTime;

    void Awake()
    {
        // 싱글톤
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        // 풀 생성
        poolL = new GameObject[poolSize];
        for (int i = 0; i < poolL.Length; i++)
        {
            GameObject dot = Instantiate(dotboxPrefabL, transform);
            dot.SetActive(false);
            poolL[i] = dot;
        }

        musicStartDspTime = AudioSettings.dspTime;
        secondsPerBeat = 60.0 / bpm;
        

        // DotBox 생성 시작
        //DotBoxGen().Forget();
    }

    void Start()
    {
        //// 풀 생성
        //poolL = new GameObject[poolSize];
        //for (int i = 0; i < poolL.Length; i++)
        //{
        //    GameObject dot = Instantiate(dotboxPrefabL, transform);
        //    dot.SetActive(false);
        //    poolL[i] = dot;
        //}

        //secondsPerBeat = 60.0 / bpm;
        //musicStartDspTime = AudioSettings.dspTime + startDelay;
        //// DotBox 생성 시작
        DotBoxGen().Forget();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) getDamage = true;
        if (Input.GetKeyDown(KeyCode.S)) getDamage = false;
    }

    public GameObject GetDotBox()
    {
        GameObject dot = poolL[pivot];
        dot.SetActive(true);
        pivot = (pivot + 1) % poolL.Length;
        return dot;
    }

    public void ReturnDot(GameObject dot)
    {
        dot.GetComponent<DotBoxConL>().SetColor();
        dot.SetActive(false);
        dot.transform.position = transform.position;
    }

    private async UniTask DotBoxGen()
    {

        while (true)
        {
            // 항상 기준 박자
            double baseDelay = secondsPerBeat * 1000.0; // ms

            // 배속 적용
            double speedMultiplier = 1.0;
            if (spawnTimer.lowDoubleDotSpeed) speedMultiplier = 2.0;    // 느리게
            else if (spawnTimer.doubleDotSpeed) speedMultiplier = 0.5;  // 빠르게

            double waitTime = baseDelay * speedMultiplier;

            await UniTask.Delay((int)waitTime);


            // 도트 생성
            GameObject dot = GetDotBox();
            dot.transform.position = transform.position;
            if (getDamage)
                _ = dot.GetComponent<DotBoxConL>().ChangeColor();

        }
    }
}