using UnityEngine;

public class CamController : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        CamMove();
    }

    private void CamMove()
    {
        if (player == null) return;

        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z - 0.5f);
        transform.rotation = player.transform.rotation;
    }
}
