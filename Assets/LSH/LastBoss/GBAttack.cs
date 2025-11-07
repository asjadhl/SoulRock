using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines.ExtrusionShapes;
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
    [SerializeField] GameObject monsterSpawner;
    [SerializeField] GameObject[] feverMonsterSpawner;
    [SerializeField] GameObject KillBall;
    [SerializeField] Animator[] cloneAnime;
    bool isAttack = false;
    float firstxPos;
    float firstyPos;
    float firstzPos;
    float firstclonexPos;
    float firstcloneyPos;
    int cooltime = 3000;
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
    [SerializeField] private DialogueUIManager dialogueUI;
    TextManager textManager;
    CancellationTokenSource cts;
    PlayerHP playerhp;
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
        playerhp = GameObject.FindWithTag("Player").GetComponent<PlayerHP>();
        cts = new CancellationTokenSource();

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
        dialogueUI = FindAnyObjectByType<DialogueUIManager>();
        textManager = FindAnyObjectByType<TextManager>();
        //bossPoss = transform.position;
    }
    private void Start()
    {
        rightBeat.SetActive(false);
        cloneAnime = new Animator[clone.Length];
        leftBeat.SetActive(false);
        originalRotation = transform.rotation; // 현재 회전 저장
        poltergeist.SetActive(false);
        monsterSpawner.SetActive(false);
        for (int i = 0; i < clone.Length; i++)
        {
			if (clone[i] == null) continue;
			clone[i].transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            cloneAnime[i] = clone[i].GetComponent<Animator>();
            clone[i].SetActive(false);
        }
        LongBitRoutine(cts.Token).Forget();
    }
    // Update is called once per frame
    void Update()
    {
        if (BossState.isBoss3Dead) return;
		if (playerhp == null || playerhp.isPlayerDead) return;
		if (!isAttack && bossMove != null && bossMove.canRun)
		{
			BossPattern().Forget();
		}
		CheckForthPattern();

		//if (normalMusicBox != null && normalMusicBox.MusicFin)
		//{
		//	// 씬 전환 시 모든 비동기 작업을 안전하게 취소합니다.
		//	if (cts != null) cts.Cancel();
		//	SceneManager.LoadScene("Ending");
		//}
        if (normalMusicBox.MusicFin) //노래끝 버티기 끝
        {
            if (cts != null) cts.Cancel();
            BossState.isBoss3Dead = true;
            dialogueUI.ShowDialogueUI(true);
            PlayBossDialojet().Forget();
            //LoadScene();
            //SceneManager.LoadScene("StageSelect");
            //DelayedDialogueCheckAsync().Forget();
        }
        switch ((int)CheckRealTime.inGamerealTime)
        {
            case 74:
                Debug.LogError("true로 바뀜");
                CircleHit.Instance.isHighLight = true;
                Debug.LogError(CircleHit.Instance.isHighLight);
                break;

            case 90:
                Debug.LogError("false 바뀜");
                CircleHit.Instance.isHighLight = false;
                Debug.LogError(CircleHit.Instance.isHighLight);
                break;

            case 148:
                Debug.LogError("true로 바뀜");
                CircleHit.Instance.isHighLight = true;
                Debug.LogError(CircleHit.Instance.isHighLight);
                break;
            case 170:
                Debug.LogError("true로 바뀜");
                CircleHit.Instance.isHighLight = false;
                Debug.LogError(CircleHit.Instance.isHighLight);
                break;
                //case int n when (n >= 90 && n < 100):
                //    Debug.Log(CircleHit.Instance.bpm);
                //    isAngry =false;
                //    isHighLight = true;
                //    break;
                //case >100:
                //    isHighLight = true;
                //    break;
        }
    }
    private async UniTask PlayBossDialojet()
    {
        await textManager.BossDialogueCheackAsync();
    }
    //public async UniTaskVoid DelayedDialogueCheckAsync()
    //{

    //    await bossTextManager.StartStageDialogueAsync(8);



    //    await UniTask.Delay(1000);
    //    SceneManager.LoadScene("Main");
    //}

    void CheckForthPattern()
    {
        if (BossState.isBoss3Dead) return;
        if (Input.GetMouseButton(2) && isBeatOn)
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
    private async UniTask BossPattern()
    {
        if (BossState.isBoss3Dead) return;
        for (int i = 0; ; i++)
            {
                patternIndex = Random.Range(0, 3);
                if (patternIndex != ranIndexBefore)
                {
                    ranIndexBefore = patternIndex;
                    break;
                }
            }
        if (!CircleHit.Instance.isHighLight)
        {
            switch (patternIndex)
            {
                case 0:
					await SoundAttack();
                    break;
                case 1:
					await Duplicate(cts.Token);
                    break;
                case 2:
					await Poltergeist(cts.Token);
                    break;
            }
        }
        if(CircleHit.Instance.isHighLight)
        {
            await FeverTime();
        }
	}
	#region("피버타임")
	private async UniTask FeverTime()
	{
		isAttack = true;
		if (BossState.isBoss3Dead || playerhp == null || musicBox == null) return;
		for (int i = 0; i < feverMonsterSpawner.Length; i++)
		{
			feverMonsterSpawner[i].SetActive(true);
		}
		while (CircleHit.Instance.isHighLight)
		{
			//if (!CircleHit.Instance.isHighLight)
			//{
			//	for (int i = 0; i < feverMonsterSpawner.Length; i++)
			//	{
			//		Debug.LogError("스포너 꺼지는중");
			//		feverMonsterSpawner[i].SetActive(false);
			//	}
			//}
			await UniTask.Delay(1000);
		}
		isAttack = false;
	}
	#endregion


	#region("일반 패턴")
	private async UniTask SoundAttack()
    {
        if (BossState.isBoss3Dead || CircleHit.Instance.isHighLight) return;
        if (playerhp == null || musicBox == null || !isActiveAndEnabled)
		{
			return; // 객체가 파괴되었거나 비활성화되면 즉시 종료
		}
		isAttack = true;
        monsterSpawner.SetActive(true);
        KillBall.SetActive(true);
        KillBall.transform.position = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);
        musicBox.panStereo = 0f;
        for (int i = 0; i < 7; i++)
        {
            if (BossState.isBoss3Dead || CircleHit.Instance.isHighLight) { isAttack = false; return; }
            teleportIndex = Random.Range(0, 2);
            animator.SetTrigger("Teleport");
            switch (teleportIndex)
            {
                case 0:
                    //musicBox.panStereo = -0.5f;
                    await SoundAttackVector(0, cts.Token);
                    break;
                case 1:
                    //musicBox.panStereo = 0.5f;
                    await SoundAttackVector(1, cts.Token);
                    break;

            }
			//musicBox.panStereo = 0f;
			if (!bossMove.canRun || cts.IsCancellationRequested) // CancellationToken 체크 추가
				break;
		}
		if (transform == null || monsterSpawner == null || KillBall == null)
		{
			return; // 객체가 파괴되었다면 여기서 즉시 종료
		}
		transform.position = new Vector3(firstxPos, firstyPos, transform.position.z);
        if(CircleHit.Instance.isHighLight)
        {
            monsterSpawner.SetActive(false);
            transform.rotation = originalRotation;
            KillBall.SetActive(false);
            isAttack = false;
            return;
        }
        monsterSpawner.SetActive(false);
        transform.rotation = originalRotation;
        KillBall.SetActive(false);
        await UniTask.Delay(2000, cancellationToken: cts.Token);
        isAttack = false;
    }

    
    private async UniTask SoundAttackVector(int patternNum,CancellationToken token)
    {
		if (transform == null) return;
		switch (patternNum)
      {
        case 0:
          transform.position = new Vector3(transform.position.x - (float)Random.Range(3, 20), transform.position.y + (float)Random.Range(0, 4), transform.position.z);
          break;
        case 1:
          transform.position = new Vector3(transform.position.x + (float)Random.Range(3, 20), transform.position.y + (float)Random.Range(0, 4), transform.position.z);
          break;
      }
      await UniTask.Delay(cooltime, cancellationToken: token);
		if (transform == null) return;
		transform.position = new Vector3(firstxPos, firstyPos, transform.position.z);
    }
	

    private async UniTask Duplicate(CancellationToken token)
    {
		if (transform == null || animator == null || clone == null || cloneTransform == null || CircleHit.Instance.isHighLight) return;
		isAttack = true;
      gameObject.tag = "RealClone";
      int teleport = Random.Range(0, cloneTransform.Length);
      for (int i = 0; i < clone.Length; i++)
      {

            if (BossState.isBoss3Dead || CircleHit.Instance.isHighLight) {isAttack = false;  return; }
            if (clone[i] == null || cloneTransform[i] == null) continue;
			clone[i].SetActive(true);
        clone[i].transform.position = new Vector3(cloneTransform[i].transform.position.x, cloneTransform[i].transform.position.y + (float)Random.Range(0, 1), transform.position.z);
        cloneAnime[i].SetTrigger("Teleport");
      }
      animator.SetTrigger("Teleport");
		if (clone[teleport] == null) return;
		clone[teleport].SetActive(false);
      transform.position = new Vector3(cloneTransform[teleport].transform.position.x, cloneTransform[teleport].transform.position.y + (float)Random.Range(0, 1), transform.position.z);
      await UniTask.Delay(cooltime + 3000, cancellationToken: token);
        if (transform == null)
            return; 
      transform.position = new Vector3(firstxPos, firstyPos, transform.position.z);
      ReturnClone();
      isAttack = false;
    
    }

    public void SuccessFindRealClone()
    {
		animator.SetTrigger("Polter");
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

    private async UniTask Poltergeist(CancellationToken token)
    {
        if (BossState.isBoss3Dead || CircleHit.Instance.isHighLight) return;
        if (poltergeist == null || poltergeistOB == null || animator == null) return;
		isAttack = true;
      poltergeist.SetActive(true);
      for (int i = 0; i < poltergeistOB.Length; i++)
      {

            if (BossState.isBoss3Dead)return;
            animator.SetTrigger("Polter");
         GameObject obj = poltergeistOB[i];
			if (obj == null) continue;
			obj.SetActive(true);
        Vector3 randomPos = transform.position + new Vector3(Random.Range(-9f, 9f), Random.Range(5f, 8f), 0f);
        obj.transform.position = randomPos;
        await UniTask.Delay(2000, cancellationToken: token);
      }

      await UniTask.Delay(cooltime + 2000, cancellationToken: token); // 모든 오브젝트가 발사된 후 대기 시간
		if (poltergeist == null) return;
		poltergeist.SetActive(false);
      isAttack = false;
    
    }
    private async UniTaskVoid LongBitRoutine(CancellationToken token)
    {   //a
        while (!token.IsCancellationRequested)
        {
            int thisran = Random.Range(10, 15);
            await UniTask.Delay(1000 * thisran, cancellationToken: token);
            //await UniTask.WaitUntil(() => !isAttack);
            await LongBit(token);
        }
    }
    public async UniTask LongBit(CancellationToken token)
    {
        if (BossState.isBoss3Dead) return;
        if (musicBox == null || rightBeat == null || leftBeat == null) return;
		ranIndex = Random.Range(0, 2);
      switch (ranIndex)
      {
        case 0:
          SoundSmooth(-1f, 4f, cts.Token).Forget();
          isBeatOn = true;
          rightBeat.SetActive(true);
          await UniTask.Delay(6000, cancellationToken: token);
          break;
        case 1:
          SoundSmooth(1f, 4f, cts.Token).Forget();
          isBeatOn = true;
          leftBeat.SetActive(true);
          await UniTask.Delay(6000, cancellationToken: token);
          break;
      }
		if (musicBox == null || rightBeat == null || leftBeat == null) return;
		CheckSuccess();
      isBeatOn = false;
      rightBeat.SetActive(false);
      leftBeat.SetActive(false);
      barAmount = 0f;
      isSuccess = false;
      musicBox.panStereo = 0f;
    }
    
    public async UniTask SoundSmooth(float stereo, float duration,CancellationToken token)
    {
		if (musicBox == null) return;
		float elapsed = 0f;
      while (elapsed < duration && !token.IsCancellationRequested)
      {
        elapsed += Time.deltaTime;
        if (musicBox != null)
          musicBox.panStereo = Mathf.Lerp(0, stereo, elapsed / duration);

        await UniTask.Yield(cancellationToken: token);
      }
		if (musicBox == null) return;
		if (!token.IsCancellationRequested)
		{
			musicBox.panStereo = stereo;
		}
	}



	private void OnDestroy()
	{
		// OnDestroy에서 CancellationTokenSource를 잘 처리하고 계십니다.
		if (cts != null)
		{
			cts.Cancel();
			cts.Dispose();
		}
	}
    #endregion
}