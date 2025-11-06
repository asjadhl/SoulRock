using UnityEngine;

public class RollTrap : MonoBehaviour
{
    public Transform roll;
    public Transform rollEmpty; 

    [Header("HP")]
    public int maxHealth = 2;
    int currentHealth;

    [Header("Motion")]

    public float throwLocalY = 0f;  
    public float upOffset = 3f;   
    public float throwSpeed = 10f;

    float upLocalY;                     
    Vector3 rollLocal;                    
    Coroutine closeRoutine;
    private RaycastHit hit;

    void Awake()
    {
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
        if (!roll.gameObject.activeSelf) roll.gameObject.SetActive(true);
        upLocalY = throwLocalY + upOffset;

        rollLocal = roll.localPosition;
        rollLocal.y = upLocalY;
        roll.localPosition = rollLocal;

        if (closeRoutine != null) StopCoroutine(closeRoutine);
        closeRoutine = StartCoroutine(CloseDoorRoutine());
    }
    public void OnHit()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {

            currentHealth--;
            Debug.Log($"HP: {currentHealth}");
            if (currentHealth <= 0)
            {
                roll.gameObject.SetActive(false);
            }
          
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _ = GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
        }
    }
    System.Collections.IEnumerator CloseDoorRoutine()
    {
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
