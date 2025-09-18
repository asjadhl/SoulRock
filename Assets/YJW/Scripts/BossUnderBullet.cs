using UnityEngine;

public class BossUnderBullet : MonoBehaviour
{
    Rigidbody bulletRb;
    private float speed = 20f;
    private GameObject player;
    private GameObject boss;

    private bool isAttacked = false;
    private bool setActiveSelf = false;

    void Awake()
    {
        bulletRb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        boss = GameObject.FindWithTag("Boss");
    }

    private void FixedUpdate()
    {

        if (gameObject.activeSelf == true && isAttacked == false)
        {
            UnderAttack(player.transform);
            isAttacked = true;
        }

        if (transform.position.z <= player.transform.position.z)
            ReturnSpawnPoint();
    }

    private void UnderAttack(Transform target)
    {
        Vector3 v0 = new Vector3(-1, 0, 0);
        bulletRb.AddTorque(v0.normalized * speed * 30, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ReturnSpawnPoint();
            player.GetComponent<PlayerHP>().PlayerHPMinus();
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
