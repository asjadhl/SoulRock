using UnityEngine;

public class Timing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Clicker();
    }
    void Clicker()
    {
       if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("클릭 감지");
        }
    }
}
