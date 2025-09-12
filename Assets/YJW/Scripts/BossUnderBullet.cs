using UnityEngine;

public class BossUnderBullet : MonoBehaviour
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
        UnderAttack(player.transform);
    }

    private void FixedUpdate()
    {
        if (transform.position.z <= -4.2f)
            Destroy(gameObject);
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
            Destroy(gameObject);
            player.GetComponent<PlayerHP>().PlayerHPMinus();
        }
    }
}
