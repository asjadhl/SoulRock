using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GameObject gunObject;
    private GameObject boss;

    [SerializeField] ParticleSystem shootParticle;
    private void Start()
    {
        boss = GameObject.FindWithTag("Boss");
    }

    public void PlayerShoot_()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _=GunMove();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10f))
            {
                if (hit.collider.gameObject.tag == "Enemy")
                    hit.collider.gameObject.GetComponent<BossBullet>().ReturnSpawnPoint();
                //if(hit.collider.gameObject.tag == "Bullet2")
                //    boss.GetComponent<BossAttack>().bullet.GetComponent<BossBullet>().BackToBoss();


            }


            // Health.cs
            if (Physics.Raycast(ray, out hit, 10f))
            {
                IDamagable damagable = hit.collider.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    Debug.Log("Fire");
                    damagable.TakeHit(1000);
                }
            }
        }
    }

    private async UniTask GunMove()
    {
        shootParticle.Play();
        gunObject.transform.Rotate(-15, 0, 0);
        await UniTask.Delay(100);
        GunOriPos();
    }

    private void GunOriPos()
    {
        gunObject.transform.Rotate(15, 0, 0);
    }
}
