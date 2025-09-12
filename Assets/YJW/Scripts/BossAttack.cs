using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField] GameObject[] bulletPrefabs;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform spawnPoint2;
    public GameObject bullet;
    private float attack1Time = 2.4f;
    private float attack2Time = 8f;
    private float attackTimer = 2.4f;
    private float attack2Timer = 8f;

    void Start()
    {
        
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
        bullet = Instantiate(bulletPrefabs[Random.Range(0, bulletPrefabs.Length)], spawnPoint.transform.position, Quaternion.identity);

        
    }

    private void BossAttack_2()
    {
        Instantiate(bulletPrefabs[3], spawnPoint2.transform.position, Quaternion.identity);
    }
}
