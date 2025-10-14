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
        //lockOnDodgeEnemy.StopDodging();
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
                    

                    //if (!lockOnDodgeEnemy.IsDodging())
                        LookAt(PlayerTransform.position);


                    transform.position += m_speed * Time.deltaTime * transform.forward;

                                            
                    EnemyGhostGraphics.AnimationManager(AnimationState.Forward, Cts.normal).Forget();
                    //if (Mathf.Abs(PlayerPos.position.z - transform.position.z) <= StartAttackingRange)
                    if (Vector3.Distance(PlayerTransform.position, transform.position) <= StartAttackingRange)
                    {
            //Stick with Player
            transform.SetParent(PlayerTransform.transform);
            EnemyGhostGraphics.AnimationManager(AnimationState.Idle, Cts.normal).Forget();

                        //lockOnDodgeEnemy.StopDodging();
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
                      Attack();
                        
                      //BYEBYE

                      GetActiveParticales(2).transform.SetParent(PlayerTransform);
                      Die();
 
                    }, 40).Forget();
                }
                break;
        }
    }

    public override void DieMethod()
    {
     if (!CanDie)
        {
      LookAt(PlayerTransform.position);
            GameObject temp =  GetActiveParticales(3,true);
            temp.transform.SetParent(transform);
            temp.transform.localPosition += new Vector3(0, 0, 1);
            temp.transform.SetParent(null);
            CanDie = true;


      var collider = gameObject.GetComponent<Collider>();
      if (collider != null)
        collider.enabled = false;
      else
        GetComponentInChildren<Collider>().enabled = false;

      EnemyGhostGraphics.AnimationManager(AnimationState.DamageDone,Cts.normal).Forget();
            EnemyGhostGraphics.UniTriggerAtSample(AnimationState.DamageDone, 40, Cts.master, true, () =>
        {

          
        MyBehavior = Behavior.Alert;

        //lockOnDodgeEnemy.StartDodging();
          if (collider != null)
            collider.enabled = true;
          else
            GetComponentInChildren<Collider>().enabled = false;

        }).Forget();
           
            return;

     }
        //lockOnDodgeEnemy.StopDodging();
        MyBehavior = Behavior.Null;
        transform.SetParent(null);
        if (TryGetComponent<Collider>(out var c))
            c.enabled = false;
        else
        {
            GetComponentInChildren<Collider>().enabled = false;
        }

        EnemyGhostGraphics.AnimationManager(AnimationState.Die, Cts.master, () => {


          Die();
        }
        , 80).Forget();
     
    //EnemyGhostGraphics.UniScaleChangeOverTime().Forget();
     ActiveParticales(0);
    }


    public override void FactoryReset()
    {

    
    transform.eulerAngles = new Vector3(0, Random.Range(0, 180), 0);
        transform.localScale = new Vector3(1, 1, 1);
        CanDie = false;
      

         var health =  gameObject.GetComponent<Health>();
          if (health == null)
      Debug.LogError("Health.cs NULL");

              health.SetFullHealth();
       
            var mat = GetComponentInChildren<SkinnedMeshRenderer>();
            mat.enabled = true;
     
            mat.material.SetColor("_MainColor", Color.black);
         

       

        var collider = gameObject.GetComponent<Collider>();
            if(collider != null)
               collider.enabled = true;
            else
              GetComponentInChildren<Collider>().enabled = false;

        EnemyGhostGraphics = GetComponent<EnemyGraphics>();
        EnemyGhostGraphics.ResetNow();
         
    }
}