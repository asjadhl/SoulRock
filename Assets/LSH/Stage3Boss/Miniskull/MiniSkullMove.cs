using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 미니 스컬 이동 및 추격 로직
/// - 플레이어와의 거리 기반으로 이동, 추격, 폭발 처리
/// - 근접 시 약간의 딜레이 후 폭발 (연출용)
/// </summary>
public class MiniSkullMove : MonoBehaviour
{
	[Header("타겟 (플레이어)")]
	public Transform skullScanner;

	[Header("이동 속도 설정")]
	public float forwardSpeed = 2f;    
	public float chaseSpeed = 10f;    

	[Header("거리 설정")]
	public float chaseStartDistance = 10f;  
	public float disableDistance = 1f;      // 폭발 거리

	[Header("사망 이펙트")]
	[SerializeField] private ParticleSystem explosionEffect;

	[Header("폭발 타이밍 설정")]
	public float attachDelay = 1.0f;        

	private Animator animator;
	private bool isChasing = false;
	private bool hasExploded = false;
	private readonly Vector3 effectOffset = new Vector3(0, 1f, 0); 

	void Start()
	{
		if (skullScanner == null)
			skullScanner = GameObject.FindWithTag("Player").transform;

		animator = GetComponent<Animator>();
	}

	void Update()
	{
		SkullMove().Forget(); 
	}

	private async UniTask SkullMove()
	{
		if (skullScanner == null || hasExploded) return;

		float distance = Vector3.Distance(transform.position, skullScanner.position);

		if (!isChasing && distance > chaseStartDistance)
		{
			transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
			return;
		}

		if (distance <= chaseStartDistance && distance > disableDistance)
		{
			animator.SetBool("Run", true);
			isChasing = true;

			Vector3 targetPos = new Vector3(
				skullScanner.position.x,
				transform.position.y,
				skullScanner.position.z
			);

			Vector3 direction = (targetPos - transform.position).normalized;

			transform.position += direction * chaseSpeed * Time.deltaTime;

			if (direction != Vector3.zero)
				transform.rotation = Quaternion.LookRotation(direction);

			return;
		}

		if (distance <= disableDistance && !hasExploded)
		{
			hasExploded = true;
			animator.SetBool("Run", false);
			transform.LookAt(skullScanner);

			transform.SetParent(skullScanner.transform, true);

			await UniTask.Delay((int)(attachDelay * 1000));

			Vector3 effectPos = transform.position + effectOffset;
			var explosion = Instantiate(explosionEffect, effectPos, Quaternion.identity);
			explosion.Play();

			gameObject.SetActive(false);
		}
	}
}
