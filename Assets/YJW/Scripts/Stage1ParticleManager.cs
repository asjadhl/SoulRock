using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Stage1ParticleManager : MonoBehaviour
{
    public static Stage1ParticleManager Instance { get; private set; }

    [Header("광대 파티클")]
    [SerializeField] private ParticleSystem clownParticle;
    [Header("박스 파티클")]
    [SerializeField] private ParticleSystem boxParticle;
    [Header("풀 사이즈")]
    [SerializeField] private int poolSize = 10;

    private Queue<ParticleSystem> clownPool = new Queue<ParticleSystem>();
    private Queue<ParticleSystem> boxPool = new Queue<ParticleSystem>();


    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem effect = Instantiate(clownParticle, transform);
            effect.gameObject.SetActive(false);
            clownPool.Enqueue(effect);
        }
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem effect = Instantiate(boxParticle, transform);
            effect.gameObject.SetActive(false);
            boxPool.Enqueue(effect);
        }
    }

    public void PlayClownEffect(Vector3 pos)
    {
        if (clownPool.Count == 0)
        {
            AddEffectToPool(clownParticle, clownPool, 1);
        }

        ParticleSystem effect = clownPool.Dequeue();
        effect.transform.position = pos;
        effect.gameObject.SetActive(true);
        effect.Play();

        ReturnToPoolAfter(effect, clownPool).Forget();
    }

    public void PlayBoxEffect(Vector3 pos)
    {
        if (boxPool.Count == 0)
        {
            AddEffectToPool(boxParticle, boxPool, 1);
        }

        ParticleSystem effect = boxPool.Dequeue();
        effect.transform.position = pos;
        effect.gameObject.SetActive(true);
        effect.Play();

        ReturnToPoolAfter(effect, boxPool).Forget();
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
