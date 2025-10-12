using Cysharp.Threading.Tasks;
using UnityEngine;

public class BossMove : MonoBehaviour
{
   private float moveSpeed = 8f;

    private void Update()
    {
        _=BossRun();
    }

    private async UniTask BossRun()
    {
        await UniTask.Delay(3000);
        transform.Translate(new Vector3(0, 0, -1) * moveSpeed * Time.fixedDeltaTime);
    }
}
