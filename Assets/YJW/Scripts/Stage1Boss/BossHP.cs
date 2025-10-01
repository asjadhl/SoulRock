using UnityEngine;

public class BossHP : MonoBehaviour
{
    public int bossHP = 3;

    private void FixedUpdate()
    {
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
