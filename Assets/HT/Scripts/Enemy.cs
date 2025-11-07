using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public enum Behavior
{
   Null, Wondering, Alert, Attack
} 
public class Enemy : MonoBehaviour
{
  
    
   
 
    
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
    //protected LockOnDodgeEnemy lockOnDodgeEnemy;
    protected  EnemyGraphics EnemyGhostGraphics;

    Func<bool> IsBossDead;
    public Func<bool> IsMode;
    private  Mode mymode;   
    protected Vector3 Save_Scale;
    
    [Space(10)]
    public Vector3  offsetLookAt;
    public Enemy()
    { }
   public virtual void  FactoryReset()
    {
      
    }
  
    private void OnEnable()
    {
    Save_Scale = transform.localScale;
    MyBehavior = Behavior.Wondering;
                 FactoryReset();


      string currentname =  UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        switch (currentname)
        {
            case"Stage2":
                IsBossDead = () => BossState.isBoss1Dead;
                break;
            case "Stage3":
                IsBossDead = () => BossState.isBoss2Dead;
                break;
            case "LastStage":
                IsBossDead = () => BossState.isBoss3Dead;
                break;
        }
      
        CheckBossExist();
   
    }
    public void SetMode(Mode _mode)
    {
             mymode = _mode;
    }
    private void CheckBossExist()
    {
        if(IsBossDead != null && IsBossDead())
        {
#if UNITY_EDITOR
      Debug.Log("CheckBossExist-Destroy");
#endif
      ActiveParticales(0);
            Destroy(gameObject);
        }
       
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
            CheckBossExist();
    


        switch(mymode)
        {
      case Mode.NormalMode:
        if (CircleHit.Instance.isHighLight)
        {
          ActiveParticales(0);
          Destroy(gameObject);
        }
        break;
      case Mode.FeverMode:
        if (!CircleHit.Instance.isHighLight)
        {
          Debug.Log($"FeverMode -!CircleHit.Instance.isHighLight{!CircleHit.Instance.isHighLight}");
          ActiveParticales(0);
          Destroy(gameObject);
        }
        break;
        }
       
     
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
          if(gameObject  != null)
            Destroy(gameObject);
      
  }
  public void Attack()
  {
    //if (PlayerTransform.TryGetComponent<IDamagable>(out var damagable))
    //{
    //  damagable?.TakeHit(m_damage);
    //}

    var PlayerHP = PlayerTransform.GetComponent<PlayerHP>();
    if (PlayerHP != null)
        PlayerHP.PlayerHPMinus().Forget();
     
  }
  public void LookAt(Vector3 target)
  {
     target += offsetLookAt;
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


 