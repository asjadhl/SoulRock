using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GameObject gunObject;
    [SerializeField] GameObject boss2;

    private bool isRecoilPlaying = false;
    [SerializeField] private bool isFever = false; 
    [SerializeField] private float feverRecoilMultiplier = 0.3f; 
    [SerializeField] private float feverRecoverMultiplier = 3f;
    [SerializeField] ParticleSystem shootParticle;
    public Transform gunTransform;
    public float kickbackDistance = 0.1f;
    public float kickbackSpeed = 10f;
    BossMove bossMove;
    private Vector3 originalPosition;

    private Dictionary<string, Action<Collider>> _tagActions;


    void Start()
    {
        originalRotation = gunObject.transform.localRotation;
        bossMove = GameObject.FindAnyObjectByType<BossMove>();
        InitializeTagActions();
    }

    private void Update()
    {
        //    PlayerShoot_();
    }

    private void InitializeTagActions()
    {
        _tagActions = new Dictionary<string, Action<Collider>>
        {
            { "Dummy", col => {
                var dummy = col.GetComponent<DummySpawner>();
                dummy.getDummyHit = true;
                dummy.getDummyDamage();
            }},
            { "SuperBigBall", col => col.GetComponent<BigLazer>().BallHpMin() },
            { "LazerBall", col => col.gameObject.SetActive(false) },
            { "Skull", col => col.GetComponent<MiniSkullMove>().ShootReturnSkull() },
            { "Stage2Boss", col => {
                var bossAttack = boss2.GetComponent<Stage2BossAttack>();
                if (bossAttack.curShape == Shape.D)
                {
                    bossAttack.playerHitCount++;
                    Vector3 bossPos = new Vector3(col.transform.position.x, col.transform.position.y + 7f, col.transform.position.z);
                    Stage1ParticleManager.Instance.PlayDEffect(bossPos);
                }
            }},
            { "RedCard", col => col.GetComponent<CardMove>().CardGetDam() },
            { "GoldCard", col => col.GetComponent<CardMove>().CardGetDam() },
            { "miniH", col => col.GetComponent<MiniBoss>().ReturnOriPos().Forget() },
            { "GhostBall", col => col.GetComponent<LastBossMove>().HitGhostBoss().Forget() },
            { "Poltergeist", col => col.gameObject.SetActive(false) },
            { "FakeClone", col => col.GetComponent<CloneMove>().OnHit() },
            { "miniHTrue", col => col.GetComponent<MiniBoss>().miniHTrue().Forget() },
            { "Wheel", col => boss2.GetComponent<Stage2BossAttack>().WheelStop() },
            { "RealClone", col => {
                var clone = col.GetComponent<GBAttack>();
                clone.ReturnClone();
                clone.SuccessFindRealClone();
            }},
        };
    }


    public void PlayerShoot_()
    {
        Kickback().Forget();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            HandleHit(hit.collider);
        }
    }

    private void HandleHit(Collider col)
    {
        if (col == null) return;

        // 태그 기반 행동
        if (_tagActions.TryGetValue(col.tag, out var action))
        {
            action.Invoke(col);
        }

        // IDamagable 처리 (공통 데미지)
        if (col.TryGetComponent<IDamagable>(out IDamagable damageable))
        {
            damageable.TakeHit(8);
        }
        else
        {
            col.GetComponentInChildren<IDamagable>()?.TakeHit(8);
        }
    }

    private Quaternion originalRotation;

    //private async UniTask Kickback()
    //{
    //    if (isRecoilPlaying) return;
    //    shootParticle.Play();

    //    Vector3 targetPos = originalPosition - gunTransform.forward * kickbackDistance;

    //    gunObject.transform.localRotation = originalRotation * Quaternion.Euler(-15, 0, 0);

    //    while (Vector3.Distance(gunTransform.localPosition, targetPos) > 0.01f)
    //    {
    //        gunTransform.localPosition = Vector3.Lerp(
    //            gunTransform.localPosition,
    //            targetPos,
    //            kickbackSpeed * Time.deltaTime
    //        );
    //        await UniTask.Yield(PlayerLoopTiming.Update);
    //    }
    //    gunObject.transform.localRotation = originalRotation;

    //    while (Vector3.Distance(gunTransform.localPosition, originalPosition) > 0.01f)
    //    {
    //        gunTransform.localPosition = Vector3.Lerp(
    //            gunTransform.localPosition,
    //            originalPosition,
    //            kickbackSpeed * Time.deltaTime
    //        );
    //        await UniTask.Yield(PlayerLoopTiming.Update);
    //    }
    //}


    private async UniTask Kickback()
    {
        if (isRecoilPlaying) return;
        isRecoilPlaying = true;

        shootParticle.Play();

        float appliedKickback = kickbackDistance * (isFever ? feverRecoilMultiplier : 1f);
        float appliedSpeed = kickbackSpeed * (isFever ? feverRecoverMultiplier : 1f);

        Vector3 targetPos = originalPosition - gunTransform.forward * appliedKickback;

        gunObject.transform.localRotation = originalRotation * Quaternion.Euler(-15, 0, 0);


        while (Vector3.Distance(gunTransform.localPosition, targetPos) > 0.01f)
        {
            gunTransform.localPosition = Vector3.Lerp(
                gunTransform.localPosition,
                targetPos,
                appliedSpeed * Time.deltaTime
            );
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        gunObject.transform.localRotation = originalRotation;

        while (Vector3.Distance(gunTransform.localPosition, originalPosition) > 0.01f)
        {
            gunTransform.localPosition = Vector3.Lerp(
                gunTransform.localPosition,
                originalPosition,
                appliedSpeed * Time.deltaTime
            );
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        isRecoilPlaying = false;
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

