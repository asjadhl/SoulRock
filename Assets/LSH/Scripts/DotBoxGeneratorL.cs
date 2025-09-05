using UnityEngine;
using Cysharp.Threading.Tasks;
public class DotBoxGeneratorL : MonoBehaviour
{//이 스크립트는 도트박스 풀링용.
    public static DotBoxGeneratorL Instance { get; private set; }

    [Header("도트박스 프리팹")]
    [SerializeField] GameObject dotboxPrefabL;
    public GameObject[] poolL;
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
        poolL = new GameObject[20];
        for(int i = 0; i < poolL.Length; i++)
        {
            GameObject dotA = Instantiate(dotboxPrefabL, transform);
            dotA.SetActive(false);
            poolL[i] = dotA;
        }
        DotBoxGen().Forget(); //Forget을 사용안하면 Await을 기다리기 전에 Start가 종료됨.지우지말것.
    }
    void Update() 
    {
        
    }
    public GameObject GetDotBox() //활성화용도
    {
        GameObject dotA = poolL[pivot];
        dotA.SetActive(true);
        pivot++;
        if (pivot >= poolL.Length)
        {
            pivot = 0;
        }
        return dotA;
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
            GameObject dotA = GetDotBox();
            dotA.transform.position = transform.position;
            // 2초 대기
            await UniTask.Delay(2000);
        }
    }
}
