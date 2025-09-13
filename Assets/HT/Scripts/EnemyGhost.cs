using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.PackageManager.UI;
using UnityEngine;


public interface IDying
{
    public void PlayDyingAnimation(bool _);
    public bool IsDying { get; set; }
}

enum State
{
    Null, underground, Spawn, Idle, Forward, Fire, Die
}

[System.Serializable]
public class AnimationClipEvent
{
    public string clip_name;
    AnimationClip clip;
    public void SetClip(AnimationClip _clip)
    {
        clip = _clip;
    }
    public AnimationClip GetClip()
    {
        return clip;
    }
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
    private int  FireProjectileIndex;
    [Space(2)]
    public List<AnimationClipEvent> m_events;
    [SerializeField]
    private List<Coroutine> activeRoutine;
    [SerializeField]
    float Attackrate = 2f;
    [SerializeField]
    float timer = 0;

   

    private void Awake()
    { 

       

         transform.eulerAngles = new Vector3(0, 180,  0);
        //transform.LookAt(Vector3.back);
        PlayerPos = GameObject.FindWithTag("Player").transform;
        if (PlayerPos == null)
            gameObject.SetActive(false);

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

        //Get Clip and Event
        if (m_events != null)
        {
            foreach (AnimationClipEvent child in m_events)
            {
                child.SetClip(GetClipByName(child.clip_name));
            }
        }
        activeRoutine = new List<Coroutine>();
        My_State = State.Null;
        AnimationManager(State.underground);
        StartCoroutine(TriggerAtSample(m_events.Find(p => p.clip_name == "Underground").GetClip(), 20, () =>
         {
             AnimationManager(State.Spawn);
             StartCoroutine(TriggerAtSample(m_events.Find(p => p.clip_name == "Spawn").GetClip(), 25, () =>
             {
                 AnimationManager(State.Idle);
                 ghostAction = NoticingUpdate;
             }));

              



         }));
        
    }

 



      void NoticingUpdate()
    {
        if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= StartNoticingRange)
        {
            transform.LookAt(PlayerPos.position);
           
            transform.position += (MapScrollSpeed+m_speed) * Time.deltaTime * transform.forward;
            AnimationManager(State.Forward);


              if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= StartAttackingRange)
              {
                ghostAction = AttackingUpdate;
                AnimationManager(State.Idle);
              }

        }
        else
            transform.position += MapScrollSpeed * Time.deltaTime * Vector3.back;

    }
      
    void AttackingUpdate()
    {
        transform.LookAt(PlayerPos.position);
        timer = Mathf.Clamp(timer + Time.deltaTime, 0f, 10f);


        if (timer >= Attackrate)
        {

            timer = 0;
            AnimationManager(State.Fire);
        }

        //transform.position +=  MapScrollSpeed * Time.deltaTime * Vector3.back;

    }
     
   
    private void Update()
    {  ghostAction?.Invoke();
    }



    public void PlayDyingAnimation(bool _)
    {
        IsDying = _;
        AnimationManager(State.Die);
    }







    #region Animation_System 
    IEnumerator LaterCall(float time, System.Action callback = null)
    {
        yield return new WaitForSeconds(time);

        callback?.Invoke();

    }
    void AnimationManager(State _newState)
    {

        if (My_State == _newState || My_State == State.Die)
            return;
        else
            My_State = _newState;

        if (activeRoutine.Count >= 1)
        {
            for (int i = 0; i < activeRoutine.Count; i++)
            {
                StopCoroutine(activeRoutine[i]);
                activeRoutine[i] = null;
            }
            activeRoutine.Clear();
        }


        switch (My_State)
        {
            case State.underground:
                //activeRoutine.Add(StartCoroutine(PlayOneShot("Underground", State.Spawn)));
                m_anim.Play("Underground");
                break;

            case State.Spawn:
                //activeRoutine.Add(StartCoroutine(PlayOneShot("Spawn", State.Idle)));
                m_anim.Play("Spawn");
                break;

            case State.Idle:
                m_anim.Play("Idle");
                break;

            case State.Forward:
                m_anim.Play("Dash Forward In Place");
                break;
            case State.Fire:
                m_anim.Play("Power Shoot Attack");
                IDamagable idamage = PlayerPos.GetComponent<IDamagable>();
                idamage?.TakeHit(m_damage);
                activeRoutine.Add(StartCoroutine(TriggerAtSample(m_events.Find(p => p.clip_name == "Power Shoot Attack").GetClip(), 15,
                     () =>
                     {  
                         //new void
                         GameObject newprojectile = Instantiate(GameManager.instance.GetProjectTiles[FireProjectileIndex], transform.position, Quaternion.identity);
                          
                         IBullet bullet = newprojectile.GetComponent<IBullet>();
                         if (bullet != null)
                             bullet.Init(PlayerPos.transform);
                         else
                             Destroy(newprojectile);

                         //request
                         StartCoroutine(LaterCall(1f,()=>
                         {
                            
                             m_anim.Play("SpawnR");
                             StartCoroutine(TriggerAtSample(m_events.Find(p => p.clip_name == "SpawnR").GetClip(), 25,() => Destroy(gameObject)));
                         }));
                     }

                     )));
                break;
            case State.Die:

                m_anim.Play("Die");
                StartCoroutine(TriggerAtSample(m_events.Find(p => p.clip_name == "Die").GetClip(), 40,
                    () =>
                    {
                        Destroy(this.gameObject);
                    }
                    ));

                break;




        }
    }
    AnimationClip GetClipByName(string clipName)
    {
        foreach (var clip in m_anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) return clip;
        }
        return null;
    }

    private System.Collections.IEnumerator PlayOneShot(string clipName, State returnState)
    {
        m_anim.Play(clipName);

        var clip = GetClipByName(clipName);
        if (clip != null)
        {
            yield return new WaitForSeconds(clip.length);
        }


        AnimationManager(returnState);

    }
    private IEnumerator TriggerAtSample(AnimationClip clip, int sample, System.Action callback = null)
    {

        float waitTime = sample / clip.frameRate;


        yield return new WaitForSeconds(waitTime);


        callback?.Invoke();
    }
    #endregion
}


