using UnityEngine;
using Cysharp.Threading.Tasks;

public class LockOnDodgeEnemy : MonoBehaviour
{
    public Transform player;
    
    public float dodgeDistance = 5f;  
    public float dodgeSpeed = 5f; 
    public float restTime = 5f; 
    public float maxDodgeAngle = 45f;  

    private bool canDodge = true;
    private Vector3 targetPosition;
    private bool dodging = false;
    private bool isAllowedToDodge = true;
    private float dir;
    private void OnEnable()
    {
        isAllowedToDodge = true;
        this.enabled = true;
    }

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
            
            Vector3 newPos = Vector3.MoveTowards(
                new Vector3(transform.position.x, transform.position.y, 0),   
                new Vector3(targetPosition.x, targetPosition.y, 0),  
                dodgeSpeed * Time.deltaTime
            );

 
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y),
                                  new Vector2(targetPosition.x, targetPosition.y)) < 0.1f)
            {
                dodging = false;
                Rest().Forget();
            }
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
   
    public void TriggerDodge()
    {
        if (!canDodge ||!isAllowedToDodge) return;

        
        float directionSign = Random.value < 0.5f ? -1f : 1f;
        dir = directionSign;


        float angle = Random.Range(0f, maxDodgeAngle) * directionSign;

       
        float theta = angle * Mathf.Deg2Rad;

       
        float x = dodgeDistance * Mathf.Sin(theta);
        float y = 0f;  

        targetPosition = new Vector3(player.position.x + x, player.position.y + y, 0);

        dodging = true;
        canDodge = false;
    }

   

    public async UniTaskVoid Rest()
    {

        await UniTask.WaitForSeconds(restTime);
        canDodge = true;
    }
}
