using Cysharp.Threading.Tasks;
using UnityEngine;

public class Stage3Boss : MonoBehaviour
{
    [Header("LazerChargeOB")]
    [SerializeField] GameObject chargeLazer;
    [Header("LazerOB")]
    [SerializeField] GameObject lazer;
    [Header("LazerBallOB")]
    [SerializeField] GameObject lazerBall;
    [Header("BigChargeLazerOB")]
    [SerializeField] GameObject bigChargeLazer;
    [Header("BigLazerOB")]
    [SerializeField] GameObject bigLazer;
    [Header("MirrorOB")]
    [SerializeField] GameObject mirror;
    [Header("Pooling")]
    [SerializeField] GameObject[] lazerPool;
    [SerializeField] GameObject[] lazerBallPool;
    [SerializeField] GameObject[] bigLazerBallPool;
    [Header("공격 coolTime")]
    [SerializeField] int coolTime = 6000;
    [SerializeField] float firstOfLazerSize = 0.02f;
    Vector3[] thisPos;
    Transform player;
    Animator anime;
    BossHP hp;
    MusicBox musicBox;
    [Header("Renderer")]
    [SerializeField] Material material;
	int ranIndex = 0;
    int ranPos;
    //lazer 전용 풀 인덱스
    int poolIndex = 0;
    //lazerBall 전용 풀 인덱스
    //int poolBall = 0;
    int poolBigLazer = 0;
    bool isAttacking = false;
    bool isAngry = false;
    bool animeOn = false;

    void Start()
    {
		player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        anime = GetComponent<Animator>();
        hp = GetComponent<BossHP>();
        musicBox = GameObject.FindWithTag("MusicBox").GetComponent<MusicBox>();
        material.color = Color.white;
		ReadyforLazerAttack();
        ReadyforLazerBallAttack();
        ReadyforBigLazerAttack();
        chargeLazer.SetActive(false);
        bigChargeLazer.SetActive(false);
        mirror.SetActive(false);
        ranIndex = Random.Range(0, 3);
        //ranIndex = 2;
    }



    void ReadyforLazerAttack()
    {
        lazerPool = new GameObject[3];
        for (int i = 0; i < lazerPool.Length; i++)
        {
            GameObject lazerAttack = Instantiate(lazer, transform.position, Quaternion.identity);
            lazerAttack.transform.parent = transform;
            lazerAttack.SetActive(false);
            lazerPool[i] = lazerAttack;
        }
    }
    void ReadyforLazerBallAttack()
    {
        lazerBallPool = new GameObject[7];
        thisPos = new Vector3[7];
        thisPos[0] =  new Vector3(-5f , 6f, transform.position.z);
        thisPos[1] = new Vector3(5f, 6f, transform.position.z);
        thisPos[2] = new Vector3(-4f, 8f, transform.position.z);
        thisPos[3] = new Vector3(4f, 8f, transform.position.z);
        thisPos[4] = new Vector3(2f, 10f, transform.position.z);
        thisPos[5] = new Vector3(-2f, 10f, transform.position.z);
        thisPos[6] = new Vector3(0, 10.5f, transform.position.z);
        for (int i = 0; i<lazerBallPool.Length; i++)
        {
            GameObject lazerBallAttack = Instantiate(lazerBall, thisPos[i], Quaternion.identity);
            lazerBallAttack.transform.parent = transform;
            lazerBallAttack.SetActive(false);   
            lazerBallPool[i] = lazerBallAttack;
        }
    }
    void ReadyforBigLazerAttack()
    {
        bigLazerBallPool = new GameObject[2];
        for(int i = 0;i<bigLazerBallPool.Length;i++)
        {
            GameObject biglazerAttack = Instantiate(bigLazer, bigChargeLazer.transform.position, Quaternion.identity);
            biglazerAttack.transform.parent = transform;
            biglazerAttack.SetActive(false);
            bigLazerBallPool[i] = biglazerAttack;
        }    
    }

    void Update()
    {
        if (hp.bossHP == 30)
        {
			Phase2();
			isAngry = true;
            animeOn = true;
        }
			
		if (!isAttacking)  
        {
            Boss3Pattern();
        }
	}

