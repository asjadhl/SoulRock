using Unity.VisualScripting;
using UnityEngine;

public class MiniSkullMove : MonoBehaviour
{
    public Transform skullScaner;
    Animator animator;
    public float forwardSpeed = 2f; 
    public float chaseSpeed = 10f;
    [Header("잡기 시작하는거리")]
    public float chaseStartDistance = 10f;
    [Header("사망 거리")]
    public float disableDistance = 1f;
    [Header("사망 이펙트")]
    [SerializeField] GameObject explosionEffect;
    private bool isChasing = false;
    Vector3 effectTransform;
    void Start()
    {
        if (skullScaner == null)
            skullScaner = GameObject.FindWithTag("SkullScaner").transform;
        animator = GetComponent<Animator>();
        effectTransform = new Vector3(0, 1f, 0);
    }

    void Update()
    {
        SkullMove();
    }

    void SkullMove()
    {
        if (skullScaner == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, skullScaner.position);

        if (!isChasing && distanceToPlayer > chaseStartDistance)
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }
        else if (distanceToPlayer <= chaseStartDistance && distanceToPlayer > disableDistance)
        {
            animator.SetBool("Run", true);
            isChasing = true;
            Vector3 direction = (skullScaner.position - transform.position).normalized;
            transform.position += direction * chaseSpeed * Time.deltaTime;

            //transform.rotation = Quaternion.LookRotation(direction);
            transform.LookAt(skullScaner);
        }
        else if (distanceToPlayer <= disableDistance)
        {
            Vector3 effectPos = transform.position + effectTransform;
            Instantiate(explosionEffect, effectPos, Quaternion.identity);
            gameObject.SetActive(false);
        }

    }
}
