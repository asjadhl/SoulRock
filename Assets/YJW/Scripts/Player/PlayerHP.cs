using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    /*
     0.1초에 0.1씩 줄어들고
    PlayerShoot()실행되면 = 박자에 맞춰서 총을 쏘면(누굴 못맞추더라도) 0.2씩 플러스.
    적에게 대미지를 받으면 -10씩
    적을 잘 맞추면 +5씩
     */

    public float playerHP = 100;

    [SerializeField] GameObject DamageImage;

    private void FixedUpdate()
    {
        PlayerHPTimer();
        if (playerHP <=0 || Stage2BossAttack.clubStack == 7)
            PlayerDie();
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

    private void PlayerDie()
    {
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
