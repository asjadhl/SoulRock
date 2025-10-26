using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RotateOB : MonoBehaviour
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
        transform.Rotate(0f, 0f, 100f * Time.deltaTime);
        if (gameObject.activeSelf == true)
        {
            OBMove();
        }

    }
    public async void OBMove()
    {
        await UniTask.Delay(5000);
        transform.LookAt(player.position);
        transform.Translate(Vector3.forward * OBSpeed * Time.fixedDeltaTime);
    }
    // Update is called once per frame
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
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _ = GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        if (!isInitialized) return; //시작 폭발 방지임ㅋㅋ
        Vector3 effectPos = transform.position + new Vector3(0, 1f, 2f);
        particleManager.PlayHitEffect(effectPos);
        gameObject.SetActive(false);
    }
}
