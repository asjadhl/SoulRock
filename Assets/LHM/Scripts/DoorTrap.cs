using UnityEngine;

public class DoorTrap : MonoBehaviour
{
    public Transform doorFrame; // 문틀
    public Transform door;      // 실제 문(움직이는 부분)

    public int maxHealth = 10;
    private int currentHealth;

    public float closeSpeed = 1f;
    public float doorHeight = 2f; // 문이 닫힐 높이

    private Vector3 targetPosition;

    void Start()
    {
        currentHealth = maxHealth;

        // 문틀 기준으로 문이 닫히는 위치 계산
        targetPosition = new Vector3(
            door.position.x,
            doorFrame.position.y - doorHeight,
            door.position.z
        );

        StartCoroutine(CloseDoor());
    }

    private System.Collections.IEnumerator CloseDoor()
    {
        while (door != null && Vector3.Distance(door.position, targetPosition) > 0.01f)
        {
            door.position = Vector3.MoveTowards(door.position, targetPosition, closeSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Player"))
        {
            currentHealth = 0; // 플레이어가 닿으면 즉시 파괴
            TakeDamage(0);
            // 플레이어가 닿으면 피해를 입히는 로직 추가 가능
            Debug.Log("플레이어가 문에 닿음!");
        }
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("문이 공격받음! 현재 체력: " + currentHealth);

        if (currentHealth <= 0)
        {
            Destroy(door.gameObject); // 문틀은 남고 문만 제거
            Debug.Log("문 파괴됨!");
        }
    }
}