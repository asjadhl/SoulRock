using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float playerJumpForce = 13f;
    private Rigidbody playerRb;
    private bool isGrounded = true;
    private float moveSpeed = 1f;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        PlayerJump();
        PlayerRun();
    }

    private void PlayerRun()
    {
        transform.Translate(new Vector3(0,0,1) * moveSpeed * Time.fixedDeltaTime);
    }

    private void PlayerJump()
    {
        if(isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
            playerRb.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Player is grounded.");
        }
    }
}
