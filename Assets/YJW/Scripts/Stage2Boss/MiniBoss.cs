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

    float x;
    float y;
    Vector3 oriPos;

    private void Start()
    {
        x = transform.position.x;
        y = transform.position.y;
    }

    private void FixedUpdate()
    {
        if(gameObject.activeSelf == true)
        {
            oriPos = new Vector3(x, y, boss.transform.position.z);
            if(isSpawned == false)
            {
                _=SetRanPos();
                isSpawned = true;
            }
            if (Mathf.Abs(transform.position.z - player.transform.position.z) >= 11)
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

    public async UniTask ReturnOriPos()
    {
        Vector3 InstPos = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
        Stage1ParticleManager.Instance.PlayBoxEffect(InstPos);
        await UniTask.Delay(150);
        transform.position = oriPos;
        isSpawned = false;
        gameObject.SetActive(false);
    }

    public async UniTask miniHTrue()
    {
        boss.GetComponent<Stage2BossAttack>().HeartTrue();
        //Transform cap = transform.GetChild(0);
        Transform joker = transform.GetChild(1);
        while(joker.localScale.y < 1.5f)
        {
            joker.localScale += new Vector3(0, 0.1f, 0);
            await UniTask.Delay(20);
        }
    }

    public void miniHTureReturnOriState()
    {
        Transform joker = transform.GetChild(1);
        joker.localScale =  new Vector3(1, 0.5f, 1);
    }
}
