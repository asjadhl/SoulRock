using UnityEngine;

public class abcde : MonoBehaviour
{
    Rigidbody rb;
    bool a = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(a == false)  
        {
            rb.AddForce(Vector3.left * 0.1f, ForceMode.Impulse);
            a = true;
        }
    }
}
