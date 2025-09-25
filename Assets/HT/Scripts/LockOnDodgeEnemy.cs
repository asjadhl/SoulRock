using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public class LockOnDodgeEnemy : MonoBehaviour
{
    public Transform player;
    
    public float dodgeDistance = 5f;  
    public float dodgeSpeed = 5f; 
    public float restTime = 5f;
    public float minDodgeAngle = 0;
    public float maxDodgeAngle = 45f;  

    private bool canDodge = true;
    private Vector3 targetPosition;
    private bool dodging = false;
    private bool isAllowedToDodge = true;
    private float dir;
    

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        
    }
    public bool IsDodging()
    {
        return dodging;
    }
    void Update()
    {

        


        if (dodging && isAllowedToDodge)
        {


            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y),
                                  new Vector2(targetPosition.x, targetPosition.y)) < 0.2f)
            {
                Debug.Log("a");
                dodging = false;
                Rest().Forget();
            }


           
            
            Vector3 newPos = Vector3.MoveTowards(
                new Vector3(transform.position.x, transform.position.y, 0),   
                new Vector3(targetPosition.x, targetPosition.y, 0),  
                dodgeSpeed * Time.deltaTime
            );

                  
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

            
        }

        
        //transform.LookAt(player);
    }

    public void StopDodging()
    {
        isAllowedToDodge = false;
        this.enabled = false;
    }
    public void StartDodging()
    {
        isAllowedToDodge = true;
        this.enabled = true;
    }

    float x = 0;
    float y = 0;
    public int cou = 0;

    public void TriggerDodge()
    {
         if (!canDodge ||!isAllowedToDodge) return;

        // float directionSign = Random.value < 0.5f ? -1f : 1f;

       
 
        
      //  dir = directionSign;
        

        float angle = Random.Range(minDodgeAngle, maxDodgeAngle); //* directionSign;

       
        float theta = angle * Mathf.Deg2Rad;

        int[] dir = { -1, 1 };
        int resultX = dir[Random.Range(0, 2)];
        int resultY = dir[Random.Range(0, 2)];
          
          x += dodgeDistance * Mathf.Sin(theta) * resultX; 
          y += dodgeDistance * Mathf.Sin(theta) * resultY;  

        targetPosition = new Vector3(player.position.x + x, player.position.y + y, transform.position.z);
       


        

        targetPosition =  Camera.main.WorldToViewportPoint(targetPosition);
        Debug.Log($"ViewPort: {targetPosition}");
        targetPosition = new Vector3(Mathf.Clamp(targetPosition.x, 0.1f, 0.9f), Mathf.Clamp(targetPosition.y, 0.1f, 0.9f), targetPosition.z);
        Debug.Log($"ViewPort-Clamp: {targetPosition}");



        targetPosition = Camera.main.ViewportToWorldPoint(targetPosition);
        x = targetPosition.x;
        y = targetPosition.y;
        
        dodging = true;
        canDodge = false;
        transform.LookAt(targetPosition);
    }

   

    public async UniTaskVoid Rest()
    {

        await UniTask.WaitForSeconds(restTime);
        canDodge = true;
    }
}
