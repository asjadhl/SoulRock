using UnityEngine;
using Cysharp.Threading.Tasks;

public class LockOnDodgeEnemy : MonoBehaviour
{
    public Transform player;

    [Header("Dodge Settings")]
    public float dodgeDistance = 5f;      
    public float dodgeSpeed = 5f;          
    public float restTime = 2f;            
    public float minDodgeLenght = 0f;
    public float maxDodgeLenght;

    private Vector3 targetPosition;
    private bool dodging = false;
    private bool canDodge = true;
    private bool isAllowedToDodge = true;


    
 
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        
    }

  void UpdateClamp()
  {
    float width = Screen.width; // 100
    float height = Screen.height; //100
    //Clamp ScreenWorld
    Vector3 newresult = Camera.main.WorldToScreenPoint(targetPosition);
    float xclamp = Mathf.Clamp(newresult.x, width / 100f * 10, width / 100f * 90);
    float yclamp = Mathf.Clamp(newresult.y, height / 100f * 10, height / 100f * 70);


    newresult = new Vector3(xclamp, yclamp, newresult.z);


    targetPosition = Camera.main.ScreenToWorldPoint(newresult);
     

  }
  private void Update()
    {
        if (!isAllowedToDodge) return;

     if (dodging)
        {
             //Smoothly move toward target position
             transform.position = Vector3.MoveTowards(transform.position, targetPosition, dodgeSpeed * Time.deltaTime);

         UpdateClamp();
      transform.LookAt(player);
      if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
        {
                    transform.position = targetPosition;
                    dodging = false;
                    Rest().Forget();  
        }
      
     }
        
    }
  //private void Update() // Old
  //{
  //  if (!isAllowedToDodge) return;

  //  if (dodging)
  //  {
  //    //Smoothly move toward target position
  //    transform.position = Vector3.MoveTowards(transform.position, targetPosition, dodgeSpeed * Time.deltaTime);
  //    float width = Screen.width;
  //    float height = Screen.height;
  //    //Clamp ScreenWorld
  //    Vector3 newresult = Camera.main.WorldToScreenPoint(transform.position);
  //    float xclamp = Mathf.Clamp(newresult.x, width / 100f * 10, width / 100f * 90);
  //    float yclamp = Mathf.Clamp(newresult.y, height / 100f * 10, height / 100f * 70);
  //    transform.position = Camera.main.ScreenToWorldPoint(newresult);
  //    if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
  //    {
  //      transform.position = targetPosition;
  //      dodging = false;
  //      Rest().Forget();
  //    }
  //  }
  //}
  public void TriggerDodge()
    {
        if (!canDodge || !isAllowedToDodge) return;

        float resultDodgeLenght = UnityEngine.Random.Range(minDodgeLenght, maxDodgeLenght);
    
        float x = resultDodgeLenght; 
              x *= UnityEngine.Random.Range(0, 2) < 1 ? -1 : 1;
        float y = resultDodgeLenght;
              y *= UnityEngine.Random.Range(0, 2) < 1 ? -1 : 1;
    targetPosition = new Vector3(x+transform.position.x,y + transform.position.y, transform.position.z);
     
    
    //Clamp SizeOfMap
     if(GameManager.instance != null)
     targetPosition = GameManager.instance.Clamp(targetPosition);

    float width = Screen.width ;
    float height = Screen.height ;
    //Clamp ScreenWorld
    targetPosition = Camera.main.WorldToScreenPoint(targetPosition);
    float xclamp = Mathf.Clamp(targetPosition.x, width / 100f * 10, width / 100f * 90);
    float yclamp = Mathf.Clamp(targetPosition.y, height / 100f * 10, height / 100f * 70);
    targetPosition = new Vector3(xclamp, yclamp, targetPosition.z);

    targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
    //transform.LookAt(targetPosition);
      
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
