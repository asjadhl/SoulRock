using Cysharp.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.SceneManagement;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GameObject gunObject;
    [SerializeField] GameObject boss1;
    [SerializeField] GameObject boss2;

    [SerializeField] ParticleSystem shootParticle;

    public Transform gunTransform;
    public float kickbackDistance = 0.1f;
    public float kickbackSpeed = 10f;

    private Vector3 originalPosition;

    private void Update()
    {
        //    PlayerShoot_();
    }
    void Start()
    {
        originalPosition = gunTransform.localPosition;
    }


    public void PlayerShoot_()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //_=GunMove();
            StopAllCoroutines();
            StartCoroutine(Kickback());

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject.tag == "Bullet2")
                {
                    hit.collider.gameObject.GetComponent<BossBullet>().BackToBoss();
                    boss1.GetComponent<Stage1BossAttack>().isGreenBallSpawned = false;
                }
                if (hit.collider.gameObject.tag == "EnemyBullet")
                {
                    hit.collider.gameObject.GetComponent<BossBullet>().ReturnSpawnPoint();
                }
                if (hit.collider.gameObject.tag == "Dummy") //요것만 주석처리하면댐
                {
                    hit.collider.gameObject.GetComponent<DummySpawner>().getDummyHit = true;
                    hit.collider.gameObject.GetComponent<DummySpawner>().getDummyDamage();
                }
                if (hit.collider.gameObject.tag == "Mirror")
                {
                    hit.collider.gameObject.GetComponent<MatarialAlpha>().mirrorRotate();
                }

				if (hit.collider.gameObject.tag == "LazerBall")
				{
                    hit.collider.gameObject.SetActive(false);
				}
				if (hit.collider.CompareTag("CloseTrap") || hit.collider.transform.root.CompareTag("CloseTrap"))
                {
                    var pressTrap = hit.collider.GetComponentInParent<PressTrap>();
                    if (pressTrap != null)
                    {
                        pressTrap.OnHit();
                    }
                    var lazertrap = hit.collider.GetComponentInParent<LazerTrap>();
                    if (lazertrap != null)
                    {
                        lazertrap.OnHit();
                    }
                    var rolltrap = hit.collider.GetComponentInParent<RollTrap>();
                    if (rolltrap != null)
                    {
                        rolltrap.OnHit();
                    }
                    var axtrap = hit.collider.GetComponentInParent<AxTrap>();
                    if (axtrap != null)
                    {
                        axtrap.OnHit();
                    }
                    var cogwheeltrap = hit.collider.GetComponentInParent<CogwheelTrap>();
                    if (cogwheeltrap != null)
                    {
                        cogwheeltrap.OnHit();
                    }
                }
                if (hit.collider.gameObject.tag == "TriggerTrap")
                {
                    var windhitbox = hit.collider.gameObject.GetComponent<WindHitBox>();
                    if (windhitbox != null)
                    {
                        windhitbox.OnHit();
                    }
                    var doortrap = hit.collider.GetComponentInParent<DoorTrap>();
                    if (doortrap != null)
                    {
                        doortrap.OnHit();
                    }
                }
                if (hit.collider.gameObject.tag == "Bullet3")
                    hit.collider.gameObject.GetComponent<BossUnderBullet>().ReturnSpawnPoint();
                if (hit.collider.gameObject.tag == "Stage2Boss" && boss2.GetComponent<Stage2BossAttack>().curShape == Shape.D)
                {
                    boss2.GetComponent<Stage2BossAttack>().playerHitCount++;
                }
                if (hit.collider.gameObject.tag == "RedCard" || hit.collider.gameObject.tag == "GoldCard")
                {
                    hit.collider.gameObject.GetComponent<CardMove>().CardGetDam();
                }
                if (hit.collider.gameObject.tag == "BlackBall")
                    hit.collider.gameObject.GetComponent<RedBlackBallMove>().ReturnOriPos();
                if (hit.collider.gameObject.tag == "RedBall")
                {
                    Stage2BossAttack.clubStack++;
                    hit.collider.gameObject.GetComponent<RedBlackBallMove>().ReturnOriPos();
                }
                if (hit.collider.gameObject.tag == "miniH")
                {
                    hit.collider.gameObject.GetComponent<MiniBoss>().ReturnOriPos();
                }
                if (hit.collider.gameObject.tag == "miniHTrue")
                {
                    hit.collider.gameObject.GetComponent<MiniBoss>().miniHTrue();
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
    private IEnumerator Kickback()
    {
        shootParticle.Play();
        Vector3 targetPos = originalPosition - gunTransform.forward * kickbackDistance;
        gunObject.transform.Rotate(-15, 0, 0);
        
        while (Vector3.Distance(gunTransform.localPosition, targetPos) > 0.01f)
        {
            
            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, targetPos, kickbackSpeed * Time.deltaTime);
            yield return null;
        }
        gunObject.transform.Rotate(15, 0, 0);
        

        while (Vector3.Distance(gunTransform.localPosition, originalPosition) > 0.01f)
        {
            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, originalPosition, kickbackSpeed * Time.deltaTime);
            yield return null;
        }
        

        //private async UniTask GunMove()
        //{
        //    shootParticle.Play();
        //    gunObject.transform.Rotate(-15, 0, 0);
        //    await UniTask.Delay(100);
        //    GunOriPos();
        //}

        //private void GunOriPos()
        //{
        //    gunObject.transform.Rotate(15, 0, 0);
        //}
    }
}
