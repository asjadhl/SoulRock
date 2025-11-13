using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers; // GetCancellationTokenOnDestroy() 사용을 위한 추가
using System.Threading; // CancellationToken 타입을 위한 추가 (권장)
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // CanvasGroup을 사용하려면 이 using이 필요할 수 있습니다. (현재 코드에는 없지만 Unity UI 사용 시 필수)

public class PlayerHP : MonoBehaviour
{
	/*
     * 0.1초에 0.1씩 줄어들고
     * PlayerShoot()실행되면 = 박자에 맞춰서 총을 쏘면(누굴 못맞추더라도) 0.2씩 플러스.
     * 적에게 대미지를 받으면 -10씩
     * 적을 잘 맞추면 +5씩
     */
	PlayerDieText playerDieText;
	[SerializeField] private GameOverManager gameOverManager;

	[SerializeField] float _playerHP;
	private float maxHP;
	public float playerHP
	{
		get => _playerHP;
		set => _playerHP = Mathf.Clamp(value, 0, maxHP);
	}

	[SerializeField] GameObject DamageImage;
	[SerializeField] GameObject gameOver;
    NormalMusicBox normalMusicBox;
    public bool isPlayerDead = false;
	private bool isProcessingDeath = false; // <<< 추가: 사망 시퀀스 중복 실행 방지 플래그

	private string sceneName;

	private void Awake()
	{
		maxHP = _playerHP;
		sceneName = SceneManager.GetActiveScene().name;
	}

	private void Start()
	{
		playerDieText = FindAnyObjectByType<PlayerDieText>();

        //normalMusicBox = GameObject.FindWithTag("MusicBox").GetComponent<NormalMusicBox>();

		normalMusicBox = FindMusicBox();

    }

	private void FixedUpdate()
	{
		// 보스 사망 시에도 isProcessingDeath 상태는 체크할 필요가 없습니다.
		//if (isPlayerDead == true || BossState.isBoss1Dead == true || BossState.isBoss2Dead == true || BossState.isBoss3Dead == true)
		//	return;
        if (isPlayerDead == true || normalMusicBox.MusicFin)
            return;


        PlayerHPTimer();

		// 사망 조건이 만족되었고, 아직 사망 시퀀스가 시작되지 않았을 때만 진입합니다.
		if ((playerHP <= 0 ) && !isProcessingDeath) // <<< 조건 수정
		{
			isPlayerDead = true;
			isProcessingDeath = true; // <<< 플래그 설정
			PlayerDie().Forget();
		}
	}

	private void PlayerHPTimer()
	{
		if (sceneName == "TutorialTrainingRoom"  || sceneName == "TraingRoom")
			return;
		playerHP -= 0.05f;
	}

	public void PlayerHPPlus(int recover)
	{
		if (isPlayerDead == true)
			return;
		playerHP += recover;
	}

	public async UniTask PlayerHPMinus()
	{
		playerHP -= 15;
		GetDamImageOn();
		await UniTask.Delay(300, cancellationToken: this.GetCancellationTokenOnDestroy());
		GetDamImageOff();
	}
	public async UniTask PlayerHPBigMinus()
	{
		playerHP -= 30;
		GetDamImageOn();
		await UniTask.Delay(300, cancellationToken: this.GetCancellationTokenOnDestroy());
		GetDamImageOff();
	}
	//private async UniTaskVoid PlayerDie()
	//   {
	//	InitializeData();

	//	// 1. TriggerGameOver 호출 로직 수정
	//	// WithCancellation()을 사용하여 CancellationToken을 전달하고, Forget()에는 인자를 넣지 않습니다.
	//	FindAnyObjectByType<GameOverManager>()?
	//.TriggerGameOver()
	//.Forget();

	//	// 2. 널 체크: CanvasGroup을 가져오기 전에 gameOver 객체 확인
	//	if (gameOver == null)
	//	{
	//		Debug.LogError("GameOver UI 오브젝트가 null입니다. 사망 시퀀스를 중단합니다.");
	//		return;
	//	}

	//	// 3. CanvasGroup 가져오기
	//	CanvasGroup cg = gameOver.GetComponent<CanvasGroup>();
	//	if (cg == null) return;

	//	Cursor.visible = true;

	//	// 4. 페이드 인 루프
	//	while (cg.alpha < 1f)
	//	{
	//		cg.alpha += Time.deltaTime / 5f;

	//		// 객체 파괴 시 취소하는 안전한 비동기 대기
	//		await UniTask.Yield(this.GetCancellationTokenOnDestroy());

	//		if (gameOver == null) break;
	//	}
	//}
	private async UniTaskVoid PlayerDie()
	{

		// 1. GameOverManager 인스턴스를 찾습니다.
		GameOverManager gameOverManagerInstance = FindAnyObjectByType<GameOverManager>();

		// 2. 명시적 널 체크 후 WithCancellation을 사용해 안전하게 호출합니다.
		if (gameOverManagerInstance != null)
		{
			gameOverManagerInstance
				.TriggerGameOver() // 이 스크립트 파괴 시 취소
				.Forget();
		}

		// 3. 널 체크: CanvasGroup을 가져오기 전에 gameOver 객체 확인
		if (gameOver == null)
		{
			Debug.LogError("GameOver UI 오브젝트가 null입니다. 사망 시퀀스를 중단합니다.");
			return;
		}

		// 4. CanvasGroup 가져오기
		CanvasGroup cg = gameOver.GetComponent<CanvasGroup>();
		if (cg == null) return;

		Cursor.visible = true;

		// 5. 페이드 인 루프
		while (cg.alpha < 1f)
		{
			cg.alpha += Time.deltaTime / 5f;

			// 객체 파괴 시 취소하는 안전한 비동기 대기
			await UniTask.Yield(this.GetCancellationTokenOnDestroy());

			if (gameOver == null) break;
		}
	}
	private void GetDamImageOn()
    {
        DamageImage.SetActive(true);
    }

    private void GetDamImageOff()
    {
        DamageImage.SetActive(false);
    }










    private NormalMusicBox FindMusicBox()
    {
        var normalboxs = FindObjectsByType<NormalMusicBox>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        if (normalboxs != null)
            if (normalboxs.Length > 0)
                for (int i = 0; i < normalboxs.Length; i++)
                    if (normalboxs[i].name == "MusicBox")
                             return normalboxs[i];
		return null;
    }

}
