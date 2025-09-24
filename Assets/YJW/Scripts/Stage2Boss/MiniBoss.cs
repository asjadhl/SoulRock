using Cysharp.Threading.Tasks;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    [SerializeField] int spawnPosIndex;
    private bool isSpawned = false;

    [SerializeField] GameObject boss;
    [SerializeField] GameObject player;

    private void FixedUpdate()
    {
        if(gameObject.activeSelf == true)
        {
            if(isSpawned == false)
            {
                SetRanPos();
                isSpawned = true;
            }
            if (Mathf.Abs(transform.position.z - player.transform.position.z) >= 10)
                transform.Translate(Vector3.forward * 3 * Time.fixedDeltaTime);
            //Debug.Log(Mathf.Abs(transform.position.z - player.transform.position.z));
        }
    }

    // 비동기 Position 값 생성
    private async UniTask SetRanPos()
    {
        var usedPos = boss.GetComponent<Stage2BossAttack>().usedPos;

        // 먼저 새로운 spawnPosIndex를 생성
        spawnPosIndex = boss.GetComponent<Stage2BossAttack>().SetMiniBossRanPos();

        // 중복되는 인덱스가 있을 경우 계속해서 새로운 인덱스를 찾음
        while (usedPos.Contains(spawnPosIndex))
        {
            spawnPosIndex = boss.GetComponent<Stage2BossAttack>().SetMiniBossRanPos();
            await UniTask.Delay(10); // 잠시 대기 후 다시 시도
        }

        boss.GetComponent<Stage2BossAttack>().AddList(spawnPosIndex);

        transform.position = boss.GetComponent<Stage2BossAttack>().spawnPos[spawnPosIndex].position;
    }
}
