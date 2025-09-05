using UnityEngine;
using Cysharp.Threading.Tasks;
public class DotBoxGenerator : MonoBehaviour
{//이 스크립트는 도트박스 풀링용.
    public static DotBoxGenerator Instance { get; private set; }

    [Header("도트박스 프리팹")]
    [SerializeField] GameObject dotboxPrefab;
    public GameObject[] pool;
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
        pool = new GameObject[20];
        for(int i = 0; i < pool.Length; i++)
        {
            GameObject dot = Instantiate(dotboxPrefab, transform);
            dot.SetActive(false);
            pool[i] = dot;
        }
        DotBoxGen().Forget(); //Forget을 사용안하면 Await을 기다리기 전에 Start가 종료됨.지우지말것.
    }
    void Update() 
    {
        
    }
    public GameObject GetDotBox() //활성화용도
    {
        GameObject dot = pool[pivot];
        dot.SetActive(true);
        pivot++;
        if (pivot >= pool.Length)
        {
            pivot = 0;
        }
        return dot;
    }
    public void ReturnDot(GameObject dot) //비활성화용도
    {
        dot.SetActive(false);
    }
    private async UniTask DotBoxGen()
    {
        while (true)
        {
           // 도트 생성
            GameObject dot = GetDotBox();
            dot.transform.position = transform.position;
            // 2초 대기
            await UniTask.Delay(2000);
        }
    }
}
