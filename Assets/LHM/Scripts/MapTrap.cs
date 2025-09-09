using UnityEngine;

public class MapTrap : MonoBehaviour
{
    public int damageAmount = 1;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Debug.Log("¥Í¿Ω");

            //PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            //if (playerHealth != null)
            //{
            //    playerHealth.TakeDamage(damageAmount);
            //}
        }
    }
}
