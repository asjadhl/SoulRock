using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
	[SerializeField] float minDis = 0.01f;
	[SerializeField] float exDis = 70f;
	[SerializeField] float maxDis = 120f;

	[Header("원 도트 프리팹")]
	[SerializeField] GameObject CirclePrefab;

	[Header("풀 사이즈")]
	[SerializeField] int poolSize = 15;

	[Header("Combo")]
	public int combo = 0;
	[SerializeField] GameObject comboText;
	[SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI comboNumText;

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
    Color randomColor;
    public bool isScale = false;
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
		playerShoot = FindAnyObjectByType<PlayerShoot>();
        playerHPSc = FindAnyObjectByType<PlayerHP>();
        circleGen().Forget();
	}

	private void Update()
	{
		CheckCol();
    }
    
	void CheckCol()
	{
		if (Input.GetMouseButtonDown(0)&&!playerHPSc.isPlayerDead)
		{
			for (int i = activeCircles.Count - 1; i >= 0; i--)
			{
				var circle = activeCircles[i];
				float distance = Vector2.Distance(targetImage.position, circle.hitRect.position);

				if (minDis <= distance && exDis >= distance)
				{
					OnClickSuccessEx().Forget();
                    comboNumText.text = combo.ToString();

                    ReturnCircle(circle.gameObject);
					activeCircles.RemoveAt(i);
				}
				else if(exDis < distance && maxDis >= distance)
				{
					OnClickSuccess().Forget();
                    comboNumText.text = combo.ToString();
                    ReturnCircle(circle.gameObject);
					activeCircles.RemoveAt(i);
                }
				//else { text.text = "Bad"; }
			}
		}
	}
    private void RanTextColor()
    {
        randomColor = new Color(Random.value, Random.value, Random.value);
        comboNumText.color = randomColor;
    }

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
		RanTextColor();
        text.text = "Good";
        isScale = true;
		cg.alpha = 1;
        a.PlayOneShot(clip);
        playerShoot.PlayerShoot_();
        playerHPSc.PlayerHPPlus(2);
		await UniTask.Delay(150);
		isScale = false;
		await UniTask.Delay(350);
		cg.alpha = 0;
	}

    public async UniTask OnClickSuccessEx()
    {
        combo++;
        RanTextColor();
		text.text = "Perfect";
        isScale = true;
        cg.alpha = 1;
        a.PlayOneShot(clip);
        playerShoot.PlayerShoot_();
        playerHPSc.PlayerHPPlus(4);
        await UniTask.Delay(150);
        isScale = false;
        await UniTask.Delay(350);
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
