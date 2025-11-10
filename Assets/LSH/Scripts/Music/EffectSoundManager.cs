using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectSoundManager : MonoBehaviour
{
    [SerializeField] GameObject audioPrefab;
    [SerializeField] private int poolSize = 5;

    private readonly Queue<AudioSource> pool = new Queue<AudioSource>();

    void Awake()
    {
        // 풀 초기화
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(audioPrefab, transform);
            var source = obj.GetComponent<AudioSource>();
            if (source == null) source = obj.AddComponent<AudioSource>();
            obj.SetActive(false);
            pool.Enqueue(source);
        }
    }

    public AudioSource GetSource()
    {
        if (pool.Count > 0)
        {
            var source = pool.Dequeue();
            source.gameObject.SetActive(true);
            return source;
        }

        var extra = Instantiate(audioPrefab, transform);
        var newSource = extra.GetComponent<AudioSource>();
        if (newSource == null) newSource = extra.AddComponent<AudioSource>();
        return newSource;
    }

    public void ReturnSource(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        pool.Enqueue(source);
    }
}
