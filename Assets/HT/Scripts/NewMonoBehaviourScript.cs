using System.Collections;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;
public class NewMonoBehaviourScript : MonoBehaviour
{


    //Boss Tag=Enemy
    //Scripts: EnemyBossController : Enemy



    //AnimatorClip:Hit:Samples(60) Boss[image](sample:3)Event(Void-HitAnimEnd) Boss2[image](6) (9) (12) (15)

    Slider hb_bar;
    Animator bossAnim;
    private int hitAnimCount = 0;
    float hitAnimTime = 0.05f;
    private void HitAnim()
    {
        bossAnim.SetTrigger("Hit");

    }
    private void Start()
    {
        if(TryGetComponent(out Animator c))
            bossAnim = c;

        
    }
    private void Update()
    {
        transform.Translate(new Vector3(0, 0, -0.05f) * Time.deltaTime);
    }

    public void HitAnimEnd()
    {
        if (hitAnimCount < 3)
        {
            hitAnimCount++;
        }
        else
            bossAnim.SetTrigger("None");
    }

}

public class EnemySpawner2:MonoBehaviour
{


    bool isBossSpawn;
    int EnemyCount = 0;

    void SpawnEnemy()
    {
        if(!isBossSpawn)
        {
            //Spawn Normal Enemy
            EnemyCount++;
        }
        else
       {

            //SpawnBoss!!
            EnemyCount = 0;
            isBossSpawn = false;
        }


        if(EnemyCount > 10)
        {
            isBossSpawn = true;
        }
    }
}
