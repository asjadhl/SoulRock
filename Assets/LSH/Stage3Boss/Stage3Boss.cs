using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Stage3Boss : MonoBehaviour
{
    [Header("LazerChargeOB")]
    [SerializeField] GameObject chargeLazer;
    [Header("LazerOB")]
    [SerializeField] GameObject lazer;
    [Header("LazerBallOB")]
    [SerializeField] GameObject lazerBall;
    [Header("Pooling")]
    [SerializeField] GameObject[] lazerPool;
    [SerializeField] GameObject[] lazerBallPool;
    [Header("공격 coolTime")]
    [SerializeField] int coolTime = 3000;
    [SerializeField] float firstOfLazerSize = 0.02f;
    Vector3[] thisPos;
    int ranIndex;
    int ranPos;
    //lazer 전용 풀 인덱스
    int poolIndex = 0;
    //lazerBall 전용 풀 인덱스
    int poolBall = 0;
    bool isAttacking = false;

    void Start()
    {
        ReadyforLazerAttack();
        ReadyforLazerBallAttack();
    }



    void ReadyforLazerAttack()
    {
        ranIndex = Random.Range(0, 4);
        lazerPool = new GameObject[3];
        for (int i = 0; i < lazerPool.Length; i++)
        {
            GameObject lazerAttack = Instantiate(lazer, transform.position, Quaternion.identity);
            lazerAttack.transform.parent = transform;
            lazerAttack.SetActive(false);
            lazerPool[i] = lazerAttack;
        }
    }
    void ReadyforLazerBallAttack()
    {
        lazerBallPool = new GameObject[7];
        thisPos = new Vector3[7];
        thisPos[0] =  new Vector3(-5f , 4f, transform.position.z);
        thisPos[1] = new Vector3(5f, 4f, transform.position.z);
        thisPos[2] = new Vector3(-4f, 6f, transform.position.z);
        thisPos[3] = new Vector3(4f, 6f, transform.position.z);
        thisPos[4] = new Vector3(2f, 8f, transform.position.z);
        thisPos[5] = new Vector3(-2f, 8f, transform.position.z);
        thisPos[6] = new Vector3(0, 8.5f, transform.position.z);
        for (int i = 0; i<lazerBallPool.Length; i++)
        {
            GameObject lazerBallAttack = Instantiate(lazerBall, thisPos[i], Quaternion.identity);
            lazerBallAttack.transform.parent = transform;
            lazerBallAttack.SetActive(false);   
            lazerBallPool[i] = lazerBallAttack;
        }
    }

    void Update()
    {
        if (!isAttacking)  
        {
            Boss3Pattern();
        }
    }

    void Boss3Pattern()
    {
        switch (ranIndex)
        {
            case 0:
                _ = ChargeLazerAttack();
                ranIndex = Random.Range(0, 4);
                break;
            case 1:
                _ = secondPattern();
                ranIndex = Random.Range(0, 4);
                break;
            case 2:
                Debug.LogWarning("세번째 패턴");
                ranIndex = Random.Range(0, 4);
                break;
            case 3:
                Debug.LogWarning("네번째 패턴");
                ranIndex = Random.Range(0, 4);
                break;
        }
    }
    private async UniTask ChargeLazerAttack()
    {
        isAttacking = true;
        chargeLazer.SetActive(true);
        for (int i = 1; i < 100; i++)
        {
            chargeLazer.transform.localScale =
                new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * i;
            await UniTask.Delay(20);
        }

        _ = LazerAttack();

        await UniTask.Delay(coolTime);
        ranIndex = Random.Range(0, 4);
        isAttacking = false;
    }

    private async UniTask LazerAttack()
    {
        chargeLazer.SetActive(false);
        lazerPool[poolIndex].transform.position = transform.position;
        lazerPool[poolIndex].SetActive(true);
        await UniTask.Delay(3000);
        ReturnLazer(lazerPool[poolIndex]);
        if (poolIndex >= lazerPool.Length-1)
        {
            poolIndex = 0;
        }
        else
            poolIndex++;
    }
    void ReturnLazer(GameObject lazer)
    {
        lazer.SetActive(false);
        lazer.transform.position = transform.position;
    }

    private async UniTask secondPattern()
    {
        isAttacking = true;
        for (int i = 0; i < lazerBallPool.Length; i++)
        {
            lazerBallPool[i].SetActive(true);
            for (int j = 1; j < 40; j++)
            {
                lazerBallPool[i].transform.localScale = new Vector3(firstOfLazerSize, firstOfLazerSize, firstOfLazerSize) * j;
                await UniTask.Delay(2);
            }
        }
        await UniTask.Delay(5000);
        _ = secondPatternAttack();
        isAttacking = false;
    }
    private async UniTask secondPatternAttack()
    {
        for (int i = 0; i < lazerBallPool.Length; i++)
        {
            lazerBallPool[i].SetActive(false);
        }
        await UniTask.Delay(3000);
    }
}