    void Boss3Pattern()
    {
        switch (ranIndex)
        {
            case 0:
                Debug.Log("레이저");
                _ = ChargeLazerAttack();
                ranIndex = Random.Range(0, 3);
                break;
            case 1:
                Debug.Log("레이저볼");
                _ = secondPattern();
                ranIndex = Random.Range(0, 3);
				break;
            case 2:
                Debug.Log("빅레이저");
                _ = ThirdPattern();
                ranIndex = Random.Range(0, 3);
                break;
        }
    }
    private async UniTask ChargeLazerAttack()
    {
        isAttacking = true;
        chargeLazer.SetActive(true);
        for (int i = 1; i < 100; i++)
        {
            chargeLazer.transform.localScale =
                new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * i;
            await UniTask.Delay(20);
        }

        _ = LazerAttack();
    }

    private async UniTask LazerAttack()
    {
		anime.SetTrigger("BloodAttack");
		chargeLazer.SetActive(false);
        lazerPool[poolIndex].transform.position = new Vector3(chargeLazer.transform.position.x, chargeLazer.transform.position.y - 1f, chargeLazer.transform.position.z);
        lazerPool[poolIndex].SetActive(true);
        await UniTask.Delay(3000);
        ReturnLazer(lazerPool[poolIndex]);
        if (poolIndex >= lazerPool.Length-1)
        {
            poolIndex = 0;
        }
        else
            poolIndex++;
        await UniTask.Delay(coolTime);
        isAttacking = false;
    }
    void ReturnLazer(GameObject lazer)
    {
        lazer.SetActive(false);
        lazer.transform.position = transform.position;
    }

    
    private async UniTask secondPattern()
    {
        isAttacking = true;
        Debug.Log("레이져볼 공격중");
        
        for (int i = 0; i < lazerBallPool.Length; i++)
        {
            lazerBallPool[i].transform.position = transform.position + thisPos[i]; 
            lazerBallPool[i].SetActive(true);
			anime.SetTrigger("SecondPattern");
			if (!lazerBallPool[i].activeSelf)
            {
                break;
            }
            for (int j = 1; j < 40; j++)
            {
                lazerBallPool[i].transform.localScale = new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * j;
                await UniTask.Delay(2);
            }
			await UniTask.Delay(800);
        }
		await UniTask.Delay(coolTime);
		isAttacking = false;
	}

    //private async UniTask secondPattern()
    //{
    //    isAttacking = true;
    //    Debug.Log("레이저볼 공격중");

    //    var tasks = new UniTask[lazerBallPool.Length];
    //    for (int i = 0; i < lazerBallPool.Length; i++)
    //    {
    //        tasks[i] = ActivateLazerBall(lazerBallPool[i]);
    //        await UniTask.Delay(1000);
    //    }

    //    await UniTask.WhenAll(tasks); // 모든 작업이 완료될 때까지 대기
    //    await UniTask.Delay(3000);
    //    isAttacking = false;
    //}

    //private async UniTask ActivateLazerBall(GameObject lazerBall)
    //{
    //    lazerBall.SetActive(true);

    //    // 크기 조정
    //    for (int j = 1; j < 40; j++)
    //    {
    //        lazerBall.transform.localScale = new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * j;
    //        await UniTask.Delay(2);
    //    }

    //}

    

    private async UniTask ThirdPattern()
    {
        isAttacking = true;
        bigChargeLazer.SetActive(true);
        mirror.SetActive(true);
        mirror.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
		anime.SetTrigger("BloodAttackReady");
		for (int i = 1; i < 250; i++)
        {
			
			bigChargeLazer.transform.localScale =
                new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * i;
            await UniTask.Delay(15);
        }
		anime.SetTrigger("BloodAttack");
		BigLazerAttack();
        await UniTask.Delay(4000);
        mirror.SetActive(false);
        isAttacking = false;
    }
    void BigLazerAttack()
    {
        bigChargeLazer.SetActive(false);
		bigLazerBallPool[poolBigLazer].SetActive(true);
        bigLazerBallPool[poolBigLazer].transform.position = bigChargeLazer.transform.position;
		bigLazerBallPool[poolBigLazer].transform.LookAt(player.position);

		if (poolBigLazer >= bigLazerBallPool.Length - 1)
        {
            poolBigLazer = 0;
        }
        else
            poolBigLazer++;
	}
	void Phase2()
    {
        if (!animeOn)
        {
			anime.Play("Jump");
		}
	    material.color = Color.red;
		coolTime = 500;
    }

}
