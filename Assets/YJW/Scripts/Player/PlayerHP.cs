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

    [SerializeField] float _playerHP;
    public float playerHP
    {
        get => _playerHP;
        set => _playerHP = Mathf.Clamp(value, 0, _playerHP);
    }

    [SerializeField] GameObject DamageImage;
    [SerializeField] GameObject gameOver;
    private void Start()
    {
        playerDieText = FindObjectOfType<PlayerDieText>();
        
    }
    private void FixedUpdate()
    {
        PlayerHPTimer();

        if (playerHP <= 0 || Stage2BossAttack.clubStack == 7)
            PlayerDie().Forget();
    }

    private void PlayerHPTimer()
    {
        playerHP -= 0.05f;
    }

    public void PlayerHPPlus(int recover)
    {
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

        CanvasGroup cg = gameOver.GetComponent<CanvasGroup>();
        while(cg.alpha < 1f)
        {
            cg.alpha += Time.deltaTime / 10f;
            await UniTask.Yield();
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

}
