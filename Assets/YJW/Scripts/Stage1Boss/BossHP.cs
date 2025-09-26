using UnityEngine;

public class BossHP : MonoBehaviour
{
    public int bossHP = 3;

    private void FixedUpdate()
    {
        Debug.Log(bossHP);
        BossDie();
    }

    public void BossHPMinus()
    {
        bossHP--;
    }

    private void BossDie()
    {
        if(bossHP <= 0)
            Destroy(gameObject);
    }
}
