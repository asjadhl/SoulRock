using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }

    }


    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"Pool with tag [{tag}] doesn't exist in PoolingManager!");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        if (pool.Count == 0)
        {
            Debug.LogError($"Pool with tag [{tag}] is empty! (size not enough)");
            return null;
        }

        GameObject obj = pool.Dequeue();
        if (obj == null)
        {
            Debug.LogError($"Pooled object with tag [{tag}] is null (Prefab missing?)");
            return null;
        }

        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);
        pool.Enqueue(obj);

        return obj;
    }

}
