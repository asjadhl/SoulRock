using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [Range(0f, 10f)] float m_speed;
    [SerializeField]
    int m_damage;


    [SerializeField]
    private Animator m_anim;


    private void Awake()
    {
        
         PlayerPos = GameObject.FindWithTag("Player").transform;
        if (PlayerPos == null)
            gameObject.SetActive(false);


      var va = FindAnyObjectByType<CorridorSpawner>();

        if (va != null)
        {
         MapScrollSpeed = va.moveSpeed;
            m_speed += MapScrollSpeed;
        }
    }

    public bool CloseToPlayer()
    {
        if ((transform.position.z - PlayerPos.position.z) <= AttackTriggeredRange)
        {
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

        transform.position += new Vector3(0, 0, -m_speed) * Time.deltaTime;
        m_anim.SetFloat("MoveX", -m_speed);

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


