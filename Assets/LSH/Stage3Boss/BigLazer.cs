using Unity.VisualScripting;
using UnityEngine;

public class BigLazer : MonoBehaviour
{
    private Transform player;
    MatarialAlpha mirror;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
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
    private void OnTriggerEnter(Collider col)
    {
        //if (col.CompareTag("Mirror"))
        //{
        //    gameObject.SetActive(false);
        //}
        if(col.CompareTag("Player"))
        {
            Debug.LogWarning("큰 레이져 플레이어에 닿았음");
            gameObject.SetActive(false);
        }
        if(col.CompareTag("Mirror") && mirror.successMirror)
        {
            Debug.LogWarning("큰 레이져 거울에 닿았음");
            gameObject.SetActive(false);
            mirror.gameObject.SetActive(false);
        }
    }
}
