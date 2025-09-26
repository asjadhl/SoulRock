using UnityEngine;
using Cysharp.Threading.Tasks;

public class LockOnDodgeEnemy : MonoBehaviour
{
    public Transform player;

    [Header("Dodge Settings")]
    public float dodgeDistance = 5f;      // max world-space dodge per step
    public float dodgeSpeed = 5f;         // world units/sec
    public float restTime = 2f;           // cooldown between dodges
    public float minDodgeLenght = 0f;
    public float maxDodgeLenght;

    private Vector3 targetPosition;
    private bool dodging = false;
    private bool canDodge = true;
    private bool isAllowedToDodge = true;


  public Enemy enemy;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (!isAllowedToDodge) return;

        if (dodging)
        {
             //Smoothly move toward target position
             transform.position = Vector3.MoveTowards(transform.position, targetPosition, dodgeSpeed * Time.deltaTime);

 
              if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
              {
                    transform.position = targetPosition;
                    dodging = false;
                    Rest().Forget();  
              }
        }
    }

    public void TriggerDodge()
    {
        if (!canDodge || !isAllowedToDodge) return;

        float resultDodgeLenght = UnityEngine.Random.Range(minDodgeLenght, maxDodgeLenght);
    
        float x = resultDodgeLenght / 2f; 
              x *= UnityEngine.Random.Range(0, 2) < 1 ? -1 : 1;
        float y = resultDodgeLenght / 2f;
              y *= UnityEngine.Random.Range(0, 2) < 1 ? -1 : 1;
    targetPosition = new Vector3(x+transform.position.x,y + transform.position.y, transform.position.z);
 


     targetPosition = GameManager.instance.Clamp(targetPosition);


    transform.LookAt(targetPosition);
        dodging = true;
        canDodge = false;

        
    }

    private async UniTaskVoid Rest()
    {
        await UniTask.WaitForSeconds(restTime);
        canDodge = true;
    }

    public bool IsDodging()
    {
        return dodging;
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
}
