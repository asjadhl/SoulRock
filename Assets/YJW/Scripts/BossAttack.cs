using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField] GameObject[] bulletPrefabs;
    [SerializeField] Transform spawnPoint;
    public GameObject bullet;
    private float attackTime = 4f;
    private float attackTimer = 0;

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
            BossAttack_();
        }
    }

    public  void BossAttack_()
    {
        bullet = Instantiate(bulletPrefabs[Random.Range(0, bulletPrefabs.Length)], spawnPoint.transform.position, Quaternion.identity);

        if(bullet == bulletPrefabs[1])
        {
            
        }
    }
}
