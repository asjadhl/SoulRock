using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
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
public class EnemyGhost : MonoBehaviour, IDying
{


    [SerializeField]
    private Transform PlayerPos;

    [SerializeField]
    private float AttackTriggeredRange;
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
        StartCoroutine(Wait(0.5f));

    }

    IEnumerator Wait(float time)
    {
        float temp_speed = m_speed;
        m_speed = 0;
        yield return new WaitForSeconds(time);
        m_speed = temp_speed;
    }



    public bool CloseToPlayer()
    {
        if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= AttackTriggeredRange)
        {

            return true;
        }
        else
            return false;




    }

    private void Update()
    {

        transform.LookAt(PlayerPos.position);



        timer = Mathf.Clamp(timer + Time.deltaTime, 0f, 10f);

        if (CloseToPlayer())
        {
            if (timer >= Attackrate)
            {

                timer = 0;
                AnimationManager(State.Fire);
            }
        }
        else
        {
            transform.position += m_speed * Time.deltaTime * transform.forward;
            AnimationManager(State.Forward);
        }
    }



    public void PlayDyingAnimation(bool _)
    {
        IsDying = _;
        AnimationManager(State.Die);
    }







    #region Animation_System 

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
                activeRoutine.Add(StartCoroutine(PlayOneShot("Underground", State.Spawn)));
                break;

            case State.Spawn:
                activeRoutine.Add(StartCoroutine(PlayOneShot("Spawn", State.Idle)));
                break;

            case State.Idle:
                m_anim.Play("Idle");
                break;

            case State.Forward:
                m_anim.Play("Dash Forward In Place");
                break;
            case State.Fire:
                activeRoutine.Add(StartCoroutine(PlayOneShot("Power Shoot Attack", State.Idle)));
                IDamagable idamage = PlayerPos.GetComponent<IDamagable>();
                idamage?.TakeHit(m_damage);
                activeRoutine.Add(StartCoroutine(TriggerAtSample(m_events.Find(p => p.clip_name == "Power Shoot Attack").GetClip(), 15,
                     () =>
                     {
                         GameObject newprojectile = Instantiate(GameManager.instance.GetProjectTiles[FireProjectileIndex], transform.position, Quaternion.identity);
                          
                         IBullet bullet = newprojectile.GetComponent<IBullet>();
                         if (bullet != null)
                             bullet.Init(PlayerPos.transform);
                         else
                             Destroy(newprojectile);
                     }

                     )));
                break;
            case State.Die:

                m_anim.Play("Die");
                StartCoroutine(TriggerAtSample(m_events.Find(p => p.clip_name == "Die").GetClip(), 40,
                    () =>
                    {
                        Destroy(gameObject);
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


