using UnityEngine;

public class SkullSpawner : MonoBehaviour
{
	[Header("원 도트 프리팹")]
	[SerializeField] GameObject skullPrefab;
	[Header("풀 사이즈")]
	[SerializeField] int poolSize = 10;
	public GameObject[] poolCircle;
	private int pivot = 0;
	private void Awake()
	{
		poolCircle = new GameObject[poolSize];
		for (int i = 0; i < poolSize; i++)
		{
			GameObject skull = Instantiate(skullPrefab, transform.position, Quaternion.identity);
			skull.transform.parent = transform;
			skull.SetActive(false);
			poolCircle[i] = skull;
		}
	}
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
        
    }

	void SpawnSkull()
	{

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
