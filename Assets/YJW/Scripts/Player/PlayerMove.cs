using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody playerRb;
    private float moveSpeed = 10f;

    float playerJumpForce = 13f;

    bool isGrounded = true;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        PlayerRun();
        if (isGrounded && Input.GetMouseButtonDown(1))
            PlayerJump();
    }

    private void PlayerRun()
    {
        transform.Translate(new Vector3(0,0,1) * moveSpeed * Time.fixedDeltaTime);
    }

    
    private void PlayerJumpButtonClick()
    {

    }
    
    public void PlayerJump()
    {
        isGrounded = false;
        playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
        playerRb.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

}
