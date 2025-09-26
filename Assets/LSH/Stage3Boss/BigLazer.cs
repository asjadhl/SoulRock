using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BigLazer : MonoBehaviour
{
    private Transform player;
    private Transform boss;
    MatarialAlpha mirror;

    bool reflect =false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        boss = GameObject.FindWithTag("Boss").GetComponent<Transform>();
        mirror = FindAnyObjectByType<MatarialAlpha>();
		transform.LookAt(player.position);
	}

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * 20 * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider col)
    {   
        if(col.CompareTag("Player"))
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
            gameObject.SetActive(false);
        }
        if(col.CompareTag("Mirror") && mirror.successMirror)
        {
			transform.LookAt(boss.position);
			mirror.gameObject.SetActive(false);
        }
        if(col.CompareTag("Boss"))
        {
            //데미지 입히는거 넣기
            gameObject.SetActive(false);
        }
    }
}
