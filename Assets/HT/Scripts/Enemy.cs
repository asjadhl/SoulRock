using Cysharp.Threading.Tasks;
using System.Net.Http.Headers;
using System.Threading;
using UnityEngine;



public enum Behavior
{
   Null, Wondering, Alert, Attack
} 
public class Enemy : MonoBehaviour
{
  
    //datas   
    string datas;
   protected string EntityType; //Walker = 0, Fly = 1
   protected string EntityIS;  // Friendly, chonglib, Hostile
 
    
    [SerializeField]
    protected float Attackrate = 2f;
    protected float timer = 0;
    [SerializeField]
    protected float m_speed;
    [SerializeField]
    protected int m_damage;
    [SerializeField]
    protected float StartAlertingRange;
    [SerializeField]
    protected float StartAttackingRange;
    protected Behavior MyBehavior;
    protected Transform PlayerTransform;
    protected LockOnDodgeEnemy lockOnDodgeEnemy;
    protected  EnemyGraphics EnemyGhostGraphics;
     
    [Space(10)]
    public float offsetEyessight;
    public Enemy()
    { }
   public virtual void  FactoryReset()
    {
      
    }
  
    private void OnEnable()
    {
    MyBehavior = Behavior.Wondering;
                 FactoryReset();
       
    }
    private void Awake()
    {
        m_Awake();
 
    }

    public virtual void m_Awake()
    { }

    private void Init()
    {
    PlayerTransform = GameObject.FindWithTag("Player").transform;
    lockOnDodgeEnemy = GetComponent<LockOnDodgeEnemy>();
    EnemyGhostGraphics = GetComponent<EnemyGraphics>();
  }
    private void Start()
    {
        
        Init();
        m_Start();
    }
    public virtual void m_Start()
    { }
    private void Update()
    {
       
            m_Update();
            DestroyWhenBehind();
        
         
    }
    public virtual void m_Update()
    { }
    public virtual void DieMethod()
    {

        
    }
 
  public void ActiveParticales(int index,bool Renderer = false)
  {
    var mat = GetComponentInChildren<SkinnedMeshRenderer>();
    mat.enabled = Renderer;
    Vector3 result  = new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z);
          GameManager.instance.GetParticale(index, result);
  }
  public GameObject GetActiveParticales(int index, bool Renderer = false)
  {
    var mat = GetComponentInChildren<SkinnedMeshRenderer>();
    mat.enabled = Renderer;
    Vector3 result = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    return GameManager.instance.GetParticale(index, result);
  }
  public void Die()
  {
    Destroy(gameObject);
  }
  public void Attack()
  {
    if (PlayerTransform.TryGetComponent<IDamagable>(out var damagable))
    {
      damagable?.TakeHit(m_damage);
    }
     
  }
  public void LookAt(Vector3 target)
  {
     target.y += offsetEyessight;
    transform.LookAt(target);
  }
    public void DestroyWhenBehind()
    {
        var dir = transform.position - PlayerTransform.position;
                   //Ez= 5  Pz=7 =   dir= Ez-Pz = -2
        var backward = -PlayerTransform.forward;
                          // Pf = 1 to -1
        float distance = Vector3.Dot(dir, backward);
    //0*0+0*0-2*-1// 2 
    if (distance >= 2f)
      Die();
    }

    
}


 