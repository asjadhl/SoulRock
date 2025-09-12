using UnityEngine;

public class PlayerShootsss : MonoBehaviour
{

    
    private void Update()
    {
        PlayerShoot_();
    }

    public void PlayerShoot_()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            

            // Health.cs
            if (Physics.Raycast(ray, out hit, 100f))
            {
                IDamagable damagable = hit.collider.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.TakeHit(8);
                }
            }
        }
    }
}
