using Cysharp.Threading.Tasks;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;


public class GBAttack : MonoBehaviour
{
    /*
     * 보스 움직임 직선적 말고 플레이어를 향해 돌진하는 거였음 좋겠음.
     * 
     * */
    AudioSource musicBox;
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
    int cooltime = 2000;
    BossMove bossMove;
    [Header("폹터가이스트 현상")]
    [SerializeField] GameObject poltergeist;
    [SerializeField] GameObject[] poltergeistOB;
    bool cloneMakeGhost = false;
    [Header("타겟 (플레이어)")]
	public Transform player;
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
        originalRotation = transform.rotation; // 현재 회전 저장
        poltergeist.SetActive(false);
        mosterSpawner.SetActive(false);
        for (int i = 0; i < clone.Length; i++)
        {
            clone[i].transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            firstclonexPos = clone[i].transform.position.x;
            firstcloneyPos = clone[i].transform.position.y;
            clone[i].SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isAttack && bossMove.canRun)
        {
            _ = BossPattern();
        }
		//StuckWithPlayer();
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
        patternIndex = Random.Range(0, 3);
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
                    musicBox.panStereo = -0.5f;
                    await SoundAttackVector(0);
                    break;
                case 1:
                    musicBox.panStereo = 0.5f;
                    await SoundAttackVector(1);
                    break;
                 
            }
            musicBox.panStereo = 0f;
            if(!bossMove.canRun)
                break;
        }
		transform.position = new Vector3(firstxPos, firstyPos, transform.position.z);
        mosterSpawner.SetActive(false);
        transform.rotation = originalRotation;
        KillBall.SetActive(false);
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
        for (int i = 0; i < clone.Length; i++)
        {
			gameObject.tag = "GhostBoss";
			Vector3 effectPos = clone[i].transform.position;
			particleManager.PlayGhostEffect(effectPos);
			clone[i].transform.position = new Vector3(firstclonexPos, firstcloneyPos, transform.position.z);

			clone[i].SetActive(false);
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
    //private async UniTask Duplicate()
    //{
    //    isAttack = true;
    //    int realBossIndex = Random.Range(0, cloneCount+ 1);

    //    GameObject[] activeClones = new GameObject[cloneCount];
    //    for (int i = 0; i < cloneCount; i++)
    //    {
    //        GameObject clone = GetCloneFromPool();
    //        if (clone != null)
    //        {
    //            float offsetX = (i - cloneCount / 2) * spacing;
    //            clone.transform.position = bossPoss + new Vector3((i - cloneCount/2) * spacing, 0, 0);
    //            clone.GetComponent<GBAttack>().enabled = false;
    //            clone.GetComponent<LastBossMove>().enabled = false;
    //            activeClones[i] = clone;
    //        }
    //    }
    //    transform.position = bossPoss + new Vector3((realBossIndex - cloneCount / 2) * spacing, 0, 0);
    //    await UniTask.Delay(cooltime+3000);
    //    for(int i = 0; i < cloneCount; i++)
    //    {
    //        if (activeClones[i] != null)
    //        {
    //            ReturnCloneToPool(activeClones[i]);
    //        }
    //    }
    //    isAttack = false;
    //}

    //private GameObject GetCloneFromPool()
    //{
    //    for (int i = 0; i < clonePoolSize; i++)
    //    {
    //        if (!cloneUsed[i])
    //        {
    //            cloneUsed[i] = true;
    //            clonePool[i].SetActive(true);
    //            return clonePool[i];
    //        }
    //    }
    //    return null;
    //}
    //private void ReturnCloneToPool(GameObject clone)
    //{
    //    for (int i = 0; i < clonePoolSize; i++)
    //    {
    //        if (clonePool[i] == clone)
    //        {
    //            cloneUsed[i] = false;
    //            clone.SetActive(false);
    //            return;
    //        }
    //    }
    //}
}
