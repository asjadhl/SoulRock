using UnityEngine;


 
public class MeleeAttack : MonoBehaviour 
{
    [SerializeField]
    Transform targetPos;

    [SerializeField]
    private float m_speed;
    [SerializeField]
    [Range(0f, 10f)]
    public float lifetime;
    [SerializeField]
    public int _damage;

    Vector3 pos = new Vector3 (0f, 0f, 0f);
   


    public void Start()
    {
        targetPos = GameObject.FindWithTag("Player").transform;
        transform.LookAt(targetPos);
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += m_speed * Time.deltaTime * transform.forward;

        if (transform != null)
            pos = transform.position;
        else
            Destroy(gameObject);

        if (Vector3.Distance(pos, targetPos.position) <= 0.5f)
        {
            targetPos.gameObject.GetComponent<IDamagable>()?.TakeHit(_damage);
            Destroy(gameObject);
        }
    }


    
}
