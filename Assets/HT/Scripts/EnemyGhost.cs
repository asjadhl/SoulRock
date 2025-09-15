using Cysharp.Threading.Tasks;
using System;
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
    Null,Idle, Forward, Attack, Die
}



public delegate void GhostAction();
public class EnemyGhost : MonoBehaviour, IDying
{


    [SerializeField]
    private Transform PlayerPos;

    [SerializeField]
    private float StartNoticingRange;
    [SerializeField]
    private float StartAttackingRange;
    [SerializeField]
    GhostAction ghostAction;
    [SerializeField]
    [Range(0f, 10f)] float MapScrollSpeed;
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



        transform.eulerAngles = new Vector3(0, 180, 0);
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
            gameObject.SetActive(false);
            Debug.LogError($"GameObject :{this.name}'s Animator is missing");
        }


        //Foreign Code
        var va = FindAnyObjectByType<CorridorSpawner>();
        if (va != null)
        {
            MapScrollSpeed = va.moveSpeed;
            m_speed += MapScrollSpeed;
        }

        //Prototype
        m_eventDic = new Dictionary<string, AnimationClip>();
        if (m_eventDic != null && ListClipName != null)
        {
            foreach (var child in ListClipName)
            {
                m_eventDic.Add(child, GetClipByName(child));
            }
        }

        cts = new CancellationTokenSource();
        master_cts = new CancellationTokenSource();


           My_State = State.Null;

        AnimationManager(State.Idle, master_cts.Token, () =>
        {
            ghostAction = NoticingUpdate;

        }).Forget();
    }





    void NoticingUpdate()
    {
        if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= StartNoticingRange)
        {



            transform.LookAt(PlayerPos.position);

            transform.position += (MapScrollSpeed + m_speed) * Time.deltaTime * transform.forward;
            AnimationManager(State.Forward,cts.Token).Forget();


            if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= StartAttackingRange)
            {
                ghostAction = AttackingUpdate;
                AnimationManager(State.Idle, cts.Token).Forget();
            }

        }
        else
            MoveWithMap();

    }

    void AttackingUpdate()
    {
        transform.LookAt(PlayerPos.position);
        timer = Mathf.Clamp(timer + Time.deltaTime, 0f, 10f);


        if (timer >= Attackrate)
        {

            timer = 0;
            AnimationManager(State.Attack,master_cts.Token, () => {

                //Attack
                Attack();


            AnimationManager(State.Die, master_cts.Token, () => { }
            
            ,60).Forget();

            },10).Forget();
        }

        //transform.position +=  MapScrollSpeed * Time.deltaTime * Vector3.back;

    }

    void Attack()
    {
        IDamagable damagable = PlayerPos.GetComponent<IDamagable>();
        if (damagable != null)
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
        AnimationManager(State.Die, master_cts.Token, () => {


            Destroy(gameObject);
        }
        
        ,60).Forget();
    }
    public void MoveWithMap()
    {
        transform.position += MapScrollSpeed * Time.deltaTime * Vector3.back;
    }



    void Interact(System.Action callback = null)
    {
        if (PlayerPos != null)
            callback?.Invoke();
        else //플레이어가 없을때 자신 없앤다
        {
            Instantiate(GameManager.instance.GetProjectTiles[AttackIndex]);
        }
    }

    #region Animation_System
    [Obsolete]
    IEnumerator LaterCall(float time, System.Action callback = null)
    {
        yield return new WaitForSeconds(time);

        callback?.Invoke();

    }
    public async UniTaskVoid UniLaterCall(float time,CancellationToken token, System.Action callback = null)
    {
      
        bool canceled = false;

        try
        {
            await UniTask.WaitForSeconds(time, cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            canceled = true; 
        }
        finally
        { 
           if(!canceled)
               callback?.Invoke();
        }
        
    }

    public async UniTaskVoid AnimationManager(State _newState,CancellationToken token, System.Action callback = null, float duration = 0)
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
            case State.Idle:
                m_anim.Play(ListClipName[0]);
                break;

            case State.Forward:
                m_anim.Play(ListClipName[1]);
                break;
            case State.Attack:
                m_anim.Play(ListClipName[2]);
                break;
            case State.Die:
                 m_anim.Play(ListClipName[3]);
                break;
        }
            
        if(duration  >= 0)
        await UniTask.WaitForSeconds(duration, cancellationToken: token);
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

    [Obsolete]
    private System.Collections.IEnumerator PlayOneShot(string clipName, State returnState)
    {
        m_anim.Play(clipName);

        var clip = GetClipByName(clipName);
        if (clip != null)
        {
            yield return new WaitForSeconds(clip.length);
        }


       // AnimationManager(returnState);

    }

    public async UniTaskVoid UniPlayOneShot(string clipName, CancellationToken token, State returnState)
    {
        bool canceled = false;
        
       

        try
        {
                m_anim.Play(clipName);
            if (m_eventDic[clipName])
            {
                await UniTask.WaitForSeconds(m_eventDic[clipName].length, cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
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

    [Obsolete]
    private IEnumerator TriggerAtSample(AnimationClip clip, int sample, System.Action callback = null)
    {

        float waitTime = sample / clip.frameRate;


        yield return new WaitForSeconds(waitTime);


        callback?.Invoke();
    }

    public async UniTaskVoid UniTriggerAtSample(string clipName,int sample, CancellationToken token,bool IsMaster, System.Action callback = null)
    {
        float waitTime = sample / m_eventDic[clipName].frameRate;

        bool canceled = false;

        try
        {
            if(!IsMaster)
            activetask++;
            await UniTask.WaitForSeconds(waitTime, cancellationToken: token);
        }
        catch(OperationCanceledException)
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

    #endregion
}




