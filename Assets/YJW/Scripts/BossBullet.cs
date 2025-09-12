using UnityEngine;

public class BossBullet : MonoBehaviour
{
    Rigidbody bulletRb;
    private float speed = 7f;
    private GameObject player;
    private GameObject boss;

    private bool isAttacked = false;

    void Awake()
    {
        bulletRb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        boss = GameObject.FindWithTag("Boss");
    }

    private void FixedUpdate()
    {
        if(gameObject.activeSelf == true )
        {
            ThrowAttack(player.transform);
            
        }

        if (transform.position.z <= -4.5f)
        {
            ReturnSpawnPoint();
            player.GetComponent<PlayerHP>().PlayerHPMinus();
        }
    }

    private void ThrowAttack(Transform target)
    {
        //Vector3[] v0 = { new Vector3(target.transform.position.x - 0.6f, 1f, target.transform.position.z),
        //    new Vector3(target.transform.position.x, 1f, target.transform.position.z),
        //    new Vector3(target.transform.position.x + 0.6f, 1f, target.transform.position.z)};
        //bulletRb.AddForce(v0[Random.Range(0, v0.Length)].normalized * speed + Vector3.up, ForceMode.Impulse);
        transform.Translate(new Vector3(0,0,1) * speed * Time.deltaTime);
    }

    public void BackToBoss()
    {
        bulletRb.linearVelocity = Vector3.zero;
        bulletRb.angularVelocity = Vector3.zero;
        bulletRb.Sleep();
        Vector3 bossPos = new Vector3(boss.transform.position.x, boss.transform.position.y, boss.transform.position.z);
        bulletRb.AddForce(bossPos.normalized * speed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Boss"))
        {
            ReturnSpawnPoint();
            boss.GetComponent<BossHP>().BossHPMinus();
        }
    }

    public void ReturnSpawnPoint()
    {
        gameObject.SetActive(false);
        bulletRb.linearVelocity = Vector3.zero;
        bulletRb.angularVelocity = Vector3.zero;
        bulletRb.Sleep();
        isAttacked = false;
        transform.position = ObjectPool.Instance.spawnPoint1.transform.position;
    }
}
