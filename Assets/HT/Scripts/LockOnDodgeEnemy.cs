using UnityEngine;
using Cysharp.Threading.Tasks;

public class LockOnDodgeEnemy : MonoBehaviour
{
    public Transform player;

    [Header("Dodge Settings")]
    public float dodgeDistance = 5f;      // max world-space dodge per step
    public float dodgeSpeed = 5f;         // world units/sec
    public float restTime = 2f;           // cooldown between dodges
    public float minDodgeAngle = 0f;
    public float maxDodgeAngle = 45f;

    private Vector3 targetPosition;
    private bool dodging = false;
    private bool canDodge = true;
    private bool isAllowedToDodge = true;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (!isAllowedToDodge) return;

        if (dodging)
        {
            // Smoothly move toward target position
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, dodgeSpeed * Time.deltaTime);
            transform.position = targetPosition;
            // Check if reached target
          //  if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
          //  {
                dodging = false;
                Rest().Forget(); // start cooldown before next dodge
           // }
        }
    }

    public void TriggerDodge()
    {
        if (!canDodge || !isAllowedToDodge) return;


        float x = maxDodgeAngle;
        x *= UnityEngine.Random.Range(0, 2) < 1 ? -1 : 1;
        targetPosition = new Vector3(player.position.x + x+transform.position.x, player.position.y + transform.position.y, transform.position.z);

        Vector3 screenpoint = Camera.main.WorldToScreenPoint(targetPosition);
        screenpoint.x = Mathf.Clamp(screenpoint.x, 0, Screen.width);


        targetPosition = Camera.main.ScreenToWorldPoint(screenpoint);





        dodging = true;
        canDodge = false;

        transform.LookAt(targetPosition);
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
