using Cysharp.Threading.Tasks;
using UnityEngine;

public class Stage3Boss : MonoBehaviour
{
    [SerializeField] GameObject chargeLazer;
    [SerializeField] GameObject lazer;
    [SerializeField] GameObject[] lazerPool;
    [Header("공격 쿨타임")]
    [SerializeField] int coolTime = 3000;
    [SerializeField] float firstOfLazerSize = 0.02f;

    int RanIndex;
    int poolIndex = 0;
    bool isAttacking = false;

    void Start()
    {
        RanIndex = Random.Range(0, 4);
        lazerPool = new GameObject[3];
        for (int i = 0; i < lazerPool.Length; i++)
        {
            GameObject lazerAttack = Instantiate(lazer, transform.position, Quaternion.identity);
            lazerAttack.SetActive(false);
            lazerPool[i] = lazerAttack;
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
        switch (RanIndex)
        {
            case 0:
                _ = ChargeLazerAttack();
                break;
            case 1:
                Debug.LogWarning("두번째 패턴");
                RanIndex = Random.Range(0, 4);
                break;
            case 2:
                Debug.LogWarning("세번째 패턴");
                RanIndex = Random.Range(0, 4);
                break;
            case 3:
                Debug.LogWarning("네번째 패턴");
                RanIndex = Random.Range(0, 4);
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
        RanIndex = Random.Range(0, 4);
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
}
