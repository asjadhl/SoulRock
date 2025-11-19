using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SoulRock : MonoBehaviour
{
	// 박자
	#region 클릭 판정 및 도트풀링
	public static CircleHit Instance { get; private set; }
	[Header("타겟 이미지")]
	[SerializeField] Transform targetImage;

	[Header("원크기")]
	public float circleBig = 11f;
	[Header("BPM (박자 속도)")]
	public double bpm = 120.0;

	[Header("판정 거리")]
	[SerializeField] public float minDis = 0.01f;
	[SerializeField] public float exDis = 70f;
	[SerializeField] public float maxDis = 120f;

	[Header("원 도트 프리팹")]
	[SerializeField] GameObject CirclePrefab;

	[Header("풀 사이즈")]
	[SerializeField] int poolSize = 15;

	[Header("Combo")]
	public int combo = 0;
	[SerializeField] GameObject comboText;

	[SerializeField] TextMeshProUGUI text;
	[SerializeField] TextMeshProUGUI comboNumText;
	[SerializeField] GameObject[] feverImage;
	AudioSource audioA;
	[SerializeField] AudioClip clip;
	[SerializeField] PlayerShoot playerShoot;
	private PlayerHP playerHPSc;
	public GameObject[] poolCircle;
	private int pivot = 0;
	private double secondsPerBeat;
	private List<CircleMove> activeCircles = new List<CircleMove>();

	CanvasGroup cg;
	Color randomColor;
	public Color feverColor;

	double firstBpm = 0;
	Image image;
	Color originalColor;
	private bool isRunning = true;
	public bool getDamage = false;
	public bool isHighLight = false; //피버용
	public bool isScale = false; //클릭시 커짐

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		poolCircle = new GameObject[poolSize];
		for (int i = 0; i < poolSize; i++)
		{
			GameObject circleDot = Instantiate(CirclePrefab, transform);
			circleDot.SetActive(false);
			poolCircle[i] = circleDot;
		}
		for (int i = 0; i < feverImage.Length; i++)
		{
			feverImage[i].SetActive(false);
		}
		secondsPerBeat = 60.0 / bpm;
		firstBpm = secondsPerBeat;
		cg = comboText.GetComponent<CanvasGroup>();
		image = GetComponent<Image>();
		originalColor = image.color;
	}

	private void Start()
	{
		audioA = GetComponent<AudioSource>();
		playerShoot = FindAnyObjectByType<PlayerShoot>();
		playerHPSc = FindAnyObjectByType<PlayerHP>();
		CircleGen().Forget();
	}

	private void Update()
	{
		CheckCol();
		CheckFever();
	}

	/*================================================
	 * 역할: 피버타임 확인하는 함수
	 *================================================ */

	void CheckFever()
	{
		switch (isHighLight)
		{
			case true:
				maxDis = 80f;
				bpm = 170;
				secondsPerBeat = 60.0 / bpm;
				for (int i = 0; i < feverImage.Length; i++)
				{
					feverImage[i].SetActive(true);
				}
				break;
			case false:
				maxDis = 140f;
				secondsPerBeat = firstBpm;
				for (int i = 0; i < feverImage.Length; i++)
				{
					feverImage[i].SetActive(false);
				}
				break;
		}
	}

	/*================================================
	 * 역할: 콤보 색상을 매번 변경 시키는 함수
	 *================================================ */

	private void RanTextColor()
	{
		randomColor = new Color(Random.value, Random.value, Random.value);
		comboNumText.color = randomColor;
	}
	#region 원풀링
	public GameObject GetCircle()
	{
		if (poolCircle.Length == 0)
		{
			return null;
		}

		GameObject circleDot = poolCircle[pivot];
		circleDot.transform.localScale = Vector3.one * circleBig;
		circleDot.transform.position = transform.position;
		circleDot.GetComponent<CircleMove>().Initialize(this);

		if (getDamage)
			circleDot.GetComponent<CircleMove>().ChangeColor().Forget();
		else
			circleDot.GetComponent<CircleMove>().SetColor();
		if (isHighLight)
		{
			image.color = feverColor;
			circleDot.GetComponent<CircleMove>().FeverTime();
		}
		else
		{
			image.color = originalColor;
			circleDot.GetComponent<CircleMove>().FeverTimeFIn();
		}

		circleDot.SetActive(true);

		activeCircles.Add(circleDot.GetComponent<CircleMove>());
		pivot = (pivot + 1) % poolCircle.Length;
		return circleDot;
	}
	public void ReturnCircle(GameObject circleDot)
	{
		if (circleDot == null || !circleDot.activeSelf) return;
		circleDot.GetComponent<CircleMove>().SetColor();
		circleDot.SetActive(false);
		circleDot.transform.localScale = Vector3.one * circleBig;
	}
	private async UniTask CircleGen()
	{
		while (isRunning && this != null && gameObject != null)
		{
			await UniTask.Delay((int)(secondsPerBeat * 1000.0)); //여기서 생성시간
			Debug.Log((int)(secondsPerBeat * 1000.0));
			if (!isRunning || this == null || gameObject == null)
				break;

			var circle = GetCircle();
			if (circle != null)
				circle.transform.position = transform.position;
		}
	}
	#endregion

	#region 클릭판정

	/*================================================
	 * 역할: 클릭 했을때 거리 체크
	 *================================================ */
	void CheckCol()
	{
		if (Input.GetMouseButtonDown(0) && !playerHPSc.isPlayerDead)
		{
			for (int i = activeCircles.Count - 1; i >= 0; i--)
			{
				var circle = activeCircles[i];
				float distance = Vector2.Distance(targetImage.position, circle.hitRect.position);

				if (minDis <= distance && exDis >= distance && !isHighLight)
				{
					OnClickSuccessEx().Forget();
					comboNumText.text = combo.ToString();
					ReturnCircle(circle.gameObject);
					activeCircles.RemoveAt(i);
				}
				else if (exDis < distance && maxDis >= distance && !isHighLight)
				{
					OnClickSuccess().Forget();
					comboNumText.text = combo.ToString();
					ReturnCircle(circle.gameObject);
					activeCircles.RemoveAt(i);
				}
				else if (minDis > distance || maxDis < distance && !isHighLight)
				{
					OnClickSuccessBad().Forget();
					comboNumText.text = combo.ToString();
					ReturnCircle(circle.gameObject);
					activeCircles.RemoveAt(i);
				}
				else if (isHighLight)
				{
					OnClickSuccessEx().Forget();
					comboNumText.text = combo.ToString();
				}
			}
		}
	}
	/*================================================
	 * 역할: Good 판정
	 *================================================ */
	public async UniTask OnClickSuccess()
	{
		if (!isHighLight)
		{
			combo++;
		}
		RanTextColor();
		text.text = "Good";
		isScale = true;
		cg.alpha = 1;
		playerShoot.PlayerShoot_();
		audioA.PlayOneShot(clip);
		playerHPSc.PlayerHPPlus(2);
		await UniTask.Delay(100);
		isScale = false;
		await UniTask.Delay(300);
		cg.alpha = 0;
	}
	/*================================================
	 * 역할: 완벽 판정
	 *================================================ */
	public async UniTask OnClickSuccessEx()
	{
		combo++;
		RanTextColor();
		text.text = "Perfect";
		isScale = true;
		cg.alpha = 1;
		audioA.PlayOneShot(clip);
		playerShoot.PlayerShoot_();
		playerHPSc.PlayerHPPlus(4);
		await UniTask.Delay(150);
		isScale = false;
		await UniTask.Delay(350);
		cg.alpha = 0;
	}
	/*================================================
	 * 역할: 실패 판정
	 *================================================ */
	public async UniTask OnClickSuccessBad()
	{
		combo = 0;
		RanTextColor();
		text.text = "Bad";
		isScale = true;
		cg.alpha = 1;
		await UniTask.Delay(150);
		isScale = false;
		await UniTask.Delay(350);
		cg.alpha = 0;
	}
	#endregion

	private void OnDisable()
	{
		isRunning = false;
	}

	private void OnDestroy()
	{
		isRunning = false;
	}
	#endregion
	// ====================== 스테이지 1 보스 ======================
	public class Stage1BossAttack : MonoBehaviour
    {
        private static int _clubStack = 0;
        public static int clubStack
        {
            get => _clubStack;
            set => _clubStack = Mathf.Clamp(value, 0, 7);
        }
        int reMiniH;
        private bool bossRecover = false;

        [SerializeField] Card[] cards;
        Card currentCard;

        [SerializeField] Image bossCardImage;
        public Shape curShape;

        GameObject player;

        private float teleportTimer = 0;
        private int teleportCount = 0;
        public int playerHitCount = 0;
        private bool miniBossSpawned = false;

        [SerializeField] GameObject[] miniBoss;
        public Transform[] spawnPos;
        public List<int> usedPos;

        [SerializeField] GameObject[] spadeCards;

        private BossHP BossHP;

        private bool isDelay = false;

        private bool isHAttacking = false;
        private bool miniHeartTrue = false;

        [SerializeField] GameObject spinWheel;
        [SerializeField] GameObject spinCircle;
        private float spinSpeed = 10f;
        private bool isCAttacking = false;
        bool isAttack = false;
        public bool wheelStop = true;
        private int index = 0;
        [SerializeField] GameObject[] clubStackImage;
        NormalMusicBox normalMusicBox;
        private bool bossDialogueTriggered = false;
        [SerializeField] private DialogueUIManager dialogueUI;

        public bool isDAttacking = false;

        PlayerHP playerHP;

        private void Start()
        {
            clubStack = 0;
            ComboSave.Instance.maxComboData.maxCombo = 0;

            for (int i = 0; i < clubStackImage.Length; i++)
                clubStackImage[i].SetActive(false);


            player = GameObject.FindWithTag("Player");
            normalMusicBox = GameObject.FindWithTag("MusicBox").GetComponent<NormalMusicBox>();
            BossHP = GetComponent<BossHP>();
            playerHP = player.GetComponent<PlayerHP>();
            if (isDelay == false)
                StartDelay().Forget();
            dialogueUI = FindAnyObjectByType<DialogueUIManager>();
        }
        private void FixedUpdate()
        {
            ClubStackUIUpdate();
            if (!isDelay) return;
            if (!isAttack)
            {
                BossPattern();
            }
            if (normalMusicBox.MusicFin && !bossDialogueTriggered)
            {
                bossDialogueTriggered = true;
                BossState.isBoss1Dead = true;
                dialogueUI.ShowDialogueUI(true);
            }
        }

        // 보스패턴 전체 관리
        private void BossPattern()
        {
            if (BossState.isBoss1Dead == false)
            {
                switch (curShape)
                {
                    case Shape.H:
                        if (!miniBossSpawned && !isHAttacking)
                        {
                            isHAttacking = true;
                            HAttack().Forget();
                        }
                        break;

                    case Shape.S:
                        SAttack().Forget();

                        break;

                    case Shape.D:
                        teleportTimer += Time.fixedDeltaTime;
                        DAttack().Forget();
                        break;
                    case Shape.C:
                        CAttack().Forget();
                        break;

                }
            }
        }

        // ====================== 카드데이터 받아오기 ======================
        private void SetCardData()
        {
            bossCardImage.sprite = currentCard.icon;
            curShape = currentCard.shape;
        }

        // ====================== 다음 카드로 바꾸기 ======================
        private async UniTask ChangeNextRanCard()
        {
            var token = this.GetCancellationTokenOnDestroy();

            await RollCardEffect(rollCount: 12, delay: 80);

            Card nextCard;
            do
            {
                nextCard = cards[Random.Range(0, cards.Length)];
            }
            while (nextCard == currentCard && !token.IsCancellationRequested);

            if (!token.IsCancellationRequested)
            {
                currentCard = nextCard;
                SetCardData();
            }

        }

        // ====================== 카드 바뀌는 이펙트 ======================
        private async UniTask RollCardEffect(int rollCount = 10, int delay = 100)
        {
            var token = this.GetCancellationTokenOnDestroy();
            for (int i = 0; i < rollCount; i++)
            {
                if (!token.IsCancellationRequested)
                {
                    var randomCard = cards[Random.Range(0, cards.Length)];
                    bossCardImage.sprite = randomCard.icon; // UI에만 반영
                    bossCardImage.transform.localScale = Vector3.one * 1.2f;
                    await UniTask.Delay(delay / 2);
                    bossCardImage.transform.localScale = Vector3.one;
                    await UniTask.Delay(delay / 2);
                }
                else
                {
                    break;
                }
            }
        }

        // ====================== 하트 패턴 ======================
        private async UniTask HAttack()
        {
            isAttack = true;
            await UniTask.Delay(500);
            reMiniH = clubStack + 4;
            for (int i = 0; i < clubStack + 4; i++)
            {
                miniBoss[i].SetActive(true);
            }
            miniBossSpawned = true;
            await UniTask.Delay(10000);
            usedPos.Clear();

            checkFindTure();

            for (int i = 0; i < clubStack + 4; i++)
            {
                if (miniBoss[i].activeSelf == true)
                {
                    miniBoss[i].GetComponent<MiniBoss>().ReturnOriPos().Forget();
                    miniBoss[i].SetActive(false);
                }
            }
            miniBossSpawned = false;

            isHAttacking = false;

            if (miniHeartTrue == false && isHAttacking == false)
            {
                player.GetComponent<PlayerHP>().PlayerHPMinus().Forget();
            }

            await ChangeNextRanCard();
            miniHeartTrue = false;
            isAttack = false;
        }


        // ====================== 스페이드 패턴 ======================
        private async UniTask SAttack()
        {
            isAttack = true;
            Debug.Log("스페이드");
            for (int i = 0; i < 6; i++)
            {
                GoldOrRedCardOn();
                await UniTask.Delay(1500);
            }
            await ChangeNextRanCard();
            isAttack = false;
        }

        // ====================== 다이아 패턴 ======================
        private async UniTask DAttack()
        {
            isAttack = true;
            isDAttacking = true;
            if (teleportCount < 5)
            {
                if (teleportTimer >= 2)
                {
                    MoveToRanPos();
                    teleportTimer = 0;
                    isDAttacking = false;
                }
            }
            else
            {
                if (playerHitCount < 5)
                {
                    BossRush();
                }
                else
                {
                    transform.position = new Vector3(0, 0, player.transform.position.z + 17);
                    await ChangeNextRanCard();
                    playerHitCount = 0;
                    teleportTimer = 0;
                    teleportCount = 0;
                    isDAttacking = false;
                }

            }
            isAttack = false;
        }

        // ====================== 클로버 패턴 ======================
        private async UniTask CAttack()
        {
            isAttack = true;
            isCAttacking = true;
            spinWheel.SetActive(true);
            await SpinWheel();

            int result = spinWheel.GetComponentInChildren<Roulette>().GetNum();
            Debug.Log(result);
            clubStack += result;
            if (clubStack >= 7)
            {
                playerHP.PlayerHPMinus().Forget();
                clubStack = 0;
            }

            await ChangeNextRanCard();

            isCAttacking = false;
            spinWheel.SetActive(false);
            isAttack = false;
            wheelStop = true;
        }

        // ====================== 다이아 패턴 관련 함수 ======================
        private void MoveToRanPos()
        {
            Vector3 currentPos = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            Stage1ParticleManager.Instance.PlayClownEffect(currentPos);

            int x = Random.Range(-8, 9);
            int z = (int)player.transform.position.z + Random.Range(10, 20);

            transform.position = new Vector3(x, 0, z);

            teleportCount++;
        }

        private void BossRush()
        {
            transform.LookAt(player.transform);
            transform.Translate(Vector3.forward * 30 * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                player.GetComponent<PlayerHP>().PlayerHPMinus().Forget();
                ChangeNextRanCard().Forget();
                transform.rotation = Quaternion.Euler(0, 180, 0);
                transform.position = new Vector3(0, 0, player.transform.position.z + 17);
                teleportCount = 0;
                playerHitCount = 0;
                teleportTimer = 0;
                isDAttacking = false;
            }
        }

        // ====================== 하트 패턴 관련 함수 ======================
        public int SetMiniBossRanPos()
        {
            int index = Random.Range(0, spawnPos.Length);
            return index;
        }

        public void AddList(int a)
        {
            usedPos.Add(a);
        }

        public void HeartTrue()
        {
            miniHeartTrue = true;
        }

        // ====================== 스페이드 패턴 관련 함수 ======================
        private void GoldOrRedCardOn()
        {
            int ranIndex = Random.Range(0, spadeCards.Length);

            spadeCards[ranIndex].SetActive(true);
        }

        private void checkFindTure()
        {
            if (miniBoss[0].activeSelf == false)
            {
                miniBoss[0].GetComponent<MiniBoss>().ReturnOriPos().Forget();
                player.GetComponent<PlayerHP>().PlayerHPMinus().Forget();
            }
            if (miniBoss[0].activeSelf == true)
            {
                miniBoss[0].GetComponent<MiniBoss>().miniHTureReturnOriState();
            }
        }

        // ====================== 클로버 패턴 관련 함수 ======================
        private async UniTask SpinWheel()
        {
            wheelStop = false;
            int randomSpin = Random.Range(800, 1300);
            for (int i = 0; i < randomSpin; i++)
            {
                if (wheelStop == true)
                {
                    break;
                }
                spinCircle.transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime * 60); // 속도 보정
                await UniTask.Yield(PlayerLoopTiming.Update); // 프레임마다 체크
            }
        }

        public void WheelStop()
        {
            wheelStop = true;
        }

        private void ClubStackUIUpdate()
        {
            for (int i = 0; i < clubStackImage.Length; i++)
                clubStackImage[i].SetActive(i < Stage2BossAttack.clubStack);
        }

        // ====================== 시작 전 딜레이 ======================
        private async UniTask StartDelay()
        {
            await UniTask.Delay(3500);
            isDelay = true;
            await ChangeNextRanCard();
        }
    }
	// ====================== 스테이지 2,3 보스 ======================
	#region 스켈레톤과 귀신
	#region 스켈레톤 보스
	public class Stage3Boss : MonoBehaviour
	{
		[Header("LazerChargeOB")]
		[SerializeField] GameObject chargeLazer;
		[Header("LazerBallOB")]
		[SerializeField] GameObject lazerBall;
		[Header("BigChargeLazerOB")]
		[SerializeField] GameObject bigChargeLazer;
		[Header("BigLazerOB")]
		[SerializeField] GameObject bigLazer;
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
		[Header("Renderer")]
		[SerializeField] Material material;
		int ranIndex = 0;
		int ranIndexBefore = 0;
		int poolIndex = 0;
		int poolBigLazer = 0;
		bool isAttacking = true;
		bool isAngry = false;
		bool animeOn = false;
		[SerializeField] GameObject monsterSpawner;
		NormalMusicBox normalMusicBox;
		[SerializeField] private DialogueUIManager dialogueUI;
		TextManager textManager;

		private bool bossDialogueTriggered = false;

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
			dialogueUI = FindAnyObjectByType<DialogueUIManager>();
			textManager = FindAnyObjectByType<TextManager>();
		}

		private async void Start()
		{
			await UniTask.Delay(5000);
			isAttacking = false;
			ranIndex = Random.Range(1, 3);
		}
		/*================================================
	 * 역할: 보스의 공격 오브젝트 풀링 
	 *================================================ */
		void ReadyforLazerBallAttack()
		{
			lazerBallPool = new GameObject[20];
			for (int i = 0; i < lazerBallPool.Length; i++)
			{
				GameObject lazerBallAttack = Instantiate(lazerBall, transform.position, Quaternion.identity);
				lazerBallAttack.transform.parent = transform;
				lazerBallAttack.SetActive(false);
				lazerBallPool[i] = lazerBallAttack;
			}
		}
		void ReadyforBigLazerAttack()
		{
			bigLazerBallPool = new GameObject[15];
			for (int i = 0; i < bigLazerBallPool.Length; i++)
			{
				GameObject biglazerAttack = Instantiate(bigLazer, bigChargeLazer.transform.position, Quaternion.identity);
				biglazerAttack.transform.parent = transform;
				biglazerAttack.SetActive(false);
				bigLazerBallPool[i] = biglazerAttack;
			}
		}

		void Update()
		{
			if (playerHP == null || playerHP.isPlayerDead || BossState.isBoss2Dead) return;
			switch ((int)CheckRealTime.inGamerealTime)
			{
				case 34:
					Phase2();
					isAngry = true;
					animeOn = true;
					break;

				case 53:
					CircleHit.Instance.isHighLight = true;
					Debug.LogError(CircleHit.Instance.isHighLight);
					break;

				case 75:
					CircleHit.Instance.isHighLight = false;
					Debug.LogError(CircleHit.Instance.isHighLight);
					break;

				case 129:
					CircleHit.Instance.isHighLight = true;
					Debug.LogError(CircleHit.Instance.isHighLight);
					break;
			}

			if (!isAttacking && !isAngry)
			{
				Boss3Pattern();
			}
			if (!isAttacking && isAngry)
			{
				AngryBoss3Pattern();
			}
			if (normalMusicBox.MusicFin && !bossDialogueTriggered) //노래끝 버티기 끝
			{
				bossDialogueTriggered = true;
				BossState.isBoss2Dead = true;
				dialogueUI.ShowDialogueUI(true);
				PlayBossDialojet().Forget();
			}
		}

		private async UniTask PlayBossDialojet()
		{
			await textManager.BossDialogueCheackAsync();
		}

		#region(보스화남)
		void AngryBoss3Pattern()
		{
			if (CircleHit.Instance.isHighLight)
			{
				ranIndex = 2;
			}
			else
				ranIndex = 1;
			switch (ranIndex)
			{
				case 1:
					AngrysecondPattern().Forget();
					break;
				case 2:
					AngryThirdPattern().Forget();
					break;
			}
		}
		private async UniTask AngrysecondPattern()
		{
			if (BossState.isBoss2Dead) return;
			isAttacking = true;
			var token = this.GetCancellationTokenOnDestroy();
			//if(isAngry)
			for (int i = 0; i < 15; i++)
			{
				if (CircleHit.Instance.isHighLight) break;
				if (BossState.isBoss2Dead) return;
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
			if (BossState.isBoss2Dead) return;
			if (CircleHit.Instance.isHighLight)
			{
				isAttacking = false;
				return;
			}
			await UniTask.Delay(4000, cancellationToken: token);
			isAttacking = false;
		}

		/*================================================
	 * 역할: 피버 타임 때 나타나는 마구잡이 패턴
	 *================================================ */
		private async UniTask AngryThirdPattern()
		{
			if (BossState.isBoss2Dead) return;
			isAttacking = true;
			monsterSpawner.SetActive(false);
			RotateCameraX(-35f, 1.2f).Forget();
			await AngryBigLazerAttack();
			await UniTask.Delay(2000);
			RotateCameraX(0f, 0.5f).Forget();
			if (!CircleHit.Instance.isHighLight)
			{
				isAttacking = false;
				monsterSpawner.SetActive(true);
				return;
			}
			await UniTask.Delay(1000);
			monsterSpawner.SetActive(true);
			isAttacking = false;
		}

		private async UniTask AngryBigLazerAttack()
		{
			Debug.LogError("하이라이트!");
			if (BossState.isBoss2Dead) return;
			int i = 0;
			while (CircleHit.Instance.isHighLight)
			{
				if (!CircleHit.Instance.isHighLight)
					break;
				Vector3 origin = bigChargeLazer.transform.position;
				int poolIndex = i % bigLazerBallPool.Length;
				Vector3 randomPos = origin + new Vector3(Random.Range(-9f, 9f), -2f, 0f);
				bigLazerBallPool[poolIndex].transform.position = randomPos;
				bigLazerBallPool[poolIndex].transform.localScale = Vector3.zero;
				bigLazerBallPool[poolIndex].SetActive(true);
				anime.SetTrigger("BloodAttackReady");
				for (int j = 0; j < 100; j++)
				{
					bigLazerBallPool[poolIndex].transform.localScale =
						new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * j;
					await UniTask.Delay(3);
					if (BossState.isBoss2Dead) return;
				}
				Vector3 targetPos = new Vector3(player.position.x, player.position.y - 1f, player.position.z);
				bigLazerBallPool[poolIndex].transform.LookAt(targetPos);
				i++;
			}
		}
		#endregion

		#region(일반보스패턴)
		void Boss3Pattern()
		{
			if (BossState.isBoss2Dead) return;
			switch (1)
			{
				case 1:
					SecondPattern().Forget();
					break;
			}
		}

		private async UniTask SecondPattern()
		{
			isAttacking = true;
			var token = this.GetCancellationTokenOnDestroy();
			for (int i = 0; i < 6; i++)
			{
				int poolIndex = i % lazerBallPool.Length;
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
		#endregion


		/*================================================
	 * 역할: 색상 빨갛게 변경 및 다음 패턴
	 *================================================ */
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

		/*================================================
	 * 역할: 피버를 위한 카메라 이동
	 *================================================ */
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

	#endregion

	#region 귀신 보스
	public class GBAttack : MonoBehaviour
	{
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
		Quaternion originalRotation;
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

		void Update()
		{
			if (BossState.isBoss3Dead) return;
			if (playerhp == null || playerhp.isPlayerDead) return;
			if (!isAttack && bossMove != null && bossMove.canRun)
			{
				BossPattern().Forget();
			}
			CheckForthPattern();
			if (normalMusicBox.MusicFin) //노래끝 버티기 끝
			{
				if (cts != null) cts.Cancel();
				BossState.isBoss3Dead = true;
				dialogueUI.ShowDialogueUI(true);
				PlayBossDialojet().Forget();
			}
			switch ((int)CheckRealTime.inGamerealTime)
			{
				case 74:
					CircleHit.Instance.isHighLight = true;
					Debug.LogError(CircleHit.Instance.isHighLight);
					break;

				case 100:
					CircleHit.Instance.isHighLight = false;
					Debug.LogError(CircleHit.Instance.isHighLight);
					break;

				case 148:
					CircleHit.Instance.isHighLight = true;
					Debug.LogError(CircleHit.Instance.isHighLight);
					break;
				case 159:
					CircleHit.Instance.isHighLight = false;
					Debug.LogError(CircleHit.Instance.isHighLight);
					break;
			}
		}
		private async UniTask PlayBossDialojet()
		{
			await textManager.BossDialogueCheackAsync();
		}

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
			if (CircleHit.Instance.isHighLight)
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
				await UniTask.Delay(1000);
			}
			if (!CircleHit.Instance.isHighLight)
			{
				for (int i = 0; i < feverMonsterSpawner.Length; i++)
				{
					feverMonsterSpawner[i].SetActive(false);
				}
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
				if (BossState.isBoss3Dead || CircleHit.Instance.isHighLight) { isAttack = false; KillBall.SetActive(false); return; }
				teleportIndex = Random.Range(0, 2);
				animator.SetTrigger("Teleport");
				switch (teleportIndex)
				{
					case 0:
						await SoundAttackVector(0, cts.Token);
						break;
					case 1:
						await SoundAttackVector(1, cts.Token);
						break;

				}
				if (!bossMove.canRun || cts.IsCancellationRequested)
					break;
			}
			if (transform == null || monsterSpawner == null || KillBall == null)
			{
				return; // 객체가 파괴되었다면 여기서 즉시 종료
			}
			transform.position = new Vector3(firstxPos, firstyPos, transform.position.z);
			if (CircleHit.Instance.isHighLight)
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

		private async UniTask SoundAttackVector(int patternNum, CancellationToken token)
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
			if (CircleHit.Instance.isHighLight)
			{
				transform.position = new Vector3(firstxPos, firstyPos, transform.position.z); return;
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

				if (BossState.isBoss3Dead || CircleHit.Instance.isHighLight) { isAttack = false; return; }
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
				if (BossState.isBoss3Dead || CircleHit.Instance.isHighLight)
				{
					poltergeist.SetActive(false);
					isAttack = false;
					return;
				}
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
		{
			while (!token.IsCancellationRequested)
			{
				int thisran = Random.Range(10, 15);
				await UniTask.Delay(1000 * thisran, cancellationToken: token);
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
		public async UniTask SoundSmooth(float stereo, float duration, CancellationToken token)
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
			if (cts != null)
			{
				cts.Cancel();
				cts.Dispose();
			}
		}
		#endregion
	}
    #endregion
    #endregion
// ====================== 맵 ======================

    #region ("맵 풀링")
    public class PoolingManager : MonoBehaviour
{
    
    public static PoolingManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;//오브젝트 풀의 태그
        public GameObject prefab;//풀에 저장할 오브젝트의 프리팹
        public int size;//풀의 크기
    }
    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;//태그와 오브젝트 큐를 매핑하는 딕셔너리
    private void Awake()
    {
        if (Instance == null) Instance = this;//싱글톤 인스턴스 설정
        else Destroy(gameObject);
    }
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();//딕셔너리 초기화

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();//오브젝트 큐 초기화

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);//프리팹 인스턴스화
                obj.SetActive(false);//오브젝트 비활성화
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);//태그와 오브젝트 큐를 딕셔너리에 추가
        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)//오브젝트 풀에서 오브젝트를 꺼내 활성화하고 위치와 회전을 설정
    {
        Queue<GameObject> pool = poolDictionary[tag];//태그에 해당하는 오브젝트 큐 가져오기
        GameObject obj = pool.Dequeue();//큐에서 오브젝트 꺼내기

        obj.SetActive(true);//오브젝트 활성화
        obj.transform.SetPositionAndRotation(position, rotation);//위치와 회전 설정
        pool.Enqueue(obj);//다시 큐에 오브젝트 넣기
        return obj;
    }
    #endregion

    #region ("스테이지별 맵 스폰")
    [System.Serializable]
    public class StageInfo
    {
        public string normalTag;//일반 맵 태그              
        public List<string> trapTags;//트랩x 지금은 맵 해당 스테이지 안에서 맵 전환용      
        [Range(0f, 1f)] public float trapChance;//트랩 맵이 나올 확률
        public float corridorLength = 85;//세로     
    }

    public class CorridorSpawner : MonoBehaviour
    {
        [Header("Stage")]
        public int currentStage = 1;//현 스테이지
        public List<StageInfo> stages;//스테이지 정보 리스트

        [Header("Stage Timing")]
        public float emptyDelay = 5f;//스테이지 전환시 빈 맵 지속 시간           
        public float normalDelay = 60f;//기본 스테이지 지속 시간      

        private bool isTransitionRunning = false;

        [Header("Corridor")]
        public int corridorCount = 5;//뒤에 이어지는 맵의 수
        public float corridorLength = 85f;//세로
        public float corridorWidth = 60f;//가로

        [Header("Player")]
        public Transform player;

        private Queue<GameObject> corridors = new Queue<GameObject>();
        private Coroutine stageCoroutine;

        void Start()
        {
            //시작시 기본맵 설치
            float startZ = 0f;
            for (int i = 0; i < corridorCount; i++)
            {
                string tag = GetStageCorridorTag();//태그 얻기
                GameObject corridor = PoolingManager.Instance.SpawnFromPool(
                    tag,
                    new Vector3(player.position.x, 0, startZ - 86),//-86은 시작맵 오차 보정용
                    Quaternion.identity
                );
                corridors.Enqueue(corridor);
                startZ += corridorLength;//다음 맵 위치
            }
            StartStageTimer();
        }
        void Update()
        {
            ManageCorridors();
        }
        void ManageCorridors()//맵 관리
        {
            if (corridors.Count == 0) return;

            GameObject first = corridors.Peek();//첫번째 맵 확인
            if (first.transform.position.z + corridorLength < player.position.z - 5f)//플레이어 기준 
            {
                GameObject old = corridors.Dequeue();
                old.SetActive(false);

                GameObject last = null;
                foreach (var c in corridors) last = c;//마지막 맵 확인


                Renderer[] renderers = last.GetComponentsInChildren<Renderer>();
                float endZ = 0f;
                foreach (Renderer r in renderers)//마지막 맵의 z 최대값 찾기
                {
                    endZ = Mathf.Max(endZ, r.bounds.max.z);//끝나는 z좌표
                }

                string tag = GetStageCorridorTag();
                GameObject newCorridor = PoolingManager.Instance.SpawnFromPool(//새 맵 생성
                    tag,
                    new Vector3(last.transform.position.x, 0, endZ - 1),//-1은 오차 보정용
                    Quaternion.identity
                );
                corridors.Enqueue(newCorridor);
            }
        }
        string GetStageCorridorTag()
        {
            if (currentStage < 1 || currentStage > stages.Count)
                return "Corridor";//둘다없으면 태그 Corridor로 들어감

            StageInfo stage = stages[currentStage - 1];
            if (Random.value < stage.trapChance && stage.trapTags.Count > 0)//트랩 맵이 나올 확률 조정
            {
                int randIndex = Random.Range(0, stage.trapTags.Count);//트랩 맵 태그 랜덤 선택
                return stage.trapTags[randIndex];//트랩 맵 태그 반환
            }
            return stage.normalTag;
        }
        // 타이머 관리
        void StartStageTimer()
        {
            if (stageCoroutine != null)
            {
                StopCoroutine(stageCoroutine);
                stageCoroutine = null;
            }
        }

    }

    }
	#endregion

