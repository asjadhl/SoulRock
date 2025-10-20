using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class BigLazer : MonoBehaviour
{
    private Transform player;
    private Transform boss;
    private BossHP bossHp;
    MatarialAlpha mirror;
    //ParticleSystem explosionEffect;
    ParticleManager particleManager;
    bool isInitialized = false;
    bool reflect =false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        boss = GameObject.FindWithTag("Stage3Boss").GetComponent<Transform>();
		bossHp = GameObject.FindWithTag("Stage3Boss").GetComponent<BossHP>();
		mirror = FindAnyObjectByType<MatarialAlpha>();
        //explosionEffect = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>().hitParticle;
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
    }
    private void Start()
    {
        isInitialized = true;
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * 20 * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider col)
    {   
        if(col.CompareTag("Player"))
        {
            _ = GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
            gameObject.SetActive(false);
        }
        if(col.CompareTag("Mirror") && mirror.successMirror)
        {
			transform.LookAt(boss.position);
			mirror.gameObject.SetActive(false);
        }
        if(col.CompareTag("Stage3Boss"))
        {
			//데미지 입히는거 넣기
			//bossHp.BossHPMinus();
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
        Vector3 effectPos = transform.position + new Vector3(0, 1f, 0);
        particleManager.PlayHitEffect(effectPos);
    }
}
