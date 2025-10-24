using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody playerRb;
    private float moveSpeed = 4f;

    //float playerJumpForce = 13f;

    //bool isGrounded = true;

    [SerializeField] Transform[] StartPos;
    [SerializeField] float QuadraticBezierRate = 0.6f;
    private float t = 0f;
    bool canRun = false;
    BossMove temp;
    private void Start()
    {
        temp = GameObject.FindAnyObjectByType<BossMove>();
        playerRb = GetComponent<Rigidbody>();
        WaitToRun().Forget();
    }

    private void Update()
    {
    //_=PlayerRun();
    //if (isGrounded && Input.GetMouseButtonDown(1))
    //    PlayerJump();
    if (canRun)
      UpdatePlayerRun();
    }
 
    
    void UpdatePlayerRun()
    {
       transform.position +=  moveSpeed * Time.deltaTime * transform.forward;
      
    }

  [Obsolete]
    private async UniTask PlayerRun()
    {
        //StartMove();
        await UniTask.Delay(3000);
        transform.Translate(new Vector3(0,0,1) * moveSpeed * Time.fixedDeltaTime);
    }

   private async UniTaskVoid WaitToRun()
   {
      while(t <=1f)
      {
      StartMove(t+=Time.deltaTime* QuadraticBezierRate);
      await UniTask.WaitForFixedUpdate();
      }
      await UniTask.Delay(3000);
      canRun = true;
      temp.canRun = true;
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

    private void StartMove(float t)
    {        
             // if(t<1) t+= Time.deltatime* speed ???
            transform.position = QuadraticBezier(StartPos[0].position, StartPos[1].position, StartPos[2].position, t);
            
    }

}
