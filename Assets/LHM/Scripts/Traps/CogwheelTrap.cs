using UnityEngine;

public class CogwheelTrap : MonoBehaviour 
{

    [Header("HP")]
    public int maxHealth = 10;
    int currentHealth;

    [Tooltip("회전 속도")]
    public float RotationSpeed = 40f;

    void OnEnable()
    {
        currentHealth = maxHealth;
    }
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.left * RotationSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _ = GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
        }
    }

    public void OnHit()
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}


