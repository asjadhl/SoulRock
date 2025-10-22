using UnityEngine;
using UnityEngine.SceneManagement;

public static class BossState
{
    public static bool isBoss3Dead = false;
}

public class BossHP : MonoBehaviour
{
    public int bossHP = 3;

    private bool Stage1 = false;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "Stage1")
            Stage1 = true;
    }


    private void FixedUpdate()
    {
        BossDie();
    }

    public void BossHPMinus()
    {
        bossHP--;
        if (Stage1 == true)
            GetComponent<Boss1HPUI>().BossHPUI();
    }

    private void BossDie()
    {
        if (bossHP <= 0)
            Debug.Log("더러운 파피루스 살인마.");
    }
}
