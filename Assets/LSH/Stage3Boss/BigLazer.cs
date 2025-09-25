using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class BigLazer : MonoBehaviour
{
    private Transform player;
    private Transform boss;
    MatarialAlpha mirror;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        boss = GameObject.FindWithTag("Boss").GetComponent<Transform>();
        mirror = FindAnyObjectByType<MatarialAlpha>();
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
    
    void reflectMove()
    {
        transform.LookAt(boss.position);
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
            GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();

            gameObject.SetActive(false);
        }
        if(col.CompareTag("Mirror") && mirror.successMirror)
        {
            gameObject.SetActive(false);
            mirror.gameObject.SetActive(false);
        }
        if(col.CompareTag("Boss"))
        {
            gameObject.SetActive(false);
        }
    }
}
