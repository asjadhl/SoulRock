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

    }

    private void UnderAttack(Transform target)
    {
        Vector3 v0 = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        bulletRb.AddForce(v0.normalized * speed + Vector3.forward, ForceMode.VelocityChange);
        bulletRb.AddTorque(v0.normalized * speed + Vector3.forward, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            player.GetComponent<PlayerHP>().PlayerHPMinus();
        }
    }
}
