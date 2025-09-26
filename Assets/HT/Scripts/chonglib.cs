using Unity.VisualScripting;
using UnityEngine;

public class chonglib : Enemy
{
    public bool CanDie = true;
    public chonglib()
    { }
    public override void m_Start()
    {  
   
        StartAlertingRange = 100;
        CanDie = false;
        lockOnDodgeEnemy.StopDodging();
        EnemyGhostGraphics.AnimationManager(AnimationState.Idle, Cts.master, () =>
        {

        }).Forget();
    }

    public override void m_Update()
    {
        switch (MyBehavior)
        {
            case Behavior.Wondering:

                break;
            case Behavior.Alert:
                if (Vector3.Distance(PlayerTransform.position, transform.position) <= StartAlertingRange)
                {
                    

                    if (!lockOnDodgeEnemy.IsDodging())
                        LookAt(PlayerTransform.position);


                    transform.position += m_speed * Time.deltaTime * transform.forward;

                                            
                    EnemyGhostGraphics.AnimationManager(AnimationState.Forward, Cts.normal).Forget();
                    //if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= StartAttackingRange)
                    if (Vector3.Distance(PlayerTransform.position, transform.position) <= StartAttackingRange)
                    {
            //Stick with Player
            transform.SetParent(PlayerTransform.transform);
            EnemyGhostGraphics.AnimationManager(AnimationState.Idle, Cts.normal).Forget();

                        lockOnDodgeEnemy.StopDodging();
                         LookAt(PlayerTransform.position);
                        MyBehavior = Behavior.Attack;
            
                    }
                }
                break;
            case Behavior.Attack:
                timer = Mathf.Clamp(timer + Time.deltaTime, 0f, 10f);
                if (timer >= Attackrate)
                {
                    timer = 0;
                    if (TryGetComponent<Collider>(out var c))
                        c.enabled = false;
                    else
                    {
                        GetComponentInChildren<Collider>().enabled = false;
                    }

                    EnemyGhostGraphics.AnimationManager(AnimationState.Attack, Cts.normal, () =>
                    {
                        //Attack
                        if (PlayerTransform.TryGetComponent<IDamagable>(out var damagable))
                        {
                            damagable?.TakeHit(m_damage);
                        }
                        CanDie = true;
                        Die();

                    }, 40).Forget();
                }
                break;
        }
    }

    public override void Die()
    {
        if (!CanDie)
        {
            CanDie = true;
             LookAt(PlayerTransform.position);
            EnemyGhostGraphics.AnimationManager(AnimationState.DamageDone,Cts.normal).Forget();
      EnemyGhostGraphics.UniTriggerAtSample(AnimationState.DamageDone, 40, Cts.master, true, () =>
        {
        MyBehavior = Behavior.Alert;

        lockOnDodgeEnemy.StartDodging();
          
        }).Forget();
           
            return;

        }
        lockOnDodgeEnemy.StopDodging();
        MyBehavior = Behavior.Null;
        transform.SetParent(null);
        if (TryGetComponent<Collider>(out var c))
            c.enabled = false;
        else
        {
            GetComponentInChildren<Collider>().enabled = false;
        }

        EnemyGhostGraphics.AnimationManager(AnimationState.Die, Cts.master, () => {

            
            gameObject.SetActive(false);
        }
        , 80).Forget();

        EnemyGhostGraphics.UniScaleChangeOverTime().Forget();
    }


    public override void FactoryReset()
    {
        base.FactoryReset();
        
        transform.eulerAngles = new Vector3(0, Random.Range(0, 180), 0);
        transform.localScale = new Vector3(1, 1, 1);
        CanDie = false;
        if (gameObject.TryGetComponent(out Health c))
        {
            c.SetFullHealth();
        }
        else
            Debug.Log("No Health.cs");
       
            var mat = GetComponentInChildren<SkinnedMeshRenderer>();
            mat.material.SetColor("_MainColor", Color.black);
         

        if (TryGetComponent<Collider>(out var f))
            f.enabled = true;
        else
        {
            GetComponentInChildren<Collider>().enabled = false;
        }
        EnemyGhostGraphics = GetComponent<EnemyGhostGraphics>();
        EnemyGhostGraphics.ResetNow();
         
    }
}