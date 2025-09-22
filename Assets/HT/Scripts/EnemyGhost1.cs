using UnityEngine;



public enum Behavior
{
    Wondering, Alert, Attack
}
public class EnemyGhost1 : MonoBehaviour
{
    string datas;
    Behavior MyBehavior;
    Transform PlayerTransform;
    LockOnDodgeEnemy lockOnDodgeEnemy;
    System.Action Action;
    public void FactoryReset()
    {
        MyBehavior = Behavior.Wondering;
        Action = chonglib;
    }

    private void OnEnable()
    {
        FactoryReset();
    }
    
    private void Start()
    {
        PlayerTransform = GameObject.FindWithTag("Player").transform;
        lockOnDodgeEnemy = GetComponent<LockOnDodgeEnemy>();
    }
    public void Update()
    {
        
    }

    void Hostile()
    {
        switch (MyBehavior)
        {
            case Behavior.Wondering:
                lockOnDodgeEnemy.StopDodging();
                break;
            case Behavior.Alert:
                lockOnDodgeEnemy.StartDodging();
                break;
            case Behavior.Attack:

                break;

        }
    }

    void chonglib()
    {
        switch (MyBehavior)
        {
            case Behavior.Wondering:
                lockOnDodgeEnemy.StopDodging();
                break;
            case Behavior.Alert:
                lockOnDodgeEnemy.StartDodging();
                break;
            case Behavior.Attack:

                break;

        }
    }
}
