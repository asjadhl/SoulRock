using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMonstersss : MonoBehaviour
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
    private void Awake()
    {
        PlayerPos = GameObject.FindWithTag("Player").transform;
        if (PlayerPos == null)
            gameObject.SetActive(false);


        m_speed += MapScrollSpeed;
    }

    public bool CloseToPlayer()
    {
        if ((transform.position.z - PlayerPos.position.z) <= AttackTriggeredRange)
        {
            return true;
        }
        else
            return false;
    }

    private void Update()
    {

        transform.position += new Vector3(0, 0, -m_speed) * Time.deltaTime;

        if (CloseToPlayer())
        {
            Attack();
        }
    }


    public void Attack()
    {
        IDamagable idamage = PlayerPos.GetComponent<IDamagable>();
        if (idamage != null)
        {
            idamage.TakeHit(m_damage);
        }

        Destroy(gameObject);
    }
}
