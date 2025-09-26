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
    public bool isSetData = false;
    
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
    protected  EnemyGhostGraphics EnemyGhostGraphics;
    private CancellationTokenSource cts;
    [Space(10)]
    public float offsetEyessight;
    public Enemy()
    { }
   public virtual void  FactoryReset()
    {
        MyBehavior = Behavior.Wondering; 
    }
    public void SetData(string data)
    {
        this.datas =    data;
        EntityType =    datas[0].ToString();
        EntityIS    =    datas[1].ToString();
        isSetData  =    true;
    }
    private void OnEnable()
    {
        FactoryReset();
       
    }
    private void Awake()
    {
        m_Awake();
    }

    public virtual void m_Awake()
    { }
    private void Start()
    {
        PlayerTransform = GameObject.FindWithTag("Player").transform;
        lockOnDodgeEnemy = GetComponent<LockOnDodgeEnemy>();
        EnemyGhostGraphics = GetComponent<EnemyGhostGraphics>();
        m_Start();
    }
    public virtual void m_Start()
    { }
    private void Update()
    {
        if (isSetData)
        {
            m_Update();
            DestroyWhenBehind();
        }
         
    }
    public virtual void m_Update()
    { }
    public virtual void Die()
    {

        
    }
    private async UniTaskVoid LifeTime(CancellationToken token)
    {
        await UniTask.WaitForSeconds(20f, cancellationToken: token);
        gameObject.SetActive(false);
    }


  public void LookAt(Vector3 target)
  {
    target.y = offsetEyessight;
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
            gameObject.SetActive(false);
    }

    
}


 