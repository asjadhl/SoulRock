using UnityEngine;

public class RedBlackBallMove : MonoBehaviour
{
    float x = 0;
    float y = 0;
    private Vector3 oriPos;

    [SerializeField] GameObject boss;
    [SerializeField] GameObject player;

    private float moveSpeed = 6;


    private void Awake()
    {
        x = transform.position.x;
        y = transform.position.y;
    }

    private void FixedUpdate()
    {
        if (gameObject.activeSelf == true)
        {
            oriPos = new Vector3(x, y, boss.transform.position.z);
            BallMove();
        }
    }

    private void BallMove()
    {
        transform.Translate(0, 0, -1 * moveSpeed * Time.fixedDeltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (gameObject.tag == "BlackBall")
            {
                Stage2BossAttack.clubStack++;
            }
            ReturnOriPos();
        }
    }

    public void ReturnOriPos()
    {
        transform.position = oriPos;
        gameObject.SetActive(false);
    }
}
