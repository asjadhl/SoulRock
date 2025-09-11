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
public class EnemyGhost : MonoBehaviour , IDying
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
    private GameObject FireProjectile;
    [Space(2)]
     public  List<AnimationClipEvent> m_events;
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
            foreach(AnimationClipEvent child in  m_events)
            {
                child.SetClip(GetClipByName(child.clip_name));
            }
        }
        transform.forward = Vector3.back;
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
        if (Mathf.Abs((PlayerPos.position.z - transform.position.z)) <= AttackTriggeredRange)
        {

            return true;
        }
        else
            return false;




    }

    private void Update()
    {
        m_anim.SetFloat("MoveForward", m_speed);
        transform.position += transform.forward * m_speed * Time.deltaTime;


        


        timer = Mathf.Clamp(timer+Time.deltaTime, 0f, 10f);
      
            if (CloseToPlayer())
            {
               if (timer >= Attackrate)
               {
                    timer = 0;
                    Attack();
               }
            }
    }



    public void PlayDyingAnimation(bool _)
    {
        IsDying = true  ;
      
        m_anim.Play("Die", 0, 0f);
        StartCoroutine(TriggerAtSample(m_events.Find(p => p.clip_name == "Die").GetClip(), 40,
            () =>
            {
                Destroy(gameObject);
            }
            
            
            ));
    }

   

    public void Attack()
    {   
        IDamagable idamage = PlayerPos.GetComponent<IDamagable>();
        idamage?.TakeHit(m_damage);
         
        m_anim.Play("Power Shoot Attack", 0, 0f);
        StartCoroutine(TriggerAtSample(m_events.Find(p => p.clip_name == "Power Shoot Attack").GetClip(), 15,
            () =>
            {
                Instantiate(FireProjectile, transform.position, Quaternion.identity);
            }

            ) );
       
    }

 
     
    #region Animation_System 
    AnimationClip GetClipByName(string clipName)
    {
        foreach (var clip in m_anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) return clip;
        }
        return null;
    }
    private IEnumerator TriggerAtSample(AnimationClip clip, int sample, System.Action callback = null)
    {
        
        float waitTime = sample / clip.frameRate;

        
        yield return new WaitForSeconds(waitTime);

    
        callback?.Invoke();
    }
    #endregion
}


