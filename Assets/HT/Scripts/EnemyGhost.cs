using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyGhost : MonoBehaviour
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


    [SerializeField]
    private Animator m_anim;


    private   void Awake()
    {
        
         PlayerPos = GameObject.FindWithTag("Player").transform;
        if (PlayerPos == null)
            gameObject.SetActive(false);

        if(m_anim == null)
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
            //Debug.Log($"{PlayerPos.position.z}  ,  {transform.position.z}");
            //Debug.Log($"{PlayerPos.position.z - transform.position.z}");
            return true;
        }
        else
            return false;

        //if (Vector3.Distance(transform.position,PlayerPos.position) <= AttackTriggeredRange)
        //{
        //    return true;
        //}
        //else
        //    return false;
    }

    private void Update()
    {
        m_anim.SetFloat("MoveZ",  m_speed);
        transform.position += transform.forward*m_speed  * Time.deltaTime;
        
       
       // m_anim.SetFloat("MoveX", -m_speed);

        if (CloseToPlayer())
        {
            Attack();
        }
    }


  public  void DieAnimation()
  {
        //m_anim.Play();
  }

    public void Attack()
    {
        IDamagable idamage = PlayerPos.GetComponent<IDamagable>();
        idamage?.TakeHit(m_damage);

        //Monster Die Animation

        Destroy(gameObject);
    }
}


