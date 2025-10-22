using UnityEngine;

public class SuperDeathBall : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //플레이어 피격 처리
            Debug.Log("Player Hit by SuperDeathBall");
        }
    }
}
