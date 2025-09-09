using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Camera camera;
    private GameObject boss;

    private void Start()
    {
        boss = GameObject.FindWithTag("Boss");
    }
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
            
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if(hit.collider.gameObject.tag == "Enemy")
                    Destroy(hit.collider.gameObject);
                if(hit.collider.gameObject.tag == "Bullet2")
                    boss.GetComponent<BossAttack>().bullet.GetComponent<BossBullet>().BackToBoss();
            }

               
            // Health.cs
            //if (Physics.Raycast(ray, out hit, 100f))
            //{
            //    IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            //    if (damagable != null)
            //    {
            //        damagable.TakeHit(1);
            //    }
            //}
        }
    }
}
