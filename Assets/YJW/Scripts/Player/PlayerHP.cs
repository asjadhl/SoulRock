using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int playerHP = 10;

    [SerializeField] GameObject DamageImage;

    private void FixedUpdate()
    {
        if(playerHP <=0 || Stage2BossAttack.clubStack == 7)
            PlayerDie();
    }

    public async UniTask PlayerHPMinus()
    {
        playerHP--;
        GetDamImageOn();
        await UniTask.Delay(300);
        GetDamImageOff();
    }

    private void PlayerDie()
    {
        Debug.Log("Player Die!");
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
