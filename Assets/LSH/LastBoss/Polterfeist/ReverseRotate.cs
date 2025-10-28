using Cysharp.Threading.Tasks;
using UnityEngine;

public class ReverseRotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("OBSpeed")]
    [SerializeField] float OBSpeed = 20f;
    private ParticleManager particleManager;
    private bool isInitialized = false;
    private Transform player;
    //bool isAttack = false;
    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
    }

    

    void Update()
    {
        transform.Rotate(0f, 0f, -60f * Time.deltaTime);
        if (gameObject.activeSelf == true)
        {
            OBMove();
        }

    }
    private void OnEnable()
    {
        isInitialized = false;
        this.DelayInitialize().Forget();
    }

    private async UniTaskVoid DelayInitialize()
    {
        await UniTask.Yield(); // 한 프레임 대기
        isInitialized = true;
    }
    public async void OBMove()
    {
        await UniTask.Delay(3000);
        transform.LookAt(player.position);
        transform.Translate(Vector3.forward * OBSpeed * Time.fixedDeltaTime);
       
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus().Forget();
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        if (!isInitialized) return; //시작 폭발 방지임ㅋㅋ
        Vector3 effectPos = transform.position + new Vector3(0, 1f, 2f);
        particleManager.PlayChairBoom(effectPos);
        gameObject.SetActive(false);
    }
}
