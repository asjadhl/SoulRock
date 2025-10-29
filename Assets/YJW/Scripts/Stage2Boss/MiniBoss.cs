using Cysharp.Threading.Tasks;
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

    private bool moveDone = false;

    private void Start()
    {
        x = transform.position.x;
        y = transform.position.y;
    }

    private void Update()
    {
        if (gameObject.activeSelf == true)
        {
            if (isSpawned == false)
            {
                SetRanPos().Forget();
                isSpawned = true;
            }

            if (!moveDone)
            {
                float distanceZ = Mathf.Abs(transform.position.z - player.transform.position.z);

                if (distanceZ > 11f)
                {
                    float speed = 3f;
                    Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, player.transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                }
                else
                {
                    moveDone = true;
                }
            }
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
        moveDone = false;
        gameObject.SetActive(false);
    }

    public async UniTask miniHTrue()
    {
        boss.GetComponent<Stage2BossAttack>().HeartTrue();
        //Transform cap = transform.GetChild(0);
        Transform joker = transform.GetChild(1);
        Vector3 currentPos = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        Stage1ParticleManager.Instance.PlayCEffect(currentPos);
        while (joker.localScale.y < 1.5f)
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