// ====================== 적 ======================

// ====================== 대사 ======================

	#region ("대사")
	public static class TalkState
	{
		public static bool isTalking = false;//대사가 재생중인지 확인
	}
	public class TextManager : MonoBehaviour
	{
		[SerializeField] private BossTextData bosstextData;
		[SerializeField] private StageDialogueData dialogueData;

		[SerializeField] private DialogueUIManager dialogueUI;
		[SerializeField] private GameObject BossClosePanel;

		[SerializeField] private RawImage Boss1DataImage;
		[SerializeField] private RawImage Boss2DataImage;
		[SerializeField] private RawImage Boss3DataImage;

		[SerializeField] private Texture Boss1DataImage_KO;
		[SerializeField] private Texture Boss1DataImage_EN;
		[SerializeField] private Texture Boss2DataImage_KO;
		[SerializeField] private Texture Boss2DataImage_EN;
		[SerializeField] private Texture Boss3DataImage_KO;
		[SerializeField] private Texture Boss3DataImage_EN;

		[SerializeField] public GameObject BossButton;
		public static bool BossButtonClicked = false;

		private CancellationTokenSource dialogueCTS;
		private CancellationTokenSource bossCTS;
		bool skipAll = false;

		[SerializeField] ParticleSystem bossDeadParticle;
		[SerializeField] GameObject boss;
		private bool isBossDialogueStarted = false;

		[SerializeField] TMP_InputField inputField;
		[SerializeField] GameObject inputPanel;

		#region ("스테이지 진입 씬 대사")
		public async UniTask DelayedDialogueCheckAsync()//스테이지 선택씬에서 대사 출력
		{

			if (!BossState.isBoss1Dead && !BossState.isBoss2Dead)
			{
				await StartStageDialogueAsync(1);
			}
			if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
			{
				await StartStageDialogueAsync(4);
			}
			if (!BossState.isBoss3Dead)
			{
				await StartStageDialogueAsync(8);
			}
		}
		public void OnBossImageClick()//스테이지 선택씬에서 대사 출력이후 보스패턴 이미지 패널 활성화시 닫기 버튼으로 씬 전환
		{
			if (!BossState.isBoss1Dead && !BossState.isBoss2Dead)//보스 1 시작전
			{
				OnBossImage(2).Forget();
				SceneLoader.Instance.LoadScene("Stage2");
			}
			if (BossState.isBoss1Dead && !BossState.isBoss2Dead)//보스 2 시작전
			{
				OnBossImage(3).Forget();
				SceneLoader.Instance.LoadScene("Stage3");
			}
			if (BossState.isBoss1Dead && BossState.isBoss2Dead)//보스 3 시작전
			{
				OnBossImage(4).Forget();
				SceneLoader.Instance.LoadScene("LastStage");
			}
		}
		public async UniTask MovieDialogueAsync()//<- 이거 movie씬 대사 처리
		{
			dialogueCTS?.Cancel();
			dialogueCTS = new CancellationTokenSource();//여기까지 기존 대사 취소및 초기화
			DialogueLine[] lines = dialogueData.stage1.dialogues;// 그 데이터 stage1에 있는 대사 불러옴
			if (lines != null)
				await firstPlayDialogueAsync(lines, 0, dialogueCTS.Token);//출력
		}

		public async UniTask StartStageDialogueAsync(int stageNum)
		{
			dialogueCTS?.Cancel();
			dialogueCTS = new CancellationTokenSource();

			DialogueLine[] lines = stageNum switch//스테이지별 대사들
			{
				1 => dialogueData.stage1.dialogues,
				2 => dialogueData.stage2.dialogues,
				3 => dialogueData.stage3.dialogues,
				4 => dialogueData.stage4.dialogues,
				5 => dialogueData.stage5.dialogues,
				_ => null
			};

			if (lines != null)
				await firstPlayDialogueAsync(lines, stageNum, dialogueCTS.Token);
		}
		private async UniTask firstPlayDialogueAsync(DialogueLine[] lines, int stageNum, CancellationToken token)//대사 출력
		{
			TalkState.isTalking = true;
			int index = 0;
			bool waitingForClick = false;
			bool skipAll = false;
			System.Action onClick = () => waitingForClick = false;
			System.Action onSkipAll = () => skipAll = true;

			dialogueUI.OnDialogueClick += onClick;
			dialogueUI.OnSkipAll += onSkipAll;

			dialogueUI.ShowDialogueUI(true);//대화 ui 활성화
			dialogueUI.StartImageAnimation(stageNum);//스테이지별 이미지 애니메이션

			while (index < lines.Length)
			{
				if (token.IsCancellationRequested || skipAll) break;

				DialogueLine line = lines[index];
				var localized = new LocalizedString(line.localizationTable, line.localizationKey);
				string localizedText = await localized.GetLocalizedStringAsync();//텍스트 불러오기

				dialogueUI.ShowDialogueText(localizedText, line.sound);//텍스트 출력

				waitingForClick = true;
				while (waitingForClick && !token.IsCancellationRequested && !skipAll)//클릭 할때까지 대기
					await UniTask.Yield(PlayerLoopTiming.Update, token);//한문장 출력 대기

				index++;
			}

			dialogueUI.OnDialogueClick -= onClick;
			dialogueUI.OnSkipAll -= onSkipAll;
			dialogueUI.StopImageAnimation();
			dialogueUI.ShowDialogueUI(false);
			dialogueUI.speechBubble.SetActive(false);

			TalkState.isTalking = false;
		}
		#endregion

		#region("보스 사망시 대사")
		private async UniTask BossHandleBossDeathAsync(int bossStage)//보스 사망시 대사 재생
		{
			if (bossStage == 3)
				BossState.isBoss3Dead = true;
			dialogueUI.ShowDialogueUI(true);
			await BossStartStageDialogueAsync(bossStage);
		}

		public async UniTask BossStartStageDialogueAsync(int firstStage)//보스 사망시 대사 시작
		{
			bossCTS?.Cancel();
			bossCTS = new CancellationTokenSource();
			var bossSet = System.Array.Find(bosstextData.bossDialogues, x => x.bossName == $"Boss{firstStage}");//보스 이름과 일치하는 대사 찾기
			if (bossSet == null)
			{
				return;
			}
			BossLine[] lines = bossSet.deathLines;
			if (lines != null)
			{
				await stagePlayDialogueAsync(lines, firstStage, bossCTS.Token);
			}
		}
		private async UniTask stagePlayDialogueAsync(BossLine[] lines, int stageNum, CancellationToken token)//보스 대사 출력
		{
			Debug.Log(lines);
			int index = 0;
			bool waitingForClick = false;

			System.Action onClick = () => waitingForClick = false;
			dialogueUI.OnDialogueClick += onClick;

			dialogueUI.ShowDialogueUI(true);
			dialogueUI.BossImageAnimation(stageNum);

			while (index < lines.Length)
			{

				if (token.IsCancellationRequested) break;

				BossLine line = lines[index];

				string displayText = line.text;
				if (!string.IsNullOrEmpty(line.localizationTable) && !string.IsNullOrEmpty(line.localizationKey))//영문,한문 대사 처리
				{
					var localized = new LocalizedString(line.localizationTable, line.localizationKey);
					displayText = await localized.GetLocalizedStringAsync();//텍스트 불러오기
				}

				dialogueUI.ShowDialogueText(displayText, line.sound);

				waitingForClick = true;
				while (waitingForClick && !token.IsCancellationRequested)
					await UniTask.Yield(PlayerLoopTiming.Update, token);

				index++;
			}

			dialogueUI.OnDialogueClick -= onClick;
			dialogueUI.StopImageAnimation();
			dialogueUI.ShowDialogueUI(false);
			dialogueUI.speechBubble.SetActive(false);
		}
		#endregion

		#region ("보스 사망시 씬전환")
		public async UniTask BossDialogueCheackAsync()//보스 사망시 스테이지 선택씬 전환
		{

			if (isBossDialogueStarted) return;
			isBossDialogueStarted = true;
			if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
			{
				await PlayDeadParteicle();//보스 사망시 파티클 재생
				Destroy(boss);
				ComboSave.Instance.MaxcomboSaveScr();//최대 콤보 저장
				await UniTask.Delay(2000);
				SceneLoader.Instance.LoadScene("StageSelect");
			}
			if (BossState.isBoss1Dead && BossState.isBoss2Dead && !BossState.isBoss3Dead)
			{
				await PlayDeadParteicle();//보스 사망시 파티클 재생
				Destroy(boss);
				ComboSave.Instance.MaxcomboSaveScr();//최대 콤보 저장
				await UniTask.Delay(2000);
				SceneLoader.Instance.LoadScene("StageSelect");
			}
			if (BossState.isBoss3Dead)
			{
				BossClosePanel.SetActive(true);
				await BossHandleBossDeathAsync(3);
				InputName();  // 이름 입력받아서 순위 띄우기

			}
		}
		public async UniTask ClosePanel()
		{
			BossClosePanel.SetActive(true);

		}
		private async UniTask PlayDeadParteicle()//보스 사망 파티클
		{
			Vector3 bossPos = new Vector3(boss.transform.position.x, boss.transform.position.y + 3f, boss.transform.position.z);
			ParticleSystem deadPartice = Instantiate(bossDeadParticle, bossPos, Quaternion.identity);
			deadPartice.Play();
		}
		#endregion

		#region ("언어 설정 및 엔딩")
		private void InputName()//엔딩시 유저 닉네임 기록용
		{
			Cursor.visible = true;
			inputPanel.SetActive(true);
			inputField.ActivateInputField();
		}

		public void EnterInput()
		{
			EnterPressed().Forget();
		}
		public async UniTask OnBossImage(int stageNum, bool active = true)//영문,한문 전환
		{
			// 현재 설정된 언어 가져오기 (0 = en, 1 = ko)
			int lang = PlayerPrefs.GetInt("LocalKey", 1);

			switch (stageNum)
			{
				case 2:
					Boss1DataImage.gameObject.SetActive(active);
					if (active)
						Boss1DataImage.texture = (lang == (int)Language.en) ? Boss1DataImage_EN : Boss1DataImage_KO;
					BossButton.gameObject.SetActive(active);
					break;

				case 3:
					Boss2DataImage.gameObject.SetActive(active);
					if (active)
						Boss2DataImage.texture = (lang == (int)Language.en) ? Boss2DataImage_EN : Boss2DataImage_KO;
					BossButton.gameObject.SetActive(active);
					break;

				case 4:
					Boss3DataImage.gameObject.SetActive(active);
					if (active)
						Boss3DataImage.texture = (lang == (int)Language.en) ? Boss3DataImage_EN : Boss3DataImage_KO;
					BossButton.gameObject.SetActive(active);
					break;
			}
		}

		public async UniTask EnterPressed()//엔딩씬 이름 입력 후 씬 전환
		{
			if (string.IsNullOrEmpty(inputField.text))
				return;

			Datamanager.instance.curPlayerData.playerName = inputField.text;
			Datamanager.instance.curPlayerData.combo = ComboSave.Instance.maxComboData.maxComboValue;
			Datamanager.instance.SaveToJson();

			ComboSave.Instance.maxComboData.maxCombo = 0;

			inputPanel.SetActive(false);
			await PlayDeadParteicle();
			Destroy(boss);

			await UniTask.Delay(1500);

			SceneLoader.Instance.LoadScene("Ending");
		}
		#endregion

		#region ("튜토리얼 대사 출력")
		public class TutorialManager : MonoBehaviour
		{
			[Header("Scriptable Data")]
			[SerializeField] private BossTextData tutorialData;

			[Header("UI")]
			[SerializeField] private TutorialUIManager TutoUI;
			[SerializeField] private GameObject nextButton;

			[Header("Dummy")]
			[SerializeField] private GameObject dummyTarget;
			[SerializeField] private Renderer dummyRenderer;
			[SerializeField] private Color highlightColor = Color.yellow;
			[SerializeField] private float highlightDuration = 10f;

			[Header("Settings")]
			[SerializeField] private float charInterval = 0.085f;

			private DummySpawner dummySpawner;
			private CancellationTokenSource tutorialCTS;

			private bool isTyping = false;
			private bool skipRequested = false;
			private bool waitingForNext = false;
			private bool dummyDeadTriggered = false;

			private Color originalColor;
			private void Start()
			{
				if (dummySpawner == null)
					dummySpawner = FindObjectOfType<DummySpawner>();

				if (nextButton != null)
				{
					var btn = nextButton.GetComponent<UnityEngine.UI.Button>();
					if (btn != null)
					{
						btn.onClick.RemoveAllListeners();
						btn.onClick.AddListener(OnDialogueClicked);
					}
				}

				if (dummyRenderer != null)
					originalColor = dummyRenderer.material.color;
				StartTutorialAsync().Forget();
			}
			#region ("튜토리얼 대사 재생")
			private async UniTaskVoid StartTutorialAsync()//튜토리얼 대사 시작
			{
				tutorialCTS?.Cancel();
				tutorialCTS = new CancellationTokenSource();
				TutoUI.StartImageAnimation();

				if (tutorialData.bossDialogues.Length > 0)
					await PlayDialogueSetAsync(tutorialData.bossDialogues[0].deathLines, tutorialCTS.Token);

				HighlightDummyAsync().Forget();

				await UniTask.WaitUntil(() => dummySpawner != null && dummySpawner.dummyHp <= 1);
				await OnDummyDeadSequence();
			}
			private async UniTask PlayDialogueSetAsync(BossLine[] lineSet, CancellationToken token)//대사 재생
			{
				if (lineSet == null || lineSet.Length == 0)//대사 없으면 종료
				{
					return;
				}
				foreach (var line in lineSet)//대사 한줄씩 재생
				{
					await PlayDialogueLineAsync(line, token);
				}
			}
			private async UniTask PlayDialogueLineAsync(BossLine line, CancellationToken token)//대사 한줄 재생
			{
				TutoUI.ShowDialogueUI(true);
				TutoUI.OnDialogueClick -= OnDialogueClicked;
				TutoUI.OnDialogueClick += OnDialogueClicked;

				await TypeLocalizedDialogueAsync(line.localizationTable, line.localizationKey, line.sound, token);//대사 출력

				waitingForNext = true;
				if (nextButton != null) nextButton.SetActive(true);//Next 버튼 활성화

				await UniTask.WaitUntil(() => waitingForNext == false, cancellationToken: token);//Next 버튼 클릭 대기
				TutoUI.OnDialogueClick -= OnDialogueClicked;
			}
			private async UniTask TypeLocalizedDialogueAsync(string table, string key, AudioClip clip, CancellationToken token)//대사 텍스트 타이핑 효과
			{
				TutoUI.ClearText();
				isTyping = true;

				var localizedString = new LocalizedString(table, key);
				string text = await localizedString.GetLocalizedStringAsync();//텍스트 불러오기

				if (string.IsNullOrEmpty(text))
				{
					text = key;
				}
				for (int i = 0; i < text.Length; i++)
				{
					if (token.IsCancellationRequested) return;

					if (skipRequested)//스킵 요청시 남은 텍스트 한번에 출력
					{
						TutoUI.AppendText(text.Substring(i));//텍스트 한번에 출력
						skipRequested = false;
						break;
					}
					TutoUI.AppendText(text[i].ToString());//텍스트 한글자씩 출력
					if (clip != null) TutoUI.PlaySound(clip);//텍스트 출력 사운드 재생
					await UniTask.Delay(TimeSpan.FromSeconds(charInterval), cancellationToken: token);//타이핑 효과 대기
				}

				isTyping = false;
			}
			#endregion
			#region ("훈련장 더미 관련")
			private async UniTask OnDummyDeadSequence()//더미 사망시 대사 및 씬 전환
			{
				if (dummyDeadTriggered) return;
				dummyDeadTriggered = true;

				TutoUI.StopImageAnimation();

				if (tutorialData.bossDialogues.Length > 1)
					await PlayDialogueSetAsync(tutorialData.bossDialogues[1].deathLines, tutorialCTS.Token);//더미 사망 후 대사

				await UniTask.Delay(TimeSpan.FromSeconds(1));//대사 후 잠시 대기
				PlayerPrefs.SetInt("IsTutorial", 1);//튜토리얼 완료
				SceneLoader.Instance.LoadScene("StageSelect");
			}
			private void OnDialogueClicked()
			{
				if (isTyping)
					skipRequested = true;
				else if (waitingForNext)
					waitingForNext = false;
			}
			private async UniTask HighlightDummyAsync()//더미 반짝이는 하이라이트 효과
			{
				if (dummyRenderer == null) return;

				float timer = 0f;
				while (timer < highlightDuration)
				{
					dummyRenderer.material.color = Color.Lerp(originalColor, highlightColor, Mathf.PingPong(timer * 4f, 1f));//색상 변화
					timer += Time.deltaTime;
					await UniTask.Yield();
				}
				dummyRenderer.material.color = originalColor;//원래 색상 복원
			}
			#endregion
			private void OnDestroy()
			{
				tutorialCTS?.Cancel();
			}
		}
		#endregion
	}
	#endregion

