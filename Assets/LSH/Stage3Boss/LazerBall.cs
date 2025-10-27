using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class LazerBall : MonoBehaviour
{
    [Header("LazerBallSpeed")]
    [SerializeField] float lazerBallspeed = 5f;

    private Transform player;
    private Transform boss;
    //ParticleSystem explosionEffect;
    ParticleManager particleManager;
    //Vector3 oriPos;
    //float x;
    //float y;
    bool isInitialized = false;
    bool isAttack = false;
    private void Awake()
    {
        //x = transform.position.x;
        //y = transform.position.y;
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        boss = GameObject.FindWithTag("Stage3Boss").GetComponent<Transform>();
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
    }

    //private void FixedUpdate()
    //{
    //oriPos = new Vector3(x, y, boss.position.z);
    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    //    boss = GameObject.FindWithTag("Stage3Boss").GetComponent<Transform>();
    //}

    // Update is called once per frame

    private void Start()
    {
        isInitialized = true;
    }
    void Update()
    {
        if(gameObject.activeSelf == true)
        {
            lazerMove();
        }

    }

    void lazerMove()
    {
        //Vector3 targetPos = player.position;
        //lazerBallPool[j].transform.position = Vector3.MoveTowards(
        //lazerBallPool[j].transform.position, targetPos, lazerBallspeed * Time.deltaTime);
        //await UniTask.Delay(100);
        transform.LookAt(player.position);
        transform.Translate(Vector3.forward * lazerBallspeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus().Forget();
            gameObject.SetActive(false);
        }
    }

    //private void OnDisable()
    //{
    //    Vector3 effectPos = transform.position;
    //    var explosion = Instantiate(explosionEffect, effectPos, Quaternion.identity);
    //    explosion.Play();
    //}
    private void OnDisable()
    {
        if (!isInitialized) return;
		if (particleManager == null) return;
		Vector3 effectPos = transform.position + new Vector3(0, 1f, 0);
        particleManager.PlayHitEffect(effectPos);
    }
}
