using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float playerJumpForce = 10f;
    private Rigidbody playerRb;
    private bool isGrounded = true;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        PlayerJump();
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