// ====================== 저장 ======================
public class ComboSave : MonoBehaviour
    {   // 콤보 저장
        public static ComboSave Instance { get; private set; }

        public MaxComboData maxComboData;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        public void MaxcomboSaveScr()
        {
            maxComboData.maxComboValue = CircleHit.Instance.combo;
        }

    }

    public class Datamanager : MonoBehaviour
    {
        // 제이슨 저장
        public static Datamanager instance;

        public PlayerData curPlayerData = new PlayerData();
        public PlayerDataList allData = new PlayerDataList();

        string path;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

#if UNITY_EDITOR
            string folder = Path.Combine(Application.dataPath, "Json");
#else
        string folder = Path.Combine(Application.dataPath, "../Json");
#endif

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            path = Path.Combine(folder, "PlayerData.json");
        }

        public void SaveToJson()
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                allData = JsonUtility.FromJson<PlayerDataList>(json);
            }
            else
            {
                allData = new PlayerDataList();
            }

            allData.players.Add(curPlayerData);

            string newJson = JsonUtility.ToJson(allData, true);
            File.WriteAllText(path, newJson);

        }

        public void LoadFromJson()
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                allData = JsonUtility.FromJson<PlayerDataList>(json);
            }
            else
            {
                allData = new PlayerDataList();
            }
        }
    }

    [CreateAssetMenu(fileName = "MaxComboData", menuName = "MaxComboData")]
    public class MaxComboData : ScriptableObject
    {
        // 최고 콤보 저장용 스크립터블 오브젝트
        public int maxCombo;
        public int maxComboValue
        {
            get { return maxCombo; }
            set
            {
                if (value > maxCombo)
                {
                    maxCombo = value;
                }
                else
                {
                    return;
                }
            }
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        // 저장할 플레이어 데이터
        public string playerName;
        public int combo;
    }

    public class PlayerDataList
    {
        // 플레이어 데이터들
        public List<PlayerData> players = new List<PlayerData>();
    }


    // ====================== 가상 키보드 ======================
    public class VirtualKeyboard : MonoBehaviour
    {
        [SerializeField] private TMP_InputField targetInput;

        public void OnKeyPress(string key)
        {
            if (targetInput == null) return;

            switch (key)
            {
                case "Backspace":
                    if (targetInput.text.Length > 0)
                        targetInput.text = targetInput.text.Substring(0, targetInput.text.Length - 1);
                    break;

                case "Enter":
                    break;

                default:
                    targetInput.text += key;
                    break;
            }

            targetInput.caretPosition = targetInput.text.Length;
        }

        public void SetTarget(TMP_InputField input)
        {
            targetInput = input;
        }
    }

    public class KeyboardButton : MonoBehaviour
    {
        [SerializeField] private string keyValue;
        private VirtualKeyboard keyboard;

        void Start()
        {
            keyboard = GetComponentInParent<VirtualKeyboard>();
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            keyboard.OnKeyPress(keyValue);
        }
    }

	// ====================== 정확한 시간 판정 ======================
	#region 정확한 시간 판정
	/*================================================
	 * 역할: 각 보스들의 정확한 시간에 패턴을 변경시키기 위한 타이머
	 *================================================ */
	public class CheckRealTime : MonoBehaviour
	{
		public static CheckRealTime Instance { get; private set; }
		static public double inGamerealTime = 0;

		int plusTime = 0;
		public double startTime;

		int Count = 0;
		void Awake()
		{
			// 싱글톤
			if (Instance == null) Instance = this;
			else if (Instance != this) Destroy(gameObject);
		}
		private void Start()
		{
			startTime = AudioSettings.dspTime;
		}
		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				plusTime += 10;
			}
			inGamerealTime = AudioSettings.dspTime - startTime + plusTime;
		}
	}
	#endregion
}
