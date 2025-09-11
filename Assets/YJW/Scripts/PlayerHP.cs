using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    private int playerHP = 10;

    private void FixedUpdate()
    {
        PlayerDie();
        Debug.Log(playerHP);
    }

    public void PlayerHPMinus()
    {
        playerHP--;
    }

    private void PlayerDie()
    {
        if (playerHP <= 0)
            Debug.Log("Player Die!");
    }

}
