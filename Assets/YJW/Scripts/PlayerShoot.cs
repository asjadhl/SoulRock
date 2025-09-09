using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Camera camera;
    private void Update()
    {
        //PlayerShoot_();
    }

    public void PlayerShoot_()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject.tag == "Enemy")
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
