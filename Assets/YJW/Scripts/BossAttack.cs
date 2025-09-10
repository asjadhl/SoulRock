using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField] GameObject[] bulletPrefabs;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform spawnPoint2;
    public GameObject bullet;
    private float attackTime = 2.4f;
    private float attackTimer = 2.4f;

    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        BossAttackTimer();
    }

    private void BossAttackTimer()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer >= attackTime)
        {
            attackTimer = 0;
            //BossAttack_();
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
