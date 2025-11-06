using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class LastBossMove : MonoBehaviour
{
    public float moveSpeed = 4f;
    public bool canRun = false;
    BossMove temp;
    private void Start()
    {
        temp = GameObject.FindAnyObjectByType<BossMove>();
    }
    private void FixedUpdate()
    {
        if (temp.canRun)
            UpdateBossRun();
        if (BossState.isBoss3Dead) gameObject.SetActive(false);
    }

    public async UniTask HitGhostBoss()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(0, 0, 6f);
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            await UniTask.Yield();
        }

        transform.position = targetPos; 
    }
    private void UpdateBossRun()
    {
        transform.position += moveSpeed * Time.fixedDeltaTime * -transform.forward;
    }

	private void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag("Player"))
		{
			GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPBigMinus().Forget();
			gameObject.SetActive(false);
		}
	}

	
}


