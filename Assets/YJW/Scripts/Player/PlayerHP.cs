using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHP : MonoBehaviour
{
    /*
     0.1초에 0.1씩 줄어들고
    PlayerShoot()실행되면 = 박자에 맞춰서 총을 쏘면(누굴 못맞추더라도) 0.2씩 플러스.
    적에게 대미지를 받으면 -10씩
    적을 잘 맞추면 +5씩
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

    public bool isPlayerDead;

    private void Awake()
    {
        maxHP = _playerHP;
    }

    private void Start()
    {
        playerDieText = FindObjectOfType<PlayerDieText>();
        
    }
    private void FixedUpdate()
    {
        if (isPlayerDead == true || BossState.isBoss1Dead == true || BossState.isBoss2Dead == true || BossState.isBoss3Dead == true)
            return;
        PlayerHPTimer();

        if (playerHP <= 0 || Stage2BossAttack.clubStack == 7)
        {
			isPlayerDead = true;
			PlayerDie().Forget();
		}
			
    }

    private void PlayerHPTimer()
    {
        playerHP -= 0.05f;
    }

    public void PlayerHPPlus(int recover)
    {
        if (isPlayerDead == true || BossState.isBoss1Dead == true || BossState.isBoss2Dead == true || BossState.isBoss3Dead == true)
            return;
        playerHP += recover;
    }

    public async UniTask PlayerHPMinus()
    {
        playerHP -= 10;
        GetDamImageOn();
        await UniTask.Delay(300);
        GetDamImageOff();
    }

    public async UniTask PlayerHPBigMinus()
    {
        playerHP -= 30;
        GetDamImageOn();
        await UniTask.Delay(300);
        GetDamImageOff();
    }

    private bool isDead = false;

    private async UniTaskVoid PlayerDie()
    {
        //if (isDead) return;
        //isDead = true;

        //if (playerDieText == null) return;
        //Debug.Log("플레이어 사망 시퀀스 시작");

        //await playerDieText.OnDeadSequence(3);


        //SceneManager.LoadScene("Main");
        InitializeData();

        FindAnyObjectByType<GameOverManager>()?.TriggerGameOver().Forget();

        CanvasGroup cg = gameOver.GetComponent<CanvasGroup>();
        if (gameOver == null) return;
		Cursor.visible = true;
		while (cg.alpha < 1f)
        {
            cg.alpha += Time.deltaTime / 5f;
            await UniTask.Yield();
            if(gameOver == null) break;
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

    private void InitializeData()
    {
        Stage2BossAttack.clubStack = 0;
    }

}
