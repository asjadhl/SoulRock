using UnityEngine;

public class RollTrap : MonoBehaviour
{
    public Transform roll;
    public Transform rollEmpty; // 문틀(부모)

    [Header("HP")]
    public int maxHealth = 2;
    int currentHealth;

    [Header("Motion (Local)")]
    [Tooltip("문이 완전히 닫혔을 때의 로컬 Y (문틀 기준)")]
    public float throwLocalY = 0f;      // 문틀 기준 닫힘 위치
    [Tooltip("닫힌 위치보다 얼마나 위에서 시작해 내려올지")]
    public float upOffset = 3f;        // 열린 시작 높이 (닫힘 위치 + openOffset)
    [Tooltip("닫히는 속도")]
    public float throwSpeed = 10f;

    float upLocalY;                     
    Vector3 rollLocal;                    
    Coroutine closeRoutine;
    private RaycastHit hit;

    void Awake()
    {
        // 안전장치: door가 doorFrame 자식이 아니면 자식으로 붙임 (원인 제거)
        if (roll != null && rollEmpty != null && roll.parent != rollEmpty)
        {
            roll.SetParent(rollEmpty, worldPositionStays: true);
        }
    }
    public void FixedUpdate()
    {
        roll.Rotate(new Vector3(360, 0, 0) * Time.deltaTime);
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
        if (roll == null || rollEmpty == null) return;

        currentHealth = maxHealth;

        // 문을 다시 켜고
        if (!roll.gameObject.activeSelf) roll.gameObject.SetActive(true);

        // 열린/닫힌 로컬 높이 산출
        upLocalY = throwLocalY + upOffset;

        // 즉시 '열린 위치'로 올려놓기 (로컬 기준)
        rollLocal = roll.localPosition;
        rollLocal.y = upLocalY;
        roll.localPosition = rollLocal;

        // 코루틴 재시작
        if (closeRoutine != null) StopCoroutine(closeRoutine);
        closeRoutine = StartCoroutine(CloseDoorRoutine());
    }
    public void OnHit()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {

            currentHealth--;
            Debug.Log($"RollTrap hit by bullet! HP: {currentHealth}");
            if (currentHealth <= 0)
            {
                roll.gameObject.SetActive(false);
            }
          
        }
    }
    System.Collections.IEnumerator CloseDoorRoutine()
    {
        // target: 닫힘 위치의 로컬 Y
        while (roll != null && Mathf.Abs(roll.localPosition.y - throwLocalY) > 0.01f)
        {
            rollLocal = roll.localPosition;
            float nextY = Mathf.MoveTowards(rollLocal.y, throwLocalY, throwSpeed * Time.deltaTime);
            roll.localPosition = new Vector3(rollLocal.x, nextY, rollLocal.z);
            yield return null;
        }
        closeRoutine = null;
    }
#if UNITY_EDITOR
    // 에디터에서 로컬 open/close 위치 확인용
    void OnDrawGizmosSelected()
    {
        if (rollEmpty == null) return;
        Gizmos.color = Color.green;
        var pOpen = rollEmpty.TransformPoint(new Vector3(0, throwLocalY + upOffset, 0));
        Gizmos.DrawSphere(pOpen, 0.05f);
        Gizmos.color = Color.red;
        var pClose = rollEmpty.TransformPoint(new Vector3(0, throwLocalY, 0));
        Gizmos.DrawSphere(pClose, 0.05f);
    }
#endif

}
