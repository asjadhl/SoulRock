using UnityEngine;

public class explosionFollow : MonoBehaviour
{
    private float moveSpeed = 4f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, 0, -1) * moveSpeed * Time.fixedDeltaTime);
    }
}
