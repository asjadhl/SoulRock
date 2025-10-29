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
    public bool isGoing;
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
        isGoing = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeSelf&&isGoing)
        {
			transform.Translate(Vector3.forward * 10 * Time.deltaTime);
		}
        
    }
    private void OnTriggerEnter(Collider col)
    {   
        if(col.CompareTag("Player"))
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPBigMinus().Forget();
			isGoing = false;
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
        Vector3 effectPos = transform.position + new Vector3(0, -1f, 0);
        particleManager.PlayHitEffect(effectPos);
    }

    public void BallHpMin()
    {
		isGoing = false;
		BigLazerHp--;
        if (BigLazerHp <= 0)
        {
            gameObject.SetActive(false);
        }
	}
}
