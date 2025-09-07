using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Camera camera;
    private void Update()
    {
        PlayerShoot_();
    }

    private void PlayerShoot_()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
