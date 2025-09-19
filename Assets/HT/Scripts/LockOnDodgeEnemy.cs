using UnityEngine;
using Cysharp.Threading.Tasks;

public class LockOnDodgeEnemy : MonoBehaviour
{
    public Transform player;
    public float forwardSpeed = 3f;  
    public float dodgeDistance = 5f;  
    public float dodgeSpeed = 5f; 
    public float restTime = 5f; 
    public float maxDodgeAngle = 45f;  

    private bool canDodge = true;
    private Vector3 targetPosition;
    private bool dodging = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        
        Vector3 forwardDir = (player.position - transform.position).normalized;
        transform.position += forwardDir * forwardSpeed * Time.deltaTime;

       
        if (dodging)
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

        
        transform.LookAt(player);
    }

   
    public void TriggerDodge()
    {
        if (!canDodge) return;

        
        float directionSign = Random.value < 0.5f ? -1f : 1f;

        
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
