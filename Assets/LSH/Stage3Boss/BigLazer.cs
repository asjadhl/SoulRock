using UnityEngine;

public class BigLazer : MonoBehaviour
{
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf == true)
        {
            bigLazerMove();
        }
    }

    void bigLazerMove()
    {
        transform.LookAt(player.position);
        transform.Translate(Vector3.forward * 20 * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider col)
    {
        //if (col.CompareTag("Mirror"))
        //{
        //    gameObject.SetActive(false);
        //}
        if(col.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
