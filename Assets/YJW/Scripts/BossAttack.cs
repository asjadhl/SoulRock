using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public float attack1Time;
    public float attack2Time;
    [SerializeField] float[] attack1Times;   // 3, 7, 9
    [SerializeField] float[] attack2Times;   // 8, 11, 16
    private float attackTimer = 0;
    private float attack2Timer = 0;

    [SerializeField] int[] GreenBallTime; // 53, 108, 170
    private int index = 0;

    private Animator bossAnim;

    private void Start()
    {
        bossAnim = GetComponent<Animator>();
        attack1Time = attack1Times[Random.Range(0, attack1Times.Length)];
        attack2Time = attack2Times[Random.Range(0, attack2Times.Length)];

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
            attack1Time = attack1Times[Random.Range(0, attack1Times.Length)];
        }
    }

    private void BossAttack2Timer()
    {
        attack2Timer += Time.deltaTime;
        if(attack2Timer >= attack2Time)
        {
            attack2Timer = 0;
            BossAttack_2();
            attack2Time = attack2Times[Random.Range(0, attack2Times.Length)];
        }
    }

    private void GreenBallTimer()
    {
        if((int)CheckRealTime.inGamerealTime == GreenBallTime[index])
        {

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
