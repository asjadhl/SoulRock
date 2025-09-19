using UnityEngine;

public class BossMove : MonoBehaviour
{
    private float moveSpeed = 1f;

    private void Update()
    {
        BossRun();
    }

    private void BossRun()
    {
        transform.Translate(new Vector3(0, 0, -1) * moveSpeed * Time.fixedDeltaTime);
    }
}
