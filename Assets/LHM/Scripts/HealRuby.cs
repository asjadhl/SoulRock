using UnityEngine;

public class HealRuby : MonoBehaviour
{
    public Transform endline;
    public Transform rubytransform;
    public GameObject healRuby;
    

    public int healAmount = 1;
    public int MoveSpeed = 30;
    public int maxHeal = 10;

    public float rotateSpeed = 50f;
    public float SpawnTime;

    void Start()
    {
        healRuby = this.gameObject;


    }
    void Update()
    {
        MoveCorridors();
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        if (rubytransform.position.z < endline.position.z)
        {
            RubyRespawn();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
        }
    }

    void RubyRespawn()
    {
        healRuby.SetActive(true);
        rubytransform.position = new Vector3(3, 7, 239);
    }
    void MoveCorridors()
    {
        if (healRuby != null)
        {
            healRuby.transform.Translate(Vector3.back * MoveSpeed * Time.deltaTime);
        }
    }
}
