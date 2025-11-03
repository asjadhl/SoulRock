using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage3Boss : MonoBehaviour
{
    [Header("LazerChargeOB")]
    [SerializeField] GameObject chargeLazer;
    //[Header("LazerOB")]
    //[SerializeField] GameObject lazer;
    [Header("LazerBallOB")]
    [SerializeField] GameObject lazerBall;
    [Header("BigChargeLazerOB")]
    [SerializeField] GameObject bigChargeLazer;
    [Header("BigLazerOB")]
    [SerializeField] GameObject bigLazer;
    //[Header("MirrorOB")]
    //[SerializeField] GameObject mirror;
    [Header("Pooling")]
    [SerializeField] GameObject[] lazerPool;
    [SerializeField] GameObject[] lazerBallPool;
    [SerializeField] GameObject[] bigLazerBallPool;
    [Header("공격 coolTime")]
    [SerializeField] int coolTime = 6000;
    [SerializeField] float firstOfLazerSize = 0.02f;
    [Header("카메라")]
    [SerializeField] CinemachineCamera cinemachineCamera;
    Vector3[] thisPos;
    Transform player;
    Animator anime;
    BigLazer bigLazerBool;
    PlayerHP playerHP;
    //BossHP hp;
    [Header("Renderer")]
    [SerializeField] Material material;
	int ranIndex = 0;
    int ranIndexBefore = 0;
    //lazer 전용 풀 인덱스
    int poolIndex = 0;
    //lazerBall 전용 풀 인덱스
    //int poolBall = 0;
    int poolBigLazer = 0;
    bool isAttacking = true;
    bool isAngry = false;
    bool animeOn = false;
    [SerializeField] GameObject monsterSpawner;
    NormalMusicBox normalMusicBox;
    BossTextManager bossTextManager;
    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        playerHP = GameObject.FindWithTag("Player").GetComponent<PlayerHP>();
		normalMusicBox = GameObject.FindWithTag("MusicBox").GetComponent<NormalMusicBox>();
        anime = GetComponent<Animator>();
        material.color = Color.white;
        ReadyforLazerBallAttack();
        //ReadyforLazerAttack();
        ReadyforBigLazerAttack();
        chargeLazer.SetActive(false);
        bigChargeLazer.SetActive(false);
        monsterSpawner.SetActive(false);
    }

    private async void Start()
    {
        await UniTask.Delay(5000);
        isAttacking = false;
        ranIndex = Random.Range(1, 3);
    }



 //   void ReadyforLazerAttack()
 //   {
 //       lazerPool = new GameObject[3];
 //       for (int i = 0; i < lazerPool.Length; i++)
 //       {
 //           GameObject lazerAttack = Instantiate(lazer, transform.position, Quaternion.identity);
 //           lazerAttack.transform.parent = transform;
 //           lazerAttack.SetActive(false);
 //           lazerPool[i] = lazerAttack;
 //       }
	//}
    void ReadyforLazerBallAttack()
    {
        lazerBallPool = new GameObject[12];
        //thisPos = new Vector3[7];
        //thisPos[0] =  new Vector3(-5f , 6f, transform.position.z);
        //thisPos[1] = new Vector3(5f, 6f, transform.position.z);
        //thisPos[2] = new Vector3(-4f, 8f, transform.position.z);
        //thisPos[3] = new Vector3(4f, 8f, transform.position.z);
        //thisPos[4] = new Vector3(2f, 10f, transform.position.z);
        //thisPos[5] = new Vector3(-2f, 10f, transform.position.z);
        //thisPos[6] = new Vector3(0, 10.5f, transform.position.z);
        for (int i = 0; i<lazerBallPool.Length; i++)
        {
            GameObject lazerBallAttack = Instantiate(lazerBall, transform.position, Quaternion.identity);
            lazerBallAttack.transform.parent = transform;
            lazerBallAttack.SetActive(false);   
            lazerBallPool[i] = lazerBallAttack;
        }
    }
    void ReadyforBigLazerAttack()
    {
        bigLazerBallPool = new GameObject[5];
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
        if(playerHP.isPlayerDead || BossState.isBoss2Dead)
        {
            return;
        }
			if ((int)CheckRealTime.inGamerealTime == 40)
			{
				//Debug.LogError("개빡침");
				Phase2();
				isAngry = true;
				animeOn = true;
			}

			if (!isAttacking && !isAngry)
			{
				//Debug.Log("일반보스패턴");
				Boss3Pattern();
			}
			if (!isAttacking && isAngry)
			{
				//Debug.Log("빡친보스패턴");
				AngryBoss3Pattern();
			}
			if (normalMusicBox.MusicFin) //노래끝 버티기 끝
			{
				BossState.isBoss2Dead = true;
				LoadScene();
				//SceneManager.LoadScene("StageSelect");
				//DelayedDialogueCheckAsync().Forget();
			}
		
    }

    private async void LoadScene()
    {
		await SceneLoader.Instance.LoadScene("StageSelect");
	}
    //public async UniTaskVoid DelayedDialogueCheckAsync()
    //{

    //    await bossTextManager.StartStageDialogueAsync(4);
    //    await bossTextManager.StartStageDialogueAsync(5);
    //    await bossTextManager.StartStageDialogueAsync(6);
    //    await bossTextManager.StartStageDialogueAsync(7);

    //    await UniTask.Delay(1000);
    //    SceneManager.LoadScene("StageSelect");
    //}

    #region("보스빡침")
    void AngryBoss3Pattern()
    {
        for (int i = 0; ; i++)
        {
            ranIndex = Random.Range(1, 3);
            if (ranIndex != ranIndexBefore)
            {
                ranIndexBefore = ranIndex;
                break;
            }
        }
        switch (ranIndex)
        {
            case 1:
                Debug.Log("레이저볼");
                AngrysecondPattern().Forget();
                break;
            case 2:
                Debug.Log("빅레이저");
                AngryThirdPattern().Forget();
                break;
        }
    }
    //private async UniTask AngryChargeLazerAttack()
    //{
    //    isAttacking = true;
    //    chargeLazer.SetActive(true);
    //    for (int i = 1; i < 100; i++)
    //    {
    //        chargeLazer.transform.localScale =
    //            new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * i;
    //        await UniTask.Delay(15);
    //    }

    //    AngryLazerAttack().Forget();
    //}

    //private async UniTask AngryLazerAttack()
    //{
    //    anime.SetTrigger("BloodAttack");
    //    chargeLazer.SetActive(false);
    //    lazerPool[poolIndex].transform.position = new Vector3(chargeLazer.transform.position.x, chargeLazer.transform.position.y, chargeLazer.transform.position.z);
    //    lazerPool[poolIndex].SetActive(true);
    //    await UniTask.Delay(3000);
    //    AngryReturnLazer(lazerPool[poolIndex]);
    //    if (poolIndex >= lazerPool.Length - 1)
    //    {
    //        poolIndex = 0;
    //    }
    //    else
    //        poolIndex++;
    //    await UniTask.Delay(coolTime);
    //    isAttacking = false;
    //}
    //void AngryReturnLazer(GameObject lazer)
    //{
    //    lazer.SetActive(false);
    //    lazer.transform.position = transform.position;
    //}


    private async UniTask AngrysecondPattern()
    {
        isAttacking = true;
		var token = this.GetCancellationTokenOnDestroy();
		Debug.Log("레이져볼 공격중");
        //if(isAngry)
        for (int i = 0; i < 12; i++)
        {
            int poolIndex = i % lazerBallPool.Length;
            //lazerBallPool[i].transform.position = transform.position + thisPos[i];
            Vector3 randomPos = transform.position + new Vector3(Random.Range(-12f, 12f), Random.Range(5f, 12f), 0f);
			if (token.IsCancellationRequested) break;
			lazerBallPool[poolIndex].transform.position = randomPos;
            lazerBallPool[poolIndex].transform.localScale = Vector3.zero;
            lazerBallPool[poolIndex].SetActive(true);
            anime.SetTrigger("SecondPattern");
            if (!lazerBallPool[poolIndex].activeSelf)
            {
                break;
            }
            for (int j = 1; j < 80; j++)
            {
                lazerBallPool[poolIndex].transform.localScale = new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * j;
                await UniTask.Delay(2, cancellationToken: token);
            }
            await UniTask.Delay(700, cancellationToken: token);
        }
        await UniTask.Delay(coolTime + 2000, cancellationToken: token);
        isAttacking = false;
    }


    private async UniTask AngryThirdPattern()
    {
        isAttacking = true;
		monsterSpawner.SetActive(false);
		//bigChargeLazer.SetActive(true);
		RotateCameraX(-30f, 1.2f).Forget();
		//mirror.SetActive(true);
		//mirror.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
		
        //for (int i = 1; i < 250; i++)
        //{

        //    bigChargeLazer.transform.localScale =
        //        new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * i;
        //    await UniTask.Delay(10);
        //}
        //anime.SetTrigger("BloodAttack");
        await AngryBigLazerAttack();
        await UniTask.Delay(3000);
		//mirror.SetActive(false);
		RotateCameraX(0f, 0.5f).Forget();
		monsterSpawner.SetActive(true);
		isAttacking = false;
    }

    private async UniTask AngryBigLazerAttack()
    {
		//bigChargeLazer.SetActive(false);
		for (int i = 0; i<8;  i++)
        {
			int poolIndex = i % bigLazerBallPool.Length;
            Vector3 randomPos = bigChargeLazer.transform.position + new Vector3(Random.Range(-6f, 6f), Random.Range(-8f, 0f), 0f);
            bigLazerBallPool[poolIndex].transform.localScale = Vector3.zero;
			anime.SetTrigger("BloodAttackReady");
			bigLazerBallPool[poolIndex].SetActive(true);
            bigLazerBallPool[poolIndex].transform.position = randomPos;
			for (int j = 0; j < 125; j++)
			{
				bigLazerBallPool[poolIndex].transform.localScale =
					new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * j;
				await UniTask.Delay(10);
			}
            bigLazerBool = bigLazerBallPool[poolIndex].GetComponent<BigLazer>();
			//bigLazerBool.isGoing = true;
            //bigLazerBallPool[poolBigLazer].transform.position = bigChargeLazer.transform.position;
            Vector3 targetPos = new Vector3(player.position.x, player.position.y -1.5f , player.position.z);
			bigLazerBallPool[poolIndex].transform.LookAt(targetPos);
			
            //await UniTask.WaitWhile(() => bigLazerBallPool[poolIndex].activeSelf);
            await UniTask.Delay(500);
		}
        //if (poolBigLazer >= bigLazerBallPool.Length - 1)
        //{
        //    poolBigLazer = 0;
        //}
        //else
        //    poolBigLazer++;
    }
    #endregion

    #region(일반보스패턴)
    void Boss3Pattern()
    {

        switch (ranIndex)
        {
            case 1:
                Debug.Log("레이저볼");
                secondPattern().Forget();
                ranIndex = Random.Range(1, 3);
				break;
            case 2:
                Debug.Log("빅레이저");
                ThirdPattern().Forget();
                ranIndex = Random.Range(1, 3);
                break;
        }
    }
  //  private async UniTask ChargeLazerAttack()
  //  {
  //      isAttacking = true;
  //      chargeLazer.SetActive(true);
  //      for (int i = 1; i < 100; i++)
  //      {
  //          chargeLazer.transform.localScale =
  //              new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * i;
  //          await UniTask.Delay(20);
  //      }

  //      LazerAttack().Forget();
  //  }

  //  private async UniTask LazerAttack()
  //  {
		//anime.SetTrigger("BloodAttack");
		//chargeLazer.SetActive(false);
  //      lazerPool[poolIndex].transform.position = new Vector3(chargeLazer.transform.position.x, chargeLazer.transform.position.y, chargeLazer.transform.position.z);
  //      lazerPool[poolIndex].SetActive(true);
  //      await UniTask.Delay(3000);
  //      ReturnLazer(lazerPool[poolIndex]);
  //      if (poolIndex >= lazerPool.Length-1)
  //      {
  //          poolIndex = 0;
  //      }
  //      else
  //          poolIndex++;
  //      await UniTask.Delay(coolTime);
  //      isAttacking = false;
  //  }
  //  void ReturnLazer(GameObject lazer)
  //  {
  //      lazer.SetActive(false);
  //      lazer.transform.position = transform.position;
  //  }

    
    private async UniTask secondPattern()
    {
        isAttacking = true;
		var token = this.GetCancellationTokenOnDestroy();
		Debug.Log("레이져볼 공격중");
        //if(isAngry)
        for (int i = 0; i < 6; i++)
        {
            int poolIndex = i % lazerBallPool.Length;
            //lazerBallPool[i].transform.position = transform.position + thisPos[i];
            Vector3 randomPos = transform.position + new Vector3(Random.Range(-12f, 12f), Random.Range(5f, 12f), 0f);
			if (token.IsCancellationRequested) break;
			lazerBallPool[poolIndex].transform.position = randomPos;
            lazerBallPool[poolIndex].transform.localScale = Vector3.zero;
            lazerBallPool[poolIndex].SetActive(true);
            anime.SetTrigger("SecondPattern");
            if (!lazerBallPool[i].activeSelf)
            {
                break;
            }
            for (int j = 1; j < 80; j++)
            {
                lazerBallPool[i].transform.localScale = new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * j;
                await UniTask.Delay(2, cancellationToken: token);
            }
			await UniTask.Delay(1000, cancellationToken: token);
        }
		await UniTask.Delay(coolTime, cancellationToken: token);
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
        RotateCameraX(-30f, 1.2f).Forget();
		anime.SetTrigger("BloodAttackReady");
        await BigLazerAttack(); 
		await UniTask.Delay(4000);
		RotateCameraX(0f, 0.5f).Forget();
		isAttacking = false;
    }
    private async UniTask BigLazerAttack()
    {
        Vector3 randomPos = bigChargeLazer.transform.position + new Vector3(0, -3f, 0f);
        bigLazerBallPool[poolBigLazer].SetActive(true);
        bigLazerBallPool[poolBigLazer].transform.position = randomPos;
        for (int j = 0; j < 125; j++)
        {
            bigLazerBallPool[poolBigLazer].transform.localScale =
                new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * j;
            await UniTask.Delay(10);
        }
		bigLazerBallPool[poolBigLazer].transform.LookAt(player.position);
        if (poolBigLazer >= bigLazerBallPool.Length - 1)
        {
            poolBigLazer = 0;
        }
        else
            poolBigLazer++;
	}
    #endregion

    void Phase2()
    {
        if (!animeOn)
        {
			anime.Play("Jump");
		}
        monsterSpawner.SetActive(true);
        material.color = Color.red;
		coolTime = 500;
    }

	private async UniTask RotateCameraX(float targetX, float duration)
	{
		var token = this.GetCancellationTokenOnDestroy();
		if (cinemachineCamera == null) return;
		float time = 0f;
		Transform camTransform = cinemachineCamera.transform;
		Quaternion startRot = camTransform.rotation;

		// 목표 회전 (현재 Y, Z는 유지)
		Quaternion targetRot = Quaternion.Euler(targetX,
			camTransform.eulerAngles.y,
			camTransform.eulerAngles.z);

		while (time < duration)
		{
            if (cinemachineCamera == null) break;
			time += Time.deltaTime;
			float t = Mathf.Clamp01(time / duration);

			camTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);

			await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
		}

        if (cinemachineCamera != null)
        {
            camTransform.rotation = targetRot;
        }
	}
}
