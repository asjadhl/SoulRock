using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class BossMove : MonoBehaviour
{
   private float moveSpeed = 4f;
    GameObject boss;
   public bool canRun = false;
 
  private void Update()
    {
    if (canRun)
      UpdateBossRun();
    }

    [Obsolete]
    private async UniTask BossRun()
    {
        await UniTask.Delay(3000);
        transform.Translate(new Vector3(0, 0, -1) * moveSpeed * Time.fixedDeltaTime);
    }
    
    private void UpdateBossRun()
    {
    transform.position += moveSpeed * Time.fixedDeltaTime * -transform.forward;
    }
   
}
