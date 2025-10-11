using UnityEngine;

public class DoorTrap : MonoBehaviour
{
    public Transform doorFrame; // 문틀(부모)
    public Transform door;      // 문(자식) ← 반드시 doorFrame의 자식으로 구성

    [Header("HP")]
    public int maxHealth = 3;
    int currentHealth;

    [Header("Motion (Local)")]
    [Tooltip("문이 완전히 닫혔을 때의 로컬 Y (문틀 기준)")]
    public float closedLocalY = 0f;      // 문틀 기준 닫힘 위치
    [Tooltip("닫힌 위치보다 얼마나 위에서 시작해 내려올지")]
    public float openOffset = 3f;        // 열린 시작 높이 (닫힘 위치 + openOffset)
    [Tooltip("닫히는 속도")]
    public float closeSpeed = 10f;

    float openLocalY;                     // 계산된 열린 시작 위치
    Vector3 doorLocal;                    // 재사용용 버퍼
    Coroutine closeRoutine;
    public GameObject player;   

    void Awake()
    {
        // 안전장치: door가 doorFrame 자식이 아니면 자식으로 붙임 (원인 제거)
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

        // 문을 다시 켜고
        if (!door.gameObject.activeSelf) door.gameObject.SetActive(true);

        // 열린/닫힌 로컬 높이 산출
        openLocalY = closedLocalY + openOffset;

        // 즉시 '열린 위치'로 올려놓기 (로컬 기준)
        doorLocal = door.localPosition;
        doorLocal.y = openLocalY;
        door.localPosition = doorLocal;

        // 코루틴 재시작
        if (closeRoutine != null) StopCoroutine(closeRoutine);
        closeRoutine = StartCoroutine(CloseDoorRoutine());
    }

    System.Collections.IEnumerator CloseDoorRoutine()
    {
        // target: 닫힘 위치의 로컬 Y
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
            Debug.Log("플레이어가 문에 닿음!");
            _ = GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
        } 
    }

    


    public void OnHit()
    {
        currentHealth--;
        if (currentHealth <= 0 && door != null)
        {
            door.gameObject.SetActive(false); // 문만 비활성 (풀 재사용 시 OnEnable로 복구)
            Debug.Log("문 파괴됨!");
        }
    }

#if UNITY_EDITOR
    // 에디터에서 로컬 open/close 위치 확인용
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
