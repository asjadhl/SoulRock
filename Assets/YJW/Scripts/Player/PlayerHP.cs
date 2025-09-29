using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int playerHP = 10;

    private void FixedUpdate()
    {
        if(playerHP <=0 || Stage2BossAttack.clubStack == 7)
            PlayerDie();
        //Debug.Log(playerHP);
    }

    public void PlayerHPMinus()
    {
        playerHP--;
        //GetComponent<PlayerHPUI>().PlayerHPUIUpdate(playerHP);
    }

    private void PlayerDie()
    {
        Debug.Log("Player Die!");
    }

}
