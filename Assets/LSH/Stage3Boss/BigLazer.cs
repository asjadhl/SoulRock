using Cysharp.Threading.Tasks;
using UnityEngine;

public class BigLazer : MonoBehaviour
{
    private Transform player;
    //private Transform boss;
    //private BossHP bossHp;
    //MatarialAlpha mirror;
    //ParticleSystem explosionEffect;
    int BigLazerHp = 1;
    ParticleManager particleManager;
    bool isInitialized = false;
    bool reflect =false;
    //public bool isGoing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //boss = GameObject.FindWithTag("Stage3Boss").GetComponent<Transform>();
		//bossHp = GameObject.FindWithTag("Stage3Boss").GetComponent<BossHP>();
		//mirror = FindAnyObjectByType<MatarialAlpha>();
        //explosionEffect = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>().hitParticle;
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
        
    }
    private void Start()
    {
        isInitialized = true;
        //isGoing = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeSelf)
        {
			transform.Translate(Vector3.forward * 5 * Time.deltaTime);
		}
        if (BossState.isBoss2Dead) gameObject.SetActive(false);
		if (!CircleHit.Instance.isHighLight) gameObject.SetActive(false);
	}
    private void OnTriggerEnter(Collider col)
    {   
        if(col.CompareTag("Player"))
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPBigMinus().Forget();
			//isGoing = false;
			gameObject.SetActive(false);
        }
       
    }
	private void OnEnable()
	{
		transform.LookAt(player.position);
	}
	//private void OnDisable()
	//{
	//    Vector3 effectPos = transform.position;
	//    var explosion = Instantiate(explosionEffect, effectPos, Quaternion.identity);
	//    explosion.Play();
	//}

	private void OnDisable()
    {
        //isGoing = false;
        if (!isInitialized) return;
        Vector3 effectPos = transform.position + new Vector3(0, -1f, 0);
        particleManager.PlayHitEffect(effectPos);
    }

    public void BallHpMin()
    {
        //isGoing = false;
        BigLazerHp--;
        if (BigLazerHp <= 0)
        {
            gameObject.SetActive(false);
        }
	}
}
