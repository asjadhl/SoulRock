using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.UI;

public class CircleHit : MonoBehaviour
{
	public static CircleHit Instance { get; private set; }
	[Header("타겟 이미지")]
	[SerializeField] Transform targetImage;

	[Header("원크기")]
	public float circleBig = 11f;
	[Header("BPM (박자 속도)")]
	public double bpm = 120.0;

	[Header("판정 거리")]
	[SerializeField] float mindis = 20f;
	[SerializeField] float maxdis = 60f;

	[Header("원 도트 프리팹")]
	[SerializeField] GameObject CirclePrefab;

	[Header("풀 사이즈")]
	[SerializeField] int poolSize = 15;

	[Header("Combo")]
	public int combo = 0;
	[SerializeField] GameObject comboText;

	AudioSource a;
	[SerializeField] AudioClip clip;
	[SerializeField] PlayerShoot playerShoot;
	private PlayerHP playerHPSc;
	public GameObject[] poolCircle;
	private int pivot = 0;
	private double secondsPerBeat;

	private List<CircleMove> activeCircles = new List<CircleMove>();
	public bool getDamage = false;
	CanvasGroup cg;
    private void Awake()
	{
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		poolCircle = new GameObject[poolSize];
		for (int i = 0; i < poolSize; i++)
		{
			GameObject circleDot = Instantiate(CirclePrefab, transform);
			circleDot.SetActive(false);
			poolCircle[i] = circleDot;
		}
		secondsPerBeat = 60.0 / bpm;
		cg = comboText.GetComponent<CanvasGroup>();
	}

	private void Start()
	{
		a = GetComponent<AudioSource>();

		
		//for (int i = 0; i < comboImage.Length; i++)
		//{
		//	comboImage[i].SetActive(false);
		//}
		playerShoot = FindAnyObjectByType<PlayerShoot>();
        playerHPSc = FindAnyObjectByType<PlayerHP>();

        circleGen().Forget();
	}

	private void Update()
	{
		CheckCol();
		//if(combo == 0)
		//{
		//	for (int k = 0; k < comboImage.Length; k++)
		//	{
		//		comboImage[k].SetActive(false);
		//	}
		//}
		//if(Input.GetKeyDown(KeyCode.K)) getDamage = true;
  //      if (Input.GetKeyDown(KeyCode.S)) getDamage = false;


    }

	void CheckCol()
	{
		if (Input.GetMouseButtonDown(0))
		{
			for (int i = activeCircles.Count - 1; i >= 0; i--)
			{
				var circle = activeCircles[i];
				float distance = Vector2.Distance(targetImage.position, circle.hitRect.position);

				if (mindis <= distance && distance <= maxdis)
				{
					Debug.LogWarning("클릭 성공");
					OnClickSuccess().Forget();
					ReturnCircle(circle.gameObject);
					activeCircles.RemoveAt(i);
				}
			}
		}
	}

	//public GameObject GetCircle()
	//{
	//	if (poolCircle.Length == 0)
	//	{
	//		Debug.LogError("풀에 오브젝트가 없음!");
	//		return null;
	//	}

	//	GameObject circleDot = poolCircle[pivot];

	//	// 풀링 초기화
	//	circleDot.SetActive(false);
	//	circleDot.transform.localScale = Vector3.one * circleBig;
	//	circleDot.SetActive(true);

	//		var circleMove = circleDot.GetComponent<CircleMove>();
	//       if (getDamage)
	//		circleMove.ChangeColor().Forget();
	//	circleMove.Initialize(this);
	//       activeCircles.Add(circleMove);

	//       pivot = (pivot + 1) % poolCircle.Length;
	//	return circleDot;
	//}
	public GameObject GetCircle()
	{
		if (poolCircle.Length == 0)
		{
			Debug.LogError("풀에 오브젝트가 없음!");
			return null;
		}

		GameObject circleDot = poolCircle[pivot];
		circleDot.transform.localScale = Vector3.one * circleBig;
		circleDot.transform.position = transform.position;
		circleDot.GetComponent<CircleMove>().Initialize(this);

		if (getDamage)
			circleDot.GetComponent<CircleMove>().ChangeColor().Forget();
		else
			circleDot.GetComponent<CircleMove>().SetColor();

		circleDot.SetActive(true); 

		activeCircles.Add(circleDot.GetComponent<CircleMove>());
		pivot = (pivot + 1) % poolCircle.Length;
		return circleDot;
	}
	public void ReturnCircle(GameObject circleDot)
	{
		if (circleDot == null || !circleDot.activeSelf) return;
		circleDot.GetComponent<CircleMove>().SetColor();

		circleDot.SetActive(false);
		circleDot.transform.localScale = Vector3.one * circleBig;
	}
	public async UniTask OnClickSuccess()
	{
		combo++;
		comboText.GetComponent<ComboText>().RanTextColor();
		cg.alpha = 1;
		//comboText.SetActive(true);
        a.PlayOneShot(clip);
        playerShoot.PlayerShoot_();
        playerHPSc.PlayerHPPlus(3);
		await UniTask.Delay(500);
		//comboText.SetActive(false);
		cg.alpha = 0;
	}
	private async UniTask circleGen()
	{
		while (true)
		{
			double delayMs = secondsPerBeat * 1000.0;
			await UniTask.Delay((int)delayMs);
            GetCircle().transform.position = transform.position;
        }
	}

	
}
