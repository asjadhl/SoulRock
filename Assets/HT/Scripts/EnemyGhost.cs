using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public interface IDying
{
    public void Die(bool _);
    public bool IsDying { get; set; }
}

public interface IResetable
{
    public void Resetable();
    
}

public enum State
{
    Underground,Spawn,Idle, Forward, Attack, Die,SpawnR,Null
}
public delegate void GhostAction();
public class EnemyGhost : MonoBehaviour, IDying, IResetable
{


    [SerializeField]
    private Transform PlayerPos;

    [SerializeField]
    private float StartAlertingRange;
    [SerializeField]
    private float StartAttackingRange;
    [SerializeField]
    GhostAction ghostAction;
    [SerializeField]
    [Range(-10f, 10f)] float m_speed;
    [SerializeField]
    int m_damage;
    public EnemyGhostGraphics m_enemygraphics;

    public bool IsDying { get; set; }
    
    [SerializeField]
    private int AttackIndex;
    
    
    [SerializeField]
    float Attackrate = 2f;
    [SerializeField]
    float timer = 0;
    Vector3 Origin;
    LockOnDodgeEnemy lockOnDodgeEnemy;




    private void Awake()
    {


        m_enemygraphics = GetComponent<EnemyGhostGraphics>();
        transform.eulerAngles = new Vector3(0, Random.Range(0, 180), 0);
        //transform.LookAt(Vector3.back);
        PlayerPos = GameObject.FindWithTag("Player").transform;
        if (PlayerPos == null)
        {
            gameObject.SetActive(false);

            Destroy(gameObject);

        }
        m_enemygraphics.AnimationManager(State.Idle, Cts.master, () =>
        {
            GetComponent<LockOnDodgeEnemy>().StopDodging(); // Stop Dodging If Ghost is not aware of Player Existance
            ghostAction = AlertUpdate;

        }).Forget();
        lockOnDodgeEnemy = GetComponent<LockOnDodgeEnemy>();
    }


 
    private void AlertUpdate()
    {
         
       // if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= StartNoticingRange)
        if(Vector3.Distance(PlayerPos.position,transform.position) <= StartAlertingRange)
        {
            lockOnDodgeEnemy.StartDodging();

            if (!lockOnDodgeEnemy.IsDodging())
            transform.LookAt(PlayerPos.position);


            transform.position += m_speed * Time.deltaTime * transform.forward;
            m_enemygraphics.AnimationManager(State.Forward, Cts.normal).Forget();
            //if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= StartAttackingRange)
            if (Vector3.Distance(PlayerPos.position, transform.position) <= StartAttackingRange)
            {
                //Stick with Player
                transform.SetParent(PlayerPos.transform);
                m_enemygraphics.AnimationManager(State.Idle, Cts.normal).Forget();
                lockOnDodgeEnemy.StopDodging();
                ghostAction = AttackingUpdate;

            }
        }
        
            

    }

    void AttackingUpdate()
    { 
            timer = Mathf.Clamp(timer + Time.deltaTime, 0f, 10f);
            if (timer >= Attackrate)
            { 
                timer = 0;
              if(TryGetComponent<Collider>(out var c))
                       c.enabled = false;
             else
             {
                    GetComponentInChildren<Collider>().enabled = false;          
             }

            m_enemygraphics.AnimationManager(State.Attack, Cts.normal, () =>
                {

                    //Attack
                    Attack();
                    Die(true);

                }, 40).Forget();
            }      
    }

    void Attack()
    {
        if (PlayerPos.TryGetComponent<IDamagable>(out var damagable))
        {
            damagable?.TakeHit(m_damage);
        }
    }
    private void Update()
    {


         Interact(() => { ghostAction?.Invoke(); });
    }


    
  public  void Restart()
    {
        transform.eulerAngles = new Vector3(0, Random.Range(0, 180), 0);
        transform.localScale = new Vector3(1, 1, 1);
        if (gameObject.TryGetComponent(out Health c))
        {
            c.m_CurrentHealth = c.m_MaxHealth;

        }
        else
            Debug.Log("No Health.cs");
        m_enemygraphics.SetRandomColor();

        if (TryGetComponent<Collider>(out var f))
            f.enabled = true;
        else
        {
            GetComponentInChildren<Collider>().enabled = false;
        }
        m_enemygraphics.My_State = State.Idle;
        ghostAction = AlertUpdate;
    }
    public void  Resetable()
    {
        transform.eulerAngles = new Vector3(0, Random.Range(0, 180), 0);
        transform.localScale = new Vector3(1, 1, 1);
        if (gameObject.TryGetComponent(out Health c))
        {
            c.m_CurrentHealth = c.m_MaxHealth;

        }
        else
            Debug.Log("No Health.cs");
        m_enemygraphics.SetRandomColor();

        if (TryGetComponent<Collider>(out var f))
            f.enabled = true;
        else
        {
            GetComponentInChildren<Collider>().enabled = false;
        }
        m_enemygraphics.My_State = State.Idle;
        ghostAction = AlertUpdate;
    }
    public void Die(bool _)
    {
        IsDying = _;
        ghostAction = null;
        transform.SetParent(null);


        m_enemygraphics.AnimationManager(State.Die, Cts.master, () => {


             gameObject.SetActive(false);  
        }     
        ,80).Forget();

        m_enemygraphics.UniScaleChangeOverTime(0).Forget();
    }


   

    void Interact(System.Action callback = null)
    {
        if (PlayerPos != null)
            callback?.Invoke();
        else //플레이어가 없을때 자신 없앤다
        {
            Die(false);
        }
    }
 
}




