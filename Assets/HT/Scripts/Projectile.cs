using UnityEngine;


 interface IBullet
{
   public void Init(Transform playerPos);
}
public class Projectile : MonoBehaviour, IBullet
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
    public void Init(Transform playerPos)
    {
        targetPos = playerPos;
        transform.LookAt(targetPos);
        Destroy(gameObject, lifetime);
    }

   
    private void Update()
    {
        transform.position += m_speed * Time.deltaTime * transform.forward;
      
        if (Vector3.Distance(transform.position, targetPos.position) <= 0.5f)
        {
            targetPos.gameObject.GetComponent<IDamagable>()?.TakeHit(_damage);
            Destroy(gameObject);
        }
    }


    
}
