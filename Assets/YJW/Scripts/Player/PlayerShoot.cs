using Cysharp.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.SceneManagement;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GameObject gunObject;
    [SerializeField] GameObject boss2;

    [SerializeField] ParticleSystem shootParticle;
    public Transform gunTransform;
    public float kickbackDistance = 0.1f;
    public float kickbackSpeed = 10f;
    BossMove bossMove;
    private Vector3 originalPosition;

    private void Update()
    {
        //    PlayerShoot_();
    }
    void Start()
    {
        originalPosition = gunTransform.localPosition;
        bossMove = GameObject.FindAnyObjectByType<BossMove>();
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
                if (hit.collider.gameObject.tag == "Skull")
                    hit.collider.gameObject.GetComponent<MiniSkullMove>().ShootReturnSkull();
                if (hit.collider.gameObject.tag == "Stage2Boss" && boss2.GetComponent<Stage2BossAttack>().curShape == Shape.D)
                {
                    boss2.GetComponent<Stage2BossAttack>().playerHitCount++;
                }
                if (hit.collider.gameObject.tag == "RedCard" || hit.collider.gameObject.tag == "GoldCard")
                {
                    hit.collider.gameObject.GetComponent<CardMove>().CardGetDam();
                }
                if (hit.collider.gameObject.tag == "miniH")
                {
                    hit.collider.gameObject.GetComponent<MiniBoss>().ReturnOriPos();
                }
                if(hit.collider.gameObject.tag == "GhostBall")
                {
                    hit.collider.gameObject.GetComponent<LastBossMove>().HitGhostBoss();
                }
                if(hit.collider.gameObject.tag == "Poltergeist")
                {
                    hit.collider.gameObject.SetActive(false);
                }
                if(hit.collider.gameObject.tag == "FakeClone")
                {
                    hit.collider.gameObject.GetComponent<CloneMove>().OnHit();
                }
                if (hit.collider.gameObject.tag == "miniHTrue")
                {
                    hit.collider.gameObject.GetComponent<MiniBoss>().miniHTrue();
                }
                if(hit.collider.gameObject.tag == "Wheel")
                {
                    boss2.GetComponent<Stage2BossAttack>().WheelStop();
                }
                if(hit.collider.gameObject.tag == "RealClone")
                {
                    hit.collider.gameObject.GetComponent<GBAttack>().ReturnClone();
					hit.collider.gameObject.GetComponent<GBAttack>().SuccessFindRealClone();
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
