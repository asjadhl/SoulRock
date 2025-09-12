using UnityEngine;

public class BossBullet : MonoBehaviour
{
    Rigidbody bulletRb;
    private float speed = 20f;
    private GameObject player;
    private GameObject boss;

    void Awake()
    {
        bulletRb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        boss = GameObject.FindWithTag("Boss");
        ThrowAttack(player.transform);
    }

    private void FixedUpdate()
    {
        if (transform.position.z <= -4.5f)
        {
            Destroy(gameObject);
            player.GetComponent<PlayerHP>().PlayerHPMinus();
        }
    }

    private void ThrowAttack(Transform target)
    {
        Vector3[] v0 = { new Vector3(target.transform.position.x - 0.6f, 1.5f, target.transform.position.z), 
            new Vector3(target.transform.position.x, 1.5f, target.transform.position.z), 
            new Vector3(target.transform.position.x + 0.6f, 1.5f, target.transform.position.z)};
        bulletRb.AddForce(v0[Random.Range(0, v0.Length)].normalized * speed + Vector3.up, ForceMode.Impulse);
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
        if(other.gameObject.CompareTag("Boss"))
        {
            Destroy(gameObject);
            boss.GetComponent<BossHP>().BossHPMinus();
        }
    }
}
