using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [Header("미니 스컬 폭발 파티클")]
    [SerializeField] private ParticleSystem skullParticle;
    [Header("구체 폭발 파티클")]
    [SerializeField] private ParticleSystem hitParticle;
    [Header("가짜 유령 폭발 파티클")]
    [SerializeField] private ParticleSystem fakeGhostParticle;
	[Header("진짜 유령 폭발 파티클")]
	[SerializeField] private ParticleSystem RealGhostParticle;
	[Header("풀 사이즈")]
    [SerializeField] private int poolSize = 10;

    private Queue<ParticleSystem> skullPool = new Queue<ParticleSystem>();
    private Queue<ParticleSystem> hitPool = new Queue<ParticleSystem>();
    private Queue<ParticleSystem> fakeGhostPool = new Queue<ParticleSystem>();
	private Queue<ParticleSystem> RealGhostPool = new Queue<ParticleSystem>();

	void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem effect = Instantiate(skullParticle, transform);
            effect.gameObject.SetActive(false);
            skullPool.Enqueue(effect);
        }

        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem effect = Instantiate(hitParticle, transform);
            effect.gameObject.SetActive(false);
            hitPool.Enqueue(effect);
        }
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem effect = Instantiate(fakeGhostParticle, transform);
            effect.gameObject.SetActive(false);
            fakeGhostPool.Enqueue(effect);
        }
		for(int i = 0; i < 4; i++)
        {
			ParticleSystem effect = Instantiate(RealGhostParticle, transform);
			effect.gameObject.SetActive(false);
			RealGhostPool.Enqueue(effect);
		}
	}

    public void PlaySkullEffect(Vector3 pos)
    {
        if (skullPool.Count == 0)
        {
            AddEffectToPool(skullParticle, skullPool, 1);
        }

        ParticleSystem effect = skullPool.Dequeue();
        effect.transform.position = pos;
        effect.gameObject.SetActive(true);
        effect.Play();

        ReturnToPoolAfter(effect, skullPool).Forget();
    }

    public void PlayHitEffect(Vector3 pos)
    {
        if (hitPool.Count == 0)
        {
            AddEffectToPool(hitParticle, hitPool, 1);
        }

        ParticleSystem effect = hitPool.Dequeue();
        effect.transform.position = pos;
        effect.gameObject.SetActive(true);
        effect.Play();

        ReturnToPoolAfter(effect, hitPool).Forget();
    }

    public void PlayGhostEffect(Vector3 pos)
    {
        if (fakeGhostPool.Count == 0)
        {
            AddEffectToPool(fakeGhostParticle, fakeGhostPool, 1);
        }

        ParticleSystem effect = fakeGhostPool.Dequeue();
        effect.transform.position = pos;
        effect.gameObject.SetActive(true);
        effect.Play();

        ReturnToPoolAfter(effect, fakeGhostPool).Forget();
    }
	public void PlayRealGhostEffect(Vector3 pos)
	{
		if (RealGhostPool.Count == 0)
		{
			AddEffectToPool(RealGhostParticle, RealGhostPool, 1);
		}

		ParticleSystem effect = RealGhostPool.Dequeue();
		effect.transform.position = pos;
		effect.gameObject.SetActive(true);
		effect.Play();

		ReturnToPoolAfter(effect, RealGhostPool).Forget();
	}
	private async UniTask ReturnToPoolAfter(ParticleSystem effect, Queue<ParticleSystem> pool)
    {
        await UniTask.Delay((int)(effect.main.duration * 1000)); 
        effect.Stop();
        effect.gameObject.SetActive(false);
        pool.Enqueue(effect);
    }

    private void AddEffectToPool(ParticleSystem prefab, Queue<ParticleSystem> pool, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var effect = Instantiate(prefab, transform);
            effect.gameObject.SetActive(false);
            pool.Enqueue(effect);
        }
    }
}
