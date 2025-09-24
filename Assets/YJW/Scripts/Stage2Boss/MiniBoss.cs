using Unity.VisualScripting;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    [SerializeField] Vector3 spawnPos;
    private bool isSpawned = false;

    [SerializeField] GameObject boss;
    [SerializeField] GameObject player;

    private void FixedUpdate()
    {
        if(gameObject.activeSelf == true)
        {
            if(isSpawned == false)
            {
                SetRanPos();
                transform.position = spawnPos;
                isSpawned = true;
            }
            if (Mathf.Abs(transform.position.z - player.transform.position.z) >= 10)
                transform.Translate(Vector3.forward * 3 * Time.fixedDeltaTime);
            Debug.Log(Mathf.Abs(transform.position.z - player.transform.position.z));
        }
    }

    private void SetRanPos()
    {
        spawnPos = boss.GetComponent<Stage2BossAttack>().SetMiniBossRanPos();
    }
}
