using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    public GameObject[] bulletPool;

    public GameObject spawnPoint1;
    public GameObject spawnPoint2;

    public GameObject[] bulletAArr;
    public GameObject[] bulletBArr;

    void Awake()
    {
        // ΩÃ±€≈Ê ∆–≈œ
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // ¡ﬂ∫π πÊ¡ˆ
        }
    }

    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject bulletA = Instantiate(bulletPool[i], spawnPoint1.transform);
            bulletA.SetActive(false);
            bulletAArr[i] = bulletA;
        }
        for(int i = 6; i < 9; i++)
        {
            GameObject bulletB = Instantiate(bulletPool[i], spawnPoint2.transform);
            bulletB.SetActive(false);
            bulletBArr[i - 6] = bulletB;
        }
    }
}
