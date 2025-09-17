using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private float attack1Time = 1.5f;
    private float attack2Time = 8f;
    private float attackTimer = 1.5f;
    private float attack2Timer = 8f;

    private Animator bossAnim;

    private void Start()
    {
        bossAnim = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        BossAttack1Timer();
        BossAttack2Timer();
    }

    private void BossAttack1Timer()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer >= attack1Time)
        {
            attackTimer = 0;
            BossAttack_();
        }
    }

    private void BossAttack2Timer()
    {
        attack2Timer += Time.deltaTime;
        if(attack2Timer >= attack2Time)
        {
            attack2Timer = 0;
            BossAttack_2();
        }
    }

    public  void BossAttack_()
    {
        int RanIndex = Random.Range(0, 6);
        bossAnim.SetTrigger("Attack");
        ObjectPool.Instance.bulletAArr[RanIndex].SetActive(true);
    }

    private void BossAttack_2()
    {
        int RanIndex = Random.Range(0, 3);
        ObjectPool.Instance.bulletBArr[RanIndex].SetActive(true);
    }
}
