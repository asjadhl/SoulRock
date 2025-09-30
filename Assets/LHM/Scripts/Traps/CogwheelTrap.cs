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
    


    public void OnHit()
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}


