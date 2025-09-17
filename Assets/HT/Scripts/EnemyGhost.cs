using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Animations;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;


public interface IDying
{
    public void PlayDyingAnimation(bool _);
    public bool IsDying { get; set; }
}

public enum State
{
    Underground,Spawn,Idle, Forward, Attack, Die,SpawnR,Null
}



public delegate void GhostAction();
public class EnemyGhost : MonoBehaviour, IDying
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

    public bool IsDying { get; set; }

    [Header("Animation")]
    [SerializeField]
    private Animator m_anim;
    [SerializeField]
    private State My_State;
    [SerializeField]
    private int AttackIndex;
    [Space(2)]
    public List<string> ListClipName;
    public Dictionary<string, AnimationClip> m_eventDic;
    [SerializeField]
    private CancellationTokenSource cts;
    private CancellationTokenSource master_cts;
    private int activetask = 0;
    [SerializeField]
    float Attackrate = 2f;
    [SerializeField]
    float timer = 0;


  
    private void Awake()
    {



        transform.eulerAngles = new Vector3(0,  Random.Range(0,180), 0);
        //transform.LookAt(Vector3.back);
        PlayerPos = GameObject.FindWithTag("Player").transform;
        if (PlayerPos == null)
        {
            gameObject.SetActive(false);

            Destroy(gameObject);

        }
        if (m_anim == null)
            m_anim = GetComponent<Animator>();
        if (m_anim == null)
        {
            Debug.LogError($"GameObject :{this.name}'s Animator is missing");
            gameObject.SetActive(false);
            
        }




        //Prototype
        int i = 0;
        m_eventDic = new Dictionary<string, AnimationClip>();
        if (m_eventDic != null && ListClipName != null)
        {
            foreach (var child in ListClipName)
            {
                if (child != "")
                    m_eventDic.Add(child, GetClipByName(child));
                else
                    m_eventDic.Add(i++.ToString(), null);
            }
        }

        cts = new CancellationTokenSource();
        master_cts = new CancellationTokenSource();


           My_State = State.Null;

        AnimationManager(State.Idle, master_cts.Token, () =>
        {
            ghostAction = AlertUpdate;

        }).Forget();
        
    }

     
    
   private void AlertUpdate()
    {
         
       // if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= StartNoticingRange)
        if(Vector3.Distance(PlayerPos.position,transform.position) <= StartAlertingRange)
        {
            transform.LookAt(PlayerPos.position);
            transform.position += m_speed * Time.deltaTime * transform.forward;
            AnimationManager(State.Forward,cts.Token).Forget();
            //if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= StartAttackingRange)
            if (Vector3.Distance(PlayerPos.position, transform.position) <= StartAttackingRange)
            {
                //Stick with Player
                transform.SetParent(PlayerPos.transform);
                AnimationManager(State.Idle, cts.Token).Forget();
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

                AnimationManager(State.Attack, cts.Token, () =>
                {

                    //Attack
                    Attack();
                    PlayDyingAnimation(true);

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



    public void PlayDyingAnimation(bool _)
    {
        IsDying = _;
        ghostAction = null;
        transform.SetParent(null);

        
        AnimationManager(State.Die, master_cts.Token, () => {


            Destroy(gameObject);
        }
        ,80).Forget();

        UniScaleChangeOverTime(cts.Token).Forget();
    }


    private void OnDestroy()
    {
        cts.Cancel();
      //  m.material.color = origin.color;
    }

    void Interact(System.Action callback = null)
    {
        if (PlayerPos != null)
            callback?.Invoke();
        else //플레이어가 없을때 자신 없앤다
        {
            PlayDyingAnimation(false);
        }
    }

    #region Animation_System
    
    public async UniTaskVoid UniLaterCall(float time,CancellationToken token, System.Action callback = null)
    {
      
        bool canceled = false;

        try
        {
            await UniTask.WaitForSeconds(time, cancellationToken: token);
        }
        catch (System.OperationCanceledException)
        {
            canceled = true; 
        }
        finally
        { 
           if(!canceled)
               callback?.Invoke();
        }
        
    }

    public async UniTaskVoid AnimationManager(State _newState,CancellationToken token, System.Action callback = null, float sample = 0)
    {

        if (My_State == _newState || My_State == State.Die)
            return;
        else
            My_State = _newState;


        if(activetask >= 1)
        {
          
            cts.Cancel();
        }
        switch (My_State)
        {
            case State.Underground:
                m_anim.Play(ListClipName[0]);
                break;
            case State.Spawn:
                m_anim.Play(ListClipName[1]);
                break;

            case State.Idle:
                m_anim.Play(ListClipName[2]);
                break;

            case State.Forward:
                m_anim.Play(ListClipName[3]);
                break;
            case State.Attack:
                m_anim.Play(ListClipName[4]);
                break;
            case State.Die:
                 m_anim.Play(ListClipName[5]);
                break;
            case State.SpawnR:
                m_anim.Play(ListClipName[6]);
                break;
        }

        if (sample > 0)
        {
            float waitTime = sample / m_eventDic[ListClipName[(int)_newState]].frameRate; ;
            await UniTask.WaitForSeconds(waitTime, cancellationToken: token);
        }
        else
            await UniTask.Yield();

        callback?.Invoke();
    }
    AnimationClip GetClipByName(string clipName)
    {
        foreach (var clip in m_anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) return clip;
        }
        return null;
    }

    public async UniTaskVoid UniPlayOneShot(State _newState, CancellationToken token, State returnState)
    {
        bool canceled = false;        

        try
        {
            m_anim.Play(ListClipName[(int)_newState]);
            if (m_eventDic[ListClipName[(int)_newState]])
            {
                await UniTask.WaitForSeconds(m_eventDic[ListClipName[(int)_newState]].length, cancellationToken: token);
            }
        }
        catch (System.OperationCanceledException)
        {
            canceled = true;
            Debug.Log("UniPlayOneShot-Cancel");
        }
        finally
        {
            if (!canceled)
            {
                AnimationManager(returnState,token).Forget();
            }

        }
    }

    
    public async UniTaskVoid UniTriggerAtSample(State _newState,int sample, CancellationToken token,bool IsMaster, System.Action callback = null)
    {
        float waitTime = sample / m_eventDic[ListClipName[(int)_newState]].frameRate;

        bool canceled = false;

        try
        {
            if(!IsMaster)
            activetask++;
            await UniTask.WaitForSeconds(waitTime, cancellationToken: token);
        }
        catch(System.OperationCanceledException)
        {
            canceled = true;
            Debug.Log("UniTrig-Cancel");
        }
        finally
        {
            if (!IsMaster)
                activetask--;
            if (!canceled) 
            callback?.Invoke();
        }
       
    }

    public async UniTaskVoid UniScaleChangeOverTime(CancellationToken token)  
    {
        float scales = 1;
        Vector3 Origin = transform.localScale;
        while (true)
        {
            scales -= Time.deltaTime;
            if (scales <= 0)
                break;
            transform.localScale = Origin * scales;
            await UniTask.WaitForSeconds(Time.deltaTime,cancellationToken: token);   
        }



        
    }
   
  

    #endregion
}




