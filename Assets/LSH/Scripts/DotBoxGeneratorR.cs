using UnityEngine;
using Cysharp.Threading.Tasks;
public class DotBoxGeneratorR : MonoBehaviour
{//이 스크립트는 도트박스 풀링용.
    public static DotBoxGeneratorR Instance { get; private set; }

    [Header("도트박스 프리팹")]
    [SerializeField] GameObject dotboxPrefabR;
    [Header("생성타이밍")]
    int dotboxTime = 500;
    public GameObject[] poolR;
    private int pivot = 0;

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    void Start()
    {
        poolR = new GameObject[10];
        for(int i = 0; i < poolR.Length; i++)
        {
            GameObject dotA = Instantiate(dotboxPrefabR, transform);
            dotA.SetActive(false);
            poolR[i] = dotA;
        }
        DotBoxGen().Forget(); //Forget을 사용안하면 Await을 기다리기 전에 Start가 종료됨.지우지말것.
    }
    void Update() 
    {
        
    }
    public GameObject GetDotBox() //활성화용도
    {
        GameObject dotA = poolR[pivot];
        dotA.SetActive(true);
        pivot++;
        if (pivot >= poolR.Length)
        {
            pivot = 0;
        }
        return dotA;
    }
    public void ReturnDot(GameObject dot) //비활성화용도
    {
        dot.SetActive(false);
        dot.transform.position = transform.position; // 위치 초기화
    }
    private async UniTask DotBoxGen()
    {
        while (true)
        {
           // 도트 생성
            GameObject dotA = GetDotBox();
            dotA.transform.position = transform.position;
            // 2초 대기
            await UniTask.Delay(dotboxTime);
        }
    }
}
