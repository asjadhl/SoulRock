using UnityEngine;

public class WindHitBox : MonoBehaviour
{
    WindTrap windTrap;
    RaycastHit hit;

    public void OnHit()
    {
        windTrap = GetComponentInParent<WindTrap>();
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {
            windTrap.StopBox();
        }
    }
}
