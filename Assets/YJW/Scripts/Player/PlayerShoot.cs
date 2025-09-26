using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.DualShock.LowLevel;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GameObject gunObject;
    [SerializeField] GameObject boss2;

    [SerializeField] ParticleSystem shootParticle;

    private void Update()
    {
        //    PlayerShoot_();
    }
    public void PlayerShoot_()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _=GunMove();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject.tag == "EnemyBullet")
                {
                    hit.collider.gameObject.GetComponent<BossBullet>().ReturnSpawnPoint();
                }
                if(hit.collider.gameObject.tag == "Dummy") //요것만 주석처리하면댐
                {
                    hit.collider.gameObject.GetComponent<DummySpawner>().getDummyHit = true;
                    hit.collider.gameObject.GetComponent<DummySpawner>().getDummyDamage();
                }
                if(hit.collider.gameObject.tag == "Mirror")
                {
                    hit.collider.gameObject.GetComponent<MatarialAlpha>().mirrorRotate();
                }
                if (hit.collider.CompareTag("CloseTrap") || hit.collider.transform.root.CompareTag("CloseTrap"))
                {
                    var pressTrap = hit.collider.GetComponentInParent<PressTrap>();
                    if (pressTrap != null)
                    {
                        pressTrap.OnHit();
                    }
                    hit.collider.GetComponentInParent<WindTrap>()?.closeWindOnHit();
                    hit.collider.GetComponentInParent<LazerTrap>();
                    hit.collider.GetComponentInParent<DoorTrap>();
                    hit.collider.GetComponentInParent<RollTrap>();
                    hit.collider.GetComponentInParent<AxTrap>();
                    hit.collider.GetComponentInParent<CogwheelTrap>();
                }
                if (hit.collider.gameObject.tag == "OpenTrap")
                {
                    hit.collider.gameObject.GetComponent<WindTrap>();
                    hit.collider.gameObject.GetComponent<LazerTrap>();
                }
                if (hit.collider.gameObject.tag == "TriggerTrap")
                {
                    hit.collider.gameObject.GetComponent<WindHitBox>().OnHit();
                }
                if (hit.collider.gameObject.tag == "Bullet3")
                    hit.collider.gameObject.GetComponent<BossUnderBullet>().ReturnSpawnPoint();
                if (hit.collider.gameObject.tag == "Boss" && boss2.GetComponent<Stage2BossAttack>().curShape == Shape.D)
                {
                    boss2.GetComponent<Stage2BossAttack>().playerHitCount++;
                }
                if(hit.collider.gameObject.tag == "RedCard" || hit.collider.gameObject.tag == "GoldCard")
                {
                    hit.collider.gameObject.GetComponent<CardMove>().CardGetDam();
                }
                if (hit.collider.gameObject.tag == "BlackBall")
                    hit.collider.gameObject.GetComponent<RedBlackBallMove>().ReturnOriPos();
                if(hit.collider.gameObject.tag == "RedBall")
                {
                    Stage2BossAttack.clubStack++;
                    hit.collider.gameObject.GetComponent<RedBlackBallMove>().ReturnOriPos();
                }
                if(hit.collider.gameObject.tag == "Stage2Boss")
                {
                    boss2.GetComponent<BossHP>().BossHPMinus();
                }
                if(hit.collider.gameObject.tag == "miniH")
                {
                    boss2.GetComponent<Stage2BossAttack>().reMiniHMinus();
                    hit.collider.gameObject.GetComponent<MiniBoss>().ReturnOriPos();
                }
            }

            
            // Health.cs
            if (Physics.Raycast(ray, out hit, 100f))
            {

                if (hit.collider.TryGetComponent<IDamagable>(out IDamagable c))
                {
                    c?.TakeHit(8);
                }
                else
                {
                    hit.collider.GetComponentInChildren<IDamagable>()?.TakeHit(8);
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
