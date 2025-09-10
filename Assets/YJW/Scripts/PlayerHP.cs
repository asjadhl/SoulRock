using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    private int playerHP = 10;

    private void FixedUpdate()
    {
        PlayerDie();
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
