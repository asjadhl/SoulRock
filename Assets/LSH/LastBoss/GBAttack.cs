using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class GBAttack : MonoBehaviour
{
    /*
     * 보스 노래들 시간 구해서 끝나는대로 다음 씬으로 넘어가게 만들기.
     * 클리어 할때 지우의 BossHp스크립트로 true만들어서 현민이의 대기실 박살.
     * 올 True면 엔딩.
     * */
    AudioSource musicBox;
    NormalMusicBox normalMusicBox;
    Animator animator;
    private ParticleManager particleManager;
    int teleportIndex;
    int patternIndex;
    [SerializeField] GameObject[] clone;
    [SerializeField] GameObject[] cloneTransform;
    [SerializeField] GameObject mosterSpawner;
    [SerializeField] GameObject KillBall;
    bool isAttack = false;
    float firstxPos;
    float firstyPos;
    float firstclonexPos;
    float firstcloneyPos;
    int cooltime = 1000;
    BossMove bossMove;
    [Header("폴터가이스트 현상")]
    [SerializeField] GameObject poltergeist;
    [SerializeField] GameObject[] poltergeistOB;
    bool cloneMakeGhost = false;
    [Header("타겟 (플레이어)")]
	public Transform player;

    [Header("4번째 패턴")]
    [SerializeField] GameObject rightBeat;
    [SerializeField] GameObject leftBeat;
    [SerializeField] Image rightLongBeat;
    [SerializeField] Image leftLongBeat;

    float barAmount = 0;
    int ranIndex = 0;
    int ranIndexBefore = 0;
    bool isBeatOn = false;
    bool isSuccess = false;
    //[SerializeField] Image leftLongBeat; 

    ////분신패턴
    //[SerializeField] private int clonePoolSize = 4;
    //[SerializeField] private GameObject ghostClonePrefab;
    //private GameObject[] clonePool;
    //private bool[] cloneUsed;
    //int cloneCount = 4;
    //float spacing = 3f;//간격
    //Vector3 bossPoss;
    Quaternion originalRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
		particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
		musicBox = GameObject.FindWithTag("MusicBox").GetComponent<AudioSource>();
        normalMusicBox = GameObject.FindWithTag("MusicBox").GetComponent<NormalMusicBox>();
        bossMove = GetComponent<BossMove>();
        animator = GetComponent<Animator>();
		if (player == null)
			player = GameObject.FindWithTag("Player").transform;
		teleportIndex = 0;
        patternIndex = 0;
        firstxPos = transform.position.x;
        firstyPos = transform.position.y;
		//bossPoss = transform.position;
	}
    private void Start()
    {
        ////클론만들기~
        //clonePool = new GameObject[clonePoolSize];
        //cloneUsed = new bool[clonePoolSize];
        //for (int i = 0; i < clonePoolSize; i++)
        //{
        //    clonePool[i] = Instantiate(ghostClonePrefab);
        //    clonePool[i].SetActive(false);
        //    cloneUsed[i] = false;
        //}

        rightBeat.SetActive(false);
        leftBeat.SetActive(false);
        originalRotation = transform.rotation; // 현재 회전 저장
        poltergeist.SetActive(false);
        mosterSpawner.SetActive(false);
        for (int i = 0; i < clone.Length; i++)
        {
            clone[i].transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            clone[i].SetActive(false);
        }
        LongBitRoutine().Forget();
    }
    // Update is called once per frame
    void Update()
    {
        if (!isAttack && bossMove.canRun)
        {
            BossPattern().Forget();
        }
        CheckForthPattern();
        //StuckWithPlayer();
        if(normalMusicBox.MusicFin)
        {
            //여기에 노래끝났을때.
        }
    }

    void CheckForthPattern()
    {
        if (Input.GetMouseButton(2)&&isBeatOn)
        {
            barAmount += 0.4f;
            rightLongBeat.fillAmount = barAmount / 100f;
            if (rightLongBeat.fillAmount >= 1f)
            {
                isSuccess = true;
                musicBox.panStereo = 0f;
                rightBeat.SetActive(false);
            }
        }
       if (Input.GetMouseButtonUp(2))
       {
            barAmount = 0;
            rightLongBeat.fillAmount = barAmount / 100f;
       }
       if (Input.GetMouseButton(1) && isBeatOn)
       {
            barAmount += 0.4f;
            leftLongBeat.fillAmount = barAmount / 100f;
            if (leftLongBeat.fillAmount >= 1f)
            {
                isSuccess = true;
                musicBox.panStereo = 0f;
                leftBeat.SetActive(false);
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            barAmount = 0;
            leftLongBeat.fillAmount = barAmount / 100f;
        }
        
    }

    void CheckSuccess()
    {
        if (!isSuccess)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus().Forget();
        }
    }
	//void StuckWithPlayer()
	//{
	//	float distance = Vector3.Distance(transform.position, player.position);
 //       Debug.LogError(distance);
	//	if (distance <= disableDistance)
	//	{
 //           transform.position = new Vector3(firstxPos, firstyPos, transform.position.z);
	//		bossMove.canRun = false;
	//		transform.SetParent(player.transform, true);
	//	}
	//}
	private async UniTask BossPattern()
    { 
        for (int i = 0; ; i++)
        {
            patternIndex = Random.Range(0, 3);
            if (patternIndex != ranIndexBefore)
            {
                ranIndexBefore = patternIndex;
                break;
            }
        }
        switch (patternIndex)
        {
            case 0:
                await SoundAttack();
                break;
            case 1:
                await Duplicate();
                break;
            case 2:
                await Poltergeist();
                break;
        }
        
    }
    
    private async UniTask SoundAttack()
    {
        isAttack = true;
        mosterSpawner.SetActive(true);
        KillBall.SetActive(true);
        KillBall.transform.position = new Vector3(transform.position.x, transform.position.y+3f, transform.position.z);
        musicBox.panStereo = 0f;
        for (int i = 0; i < 10; i++)
        {
            teleportIndex = Random.Range(0, 2);
            animator.SetTrigger("Teleport");
            switch (teleportIndex)
            {
                case 0:
                    //musicBox.panStereo = -0.5f;
                    await SoundAttackVector(0);
                    break;
                case 1:
                    //musicBox.panStereo = 0.5f;
                    await SoundAttackVector(1);
                    break;
                 
            }
            //musicBox.panStereo = 0f;
            if(!bossMove.canRun)
                break;
        }
		transform.position = new Vector3(firstxPos, firstyPos, transform.position.z);
        mosterSpawner.SetActive(false);
        transform.rotation = originalRotation;
        KillBall.SetActive(false);
        await UniTask.Delay(2000);
        isAttack = false;
    }
    private async UniTask SoundAttackVector(int patternNum)
    {
        switch (patternNum)
        {
            case 0:
                transform.position = new Vector3(transform.position.x - (float)Random.Range(3, 20), transform.position.y + (float)Random.Range(0, 4), transform.position.z);
                break;
            case 1:
                transform.position = new Vector3(transform.position.x + (float)Random.Range(3, 20), transform.position.y + (float)Random.Range(0, 4), transform.position.z);
                break;
        }
		await UniTask.Delay(cooltime);
		transform.position = new Vector3(firstxPos, firstyPos, transform.position.z);
	}

    private async UniTask Duplicate()
    {
        isAttack = true;
        gameObject.tag = "RealClone";
        int teleport = Random.Range(0, cloneTransform.Length);
        for (int i = 0; i < clone.Length; i++)
        {
            clone[i].SetActive(true);
            clone[i].transform.position = new Vector3(cloneTransform[i].transform.position.x, cloneTransform[i].transform.position.y + (float)Random.Range(0, 1), transform.position.z);
        }
        clone[teleport].SetActive(false);
        transform.position = new Vector3(cloneTransform[teleport].transform.position.x, cloneTransform[teleport].transform.position.y + (float)Random.Range(0, 1), transform.position.z);
        await UniTask.Delay(cooltime + 3000);
        transform.position = new Vector3(firstxPos, firstyPos, transform.position.z);
		ReturnClone();
        isAttack = false;
    }

    public void SuccessFindRealClone()
    {
		animator.SetTrigger("Teleport");
	}

    public void ReturnClone()
    {
		gameObject.tag = "GhostBoss";
		for (int i = 0; i < clone.Length; i++)
        {
            if (clone[i].activeSelf)
            {
				Vector3 effectPos = clone[i].transform.position;
				particleManager.PlayGhostEffect(effectPos);
				clone[i].SetActive(false);
			}
			//clone[i].transform.position = new Vector3(firstclonexPos, firstcloneyPos, transform.position.z);
		}
    }

    private async UniTask Poltergeist()
    {
        isAttack = true;
        poltergeist.SetActive(true);
        for (int i = 0; i < poltergeistOB.Length; i++)
        {
            animator.SetTrigger("Polter");
            GameObject obj = poltergeistOB[i];
            obj.SetActive(true);
            Vector3 randomPos = transform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(5f, 15f), 0f);
            obj.transform.position = randomPos;
            await UniTask.Delay(1000);
        }

        await UniTask.Delay(cooltime+2000); // 모든 오브젝트가 발사된 후 대기 시간
        poltergeist.SetActive(false);
        isAttack = false;
    }
    private async UniTaskVoid LongBitRoutine()
    {
        while (true)
        {
            int thisran = Random.Range(10, 15);
            await UniTask.Delay(1000 * thisran);
            //await UniTask.WaitUntil(() => !isAttack);
            await LongBit();
        }
    }
    public async UniTask LongBit()
    {
        ranIndex = Random.Range(0, 2);
        switch(ranIndex)
        {
            case 0:
                SoundSmooth(-1f, 2.5f).Forget();
                isBeatOn = true;
				rightBeat.SetActive(true);
                await UniTask.Delay(5000);
                break;
            case 1:
                SoundSmooth(1f, 2.5f).Forget();
                isBeatOn = true;
				leftBeat.SetActive(true);
				await UniTask.Delay(5000);
				break;
        }
        CheckSuccess();
        isBeatOn = false;
        rightBeat.SetActive(false);
        leftBeat.SetActive(false);
        barAmount = 0f;
        isSuccess = false;
        musicBox.panStereo = 0f;
	}
    public async UniTask SoundSmooth(float stereo, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicBox.panStereo = Mathf.Lerp(0, stereo, elapsed / duration);
            await UniTask.Yield();
        }
        musicBox.panStereo = stereo;
    }

}