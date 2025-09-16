using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
public class DotBoxGeneratorL : MonoBehaviour
{//이 스크립트는 도트박스 풀링용.
    public static DotBoxGeneratorL Instance { get; private set; }

    [Header("도트박스 프리팹")]
    [SerializeField] GameObject dotboxPrefabL;
    [Header("음악 소스")]
    [SerializeField] AudioSource musicSource;
    [Header("BPM (박자 속도)")]
    [SerializeField] double bpm = 120.0;
    [Header("풀 사이즈")]
    [SerializeField] int poolSize = 10;
    [Header("음악 시작 지연 (초)")]
    [SerializeField] double startDelay = 1.0;

    private GameObject[] poolL;
    private int pivot = 0;
    private bool getDamage = false;

    // dspTime 기반 계산용
    private double musicStartDspTime;
    private double secondsPerBeat;

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Start()
    {
        // 풀 생성
        poolL = new GameObject[poolSize];
        for (int i = 0; i < poolL.Length; i++)
        {
            GameObject dotA = Instantiate(dotboxPrefabL, transform);
            dotA.SetActive(false);
            poolL[i] = dotA;
        }

        // 이거는 한 박자가 몇초인지. 120bpm 이면 0.5초정도?
        secondsPerBeat = 60.0 / bpm;
        // BPM/60 하면 1초당 박자 수

        // 음악을 dspTime 기준으로 예약 재생
        musicStartDspTime = AudioSettings.dspTime + startDelay;
        musicSource.PlayScheduled(musicStartDspTime);

        // 도트 생성 루프 시작
        DotBoxGen().Forget();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) getDamage = true;
        if (Input.GetKeyDown(KeyCode.S)) getDamage = false;
    }

    public GameObject GetDotBox()
    {
        GameObject dotA = poolL[pivot];
        dotA.SetActive(true);
        pivot = (pivot + 1) % poolL.Length;
        return dotA;
    }

    public void ReturnDot(GameObject dot)
    {
        dot.GetComponent<DotBoxConL>().SetColor();
        dot.SetActive(false);
        dot.transform.position = transform.position;
    }

    private async UniTask DotBoxGen()
    {
        int beatIndex = 0;

        while (true)
        {
            // 이번 박자의 dspTime 계산
            double nextBeatDsp = musicStartDspTime + secondsPerBeat * beatIndex;
            double delayMs = (nextBeatDsp - AudioSettings.dspTime) * 1000.0; // 현재 시간과 초기 시간을빼고 밀리초로 변환

            if (delayMs > 0)
                await UniTask.Delay((int)delayMs);

            // 도트 생성
            GameObject dotA = GetDotBox();
            dotA.transform.position = transform.position;

            if (getDamage)
            {
                _ = dotA.GetComponent<DotBoxConL>().ChangeColor();
            }

            beatIndex++;
        }
    }

}
