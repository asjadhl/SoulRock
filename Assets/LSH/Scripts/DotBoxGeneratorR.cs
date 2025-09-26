using UnityEngine;
using Cysharp.Threading.Tasks;

public class DotBoxGeneratorR: MonoBehaviour
{
    public static DotBoxGeneratorR Instance { get; private set; }

    [Header("도트박스 프리팹")]
    [SerializeField] GameObject dotboxPrefabR;
    [Header("BPM (박자 속도)")]
    public double bpm = 120.0;
    [Header("풀 사이즈")]
    [SerializeField] int poolSize = 10;

    [SerializeField] SpawnTimer spawnTimer;
    public GameObject[] poolR;
    private int pivot = 0;
    public bool getDamage = false;

    public double delayMs; // 디버깅용
    double secondsPerBeat;

    void Awake()
    {
        // 싱글톤
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        // 풀 생성
        poolR = new GameObject[poolSize];
        for (int i = 0; i < poolR.Length; i++)
        {
            GameObject dot = Instantiate(dotboxPrefabR, transform);
            dot.SetActive(false);
            poolR[i] = dot;
        }

        secondsPerBeat = 60.0 / bpm;

    }

    void Start()
    {
        //    // 풀 생성
        //    poolR = new GameObject[poolSize];
        //    for (int i = 0; i < poolR.Length; i++)
        //    {
        //        GameObject dot = Instantiate(dotboxPrefabR, transform);
        //        dot.SetActive(false);
        //        poolR[i] = dot;
        //    }

        //    secondsPerBeat = 60.0 / bpm;

        // DotBox 생성 시작
        DotBoxGen().Forget();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) getDamage = true;
        if (Input.GetKeyDown(KeyCode.S)) getDamage = false;
    }

    public GameObject GetDotBox()
    {
        GameObject dot = poolR[pivot];
        dot.SetActive(true);
        pivot = (pivot + 1) % poolR.Length;
        return dot;
    }

    public void ReturnDot(GameObject dot)
    {
        dot.GetComponent<DotBoxConR>().SetColor();
        dot.SetActive(false);
        dot.transform.position = transform.position;
    }

    private async UniTask DotBoxGen()
    {

        while (true)
        {
            // 제발 되라
            double normalDelay = secondsPerBeat * 1000.0; // ms

            // 염병
            double speedMultiplier = 1.0;
            if (spawnTimer.lowDoubleDotSpeed) speedMultiplier = 2.0;    // 느리게
            else if (spawnTimer.doubleDotSpeed) speedMultiplier = 0.5;  // 빠르게

            double waitTime = normalDelay * speedMultiplier;

            await UniTask.Delay((int)waitTime);


            // 도트 생성
            GameObject dot = GetDotBox();
            dot.transform.position = transform.position;
            if (getDamage)
                _ = dot.GetComponent<DotBoxConR>().ChangeColor();

        }
    }
}