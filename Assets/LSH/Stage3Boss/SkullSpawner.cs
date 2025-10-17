using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class SkullSpawner : MonoBehaviour
{
	[Header("스껄 프리팹")]
	[SerializeField] GameObject skullPrefab;
	[Header("풀 사이즈")]
	[SerializeField] int poolSize = 10;
	public GameObject[] poolSkull;
	private int pivot = 0;
	bool isSpawn = false;
	[Header("스껄 스폰 시간")]
	[SerializeField] int skullSpawnTime = 5000;
    private void Awake()
	{
        poolSkull = new GameObject[poolSize];
		for (int i = 0; i < poolSize; i++)
		{
			GameObject skull = Instantiate(skullPrefab, transform.position, Quaternion.identity);
			skull.transform.parent = transform;
			skull.SetActive(false);
            poolSkull[i] = skull;
		}
	}
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
        if (!isSpawn)
        {
            isSpawn = true;
            SpawnLoop().Forget();
        }
    }
	private async UniTaskVoid SpawnLoop()
	{
         await SpawnSkull();
         await UniTask.Yield();
    }
	private async UniTask SpawnSkull()
	{
        isSpawn = true;
		
        Vector3 randomPos = transform.position + new Vector3(Random.Range(-6f, 6f), Random.Range(1f, 6f), 0f);
        poolSkull[pivot].transform.position = randomPos;
        poolSkull[pivot].transform.parent = transform;
        poolSkull[pivot].SetActive(true);
        pivot = (pivot + 1) % poolSize;
        await UniTask.Delay(skullSpawnTime);
        isSpawn = false;
    }
    public void ReturnSkull(GameObject skull)
    {
        // 객체를 풀로 반환
        skull.SetActive(false);
        skull.transform.parent = transform;
        skull.transform.position = transform.position;
        skull.transform.rotation = Quaternion.identity;
    }
    
    //void GetSkull()
    //{
    //	GameObject dot = olL[pivot];
    //	dot.SetActive(true);
    //	pivot = (pivot + 1) % poolL.Length;
    //	return dot;
    //}
    //public void ReturnSkull(GameObject skull)
    //{
    //	skull.SetActive(false);
    //	skull.transform.position = transform.position;
    //}
}
