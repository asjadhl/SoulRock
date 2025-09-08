using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;




[System.Serializable]
public class Formation
{
    public List<GameObject> m_enemies;
}

[System.Serializable]
public class Wave
{
    public List<Formation> m_formations;
    public Transform m_ScanningArea;
}

public class EnemySpawner : MonoBehaviour
{
    public Transform m_currentPlayer;
    public List<GameObject> SpawnPoint;
    [Range(0, 10)] public float m_SpawnRate;
    public List<Wave> m_waves;

    int GetRandomIndex(int min, int max)
    {
        return Random.Range(min, max);
    }
    public static bool IsInsideCircle(Vector3 pos, Vector3 center, float radius)
    {
        return (pos - center).sqrMagnitude <= radius * radius;
    }

    public void Awake()
    {
        m_currentPlayer = GameObject.FindWithTag("Player").transform;

        if (m_currentPlayer == null)
            gameObject.SetActive(false);
    }
    public void Update()
    {
        for (int i = 0; i < m_waves.Count; i++)
        {
            if (IsInsideCircle(m_currentPlayer.position, m_waves[i].m_ScanningArea.position, 4f))
            {
                StartCoroutine(SpawnNow(m_waves[i]));
                m_waves.Remove(m_waves[i]);
            }
        }
    }

    IEnumerator SpawnNow(Wave _wave)
    {

        float m_timer = 0f;

        for (int i = 0; i < _wave.m_formations.Count; i++)
        {
            while (true)
            {
                m_timer += Time.deltaTime;
                if (m_timer >= m_SpawnRate)
                {

                    for (int index = 0; index < _wave.m_formations[i].m_enemies.Count; index++)
                    {
                        if (_wave.m_formations[i].m_enemies[index] == null)
                            continue;
                        else
                        {
                            Instantiate(_wave.m_formations[i].m_enemies[index], SpawnPoint[Mathf.Clamp(index, 0, 7)].transform.position, Quaternion.identity);
                        }
                    }
                    break;

                }
            }
            m_timer = 0;
        }

        yield return null;
    }



}
