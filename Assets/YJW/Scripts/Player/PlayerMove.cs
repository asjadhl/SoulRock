using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody playerRb;
    private float moveSpeed = 1f;

    //float playerJumpForce = 13f;

    //bool isGrounded = true;

    [SerializeField] Transform[] StartPos;
    [SerializeField] float speed = 1.0f;
    private float t = 0f;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _=PlayerRun();
        //if (isGrounded && Input.GetMouseButtonDown(1))
        //    PlayerJump();
    }

    private async UniTask PlayerRun()
    {
        StartMove();
        await UniTask.Delay(3000);
        transform.Translate(new Vector3(0,0,1) * moveSpeed * Time.fixedDeltaTime);
    }

    
    private void PlayerJumpButtonClick()
    {

    }
    
    //public void PlayerJump()
    //{
    //    isGrounded = false;
    //    playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
    //    playerRb.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
    //}

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Ground"))
        //{
        //    isGrounded = true;
        //}
    }

    Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(a, b, t);
    }

    private void StartMove()
    {
        if (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.position = QuadraticBezier(StartPos[0].position, StartPos[1].position, StartPos[2].position, t);
        }
    }

}
