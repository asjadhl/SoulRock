using Cysharp.Threading.Tasks;
using UnityEngine;

public class LockOnDashEnemy : MonoBehaviour
{  
 
 
  public Transform player;

  [Header("Dash Settings")]
  public float restTime = 2f;
  public float dashSpeed = 14f;
  public float minDashLenght = 0f;
  public float maxDashLenght;

  private Vector3 targetPosition;
  private bool Dashing = false;
  private bool canDash = true;
  private bool isAllowedToDash = true;




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
    if (!isAllowedToDash) return;

    if (Dashing)
    {
      //Smoothly move toward target position
      transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);

      UpdateClamp();
      transform.LookAt(player);
      if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
      {
        transform.position = targetPosition;
        Dashing = false;
        Rest().Forget();
      }

    }

  }

  public void TriggerDash()
  {
    if (!canDash || !isAllowedToDash) return;

    float resultDashLenght = UnityEngine.Random.Range(minDashLenght, maxDashLenght);

    
    targetPosition = transform.position+transform.forward * resultDashLenght;


    //Clamp SizeOfMap
    if (GameManager.instance != null)
      targetPosition = GameManager.instance.Clamp(targetPosition);

    float width = Screen.width;
    float height = Screen.height;
    //Clamp ScreenWorld
    targetPosition = Camera.main.WorldToScreenPoint(targetPosition);
    float xclamp = Mathf.Clamp(targetPosition.x, width / 100f * 10, width / 100f * 90);
    float yclamp = Mathf.Clamp(targetPosition.y, height / 100f * 10, height / 100f * 70);
    targetPosition = new Vector3(xclamp, yclamp, targetPosition.z);

    targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
    //transform.LookAt(targetPosition);

    Dashing = true;
    canDash = false;


  }

  private async UniTaskVoid Rest()
  {
    await UniTask.WaitForSeconds(restTime);
    canDash = true;
  }

  public bool IsDodging()
  {
    return Dashing;
  }

  public void StopDodging()
  {
    isAllowedToDash = false;
    this.enabled = false;
  }

  public void StartDodging()
  {
    isAllowedToDash = true;
    this.enabled = true;
  }
}

 