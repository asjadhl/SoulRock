using UnityEngine;

public class DoorTrap : MonoBehaviour
{
    public Transform doorFrame; 
    public Transform door;    

    [Header("HP")]
    public int maxHealth = 3;
    int currentHealth;

    [Header("Motion (Local)")]
    [Tooltip("문이 완전히 닫혔을 때의 로컬 Y (문틀 기준)")]
    public float closedLocalY = 0f;      
    [Tooltip("닫힌 위치보다 얼마나 위에서 시작해 내려올지")]
    public float openOffset = 3f;
    [Tooltip("닫히는 속도")]
    public float closeSpeed = 10f;

    float openLocalY;                 
    Vector3 doorLocal;             
    Coroutine closeRoutine;
    public GameObject player;   

    void Awake()
    {
 
        if (door != null && doorFrame != null && door.parent != doorFrame)
        {
            door.SetParent(doorFrame, worldPositionStays: true);
        }
        player = GameObject.FindWithTag("Player").GetComponent<GameObject>();
    }

    void OnEnable()
    {
        ResetAndClose();
    }

    void OnDisable()
    {
        if (closeRoutine != null)
        {
            StopCoroutine(closeRoutine);
            closeRoutine = null;
        }
    }

    void ResetAndClose()
    {
        if (door == null || doorFrame == null) return;

        currentHealth = maxHealth;

 
        if (!door.gameObject.activeSelf) door.gameObject.SetActive(true);
        openLocalY = closedLocalY + openOffset;

        doorLocal = door.localPosition;
        doorLocal.y = openLocalY;
        door.localPosition = doorLocal;

        if (closeRoutine != null) StopCoroutine(closeRoutine);
        closeRoutine = StartCoroutine(CloseDoorRoutine());
    }

    System.Collections.IEnumerator CloseDoorRoutine()
    {
        while (door != null && Mathf.Abs(door.localPosition.y - closedLocalY) > 0.01f)
        {
            doorLocal = door.localPosition;
            float nextY = Mathf.MoveTowards(doorLocal.y, closedLocalY, closeSpeed * Time.deltaTime);
            door.localPosition = new Vector3(doorLocal.x, nextY, doorLocal.z);
            yield return null;
        }
        closeRoutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentHealth = 0;
            OnHit();
            _ = GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
        } 
    }

    


    public void OnHit()
    {
        currentHealth--;
        if (currentHealth <= 0 && door != null)
        {
            door.gameObject.SetActive(false); 
            Debug.Log("문 파괴");
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (doorFrame == null) return;
        Gizmos.color = Color.green;
        var pOpen = doorFrame.TransformPoint(new Vector3(0, closedLocalY + openOffset, 0));
        Gizmos.DrawSphere(pOpen, 0.05f);
        Gizmos.color = Color.red;
        var pClose = doorFrame.TransformPoint(new Vector3(0, closedLocalY, 0));
        Gizmos.DrawSphere(pClose, 0.05f);
    }
#endif
}
