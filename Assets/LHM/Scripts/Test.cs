using UnityEngine;

public class Test : MonoBehaviour
{
    public float speed = 10;

   
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

    }
}
