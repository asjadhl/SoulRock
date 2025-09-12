
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;




[System.Serializable]
public class Formation
{
    public List<EnemiesType> m_enemiesType;
}

[System.Serializable]
public class Wave
{
    public List<Formation> m_formations;
    [Range(0, 10f)] public float arcHeight = 2f;

}

public class EnemySpawner : MonoBehaviour
{
    public Transform m_currentPlayer;
    public List<GameObject> SpawnPoint;
    [Range(0, 10)] public float m_SpawnRate;
    public List<Wave> m_waves;

    public float m_radiusTriggered;
    public Transform TargetScanner;

    public static bool IsInsideCircle(Vector3 pos, Vector3 center, float radius)
    {
        return (pos - center).sqrMagnitude <= radius * radius;
    }

    public void Awake()
    {

        gameObject.SetActive(true);

        m_currentPlayer = GameObject.FindWithTag("Player").transform;

        if (m_currentPlayer == null)
            gameObject.SetActive(false);


        //Get All SpawnPoint Auto
        SpawnPoint = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            SpawnPoint.Add(transform.GetChild(i).gameObject);


        }
        m_waves = new List<Wave>();
        CreateRandomEnemy();
    }
    public void Update()
    {
        for (int i = 0; i < m_waves.Count; i++)
        {
            if (IsInsideCircle(m_currentPlayer.position, TargetScanner.position, m_radiusTriggered))
            {
                StartCoroutine(SpawnNow(m_waves[i]));
                m_waves.Remove(m_waves[i]);
            }

        }
    }

    IEnumerator SpawnNow(Wave _wave)
    {

      

        for (int i = 0; i < _wave.m_formations.Count; i++)
        {


            for (int index = 0; index < _wave.m_formations[i].m_enemiesType.Count; index++)
            {
                 
                if (_wave.m_formations[i].m_enemiesType[index] != null)
                {

                    if (_wave.m_formations[i].m_enemiesType[index].enemyobject != null)
                    {
                        //if (_wave.m_formations[i].m_enemiesType[index].type == EntityType.Walker)
                        //{
                        //    Instantiate(_wave.m_formations[i].m_enemiesType[index].enemyobject, SpawnPoint[Random.Range(4, 7)].transform.position, Quaternion.identity);
                        //}
                        //else if (_wave.m_formations[i].m_enemiesType[index].type == EntityType.Fly)
                        //{
                        //    Instantiate(_wave.m_formations[i].m_enemiesType[index].enemyobject, SpawnPoint[Random.Range(0, 3)].transform.position, Quaternion.identity);
                        //}
                        Instantiate(_wave.m_formations[i].m_enemiesType[index].enemyobject, SpawnPoint[Random.Range(0, SpawnPoint.Count-1)].transform).transform.SetParent(null);
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(m_SpawnRate);


        }
        CreateRandomEnemy();
    }


    void CreateRandomEnemy()
    {


        Wave wave = new Wave();
        wave.m_formations = new List<Formation>();
        for (int i = 0; i < 3; i++) //three formation
        {
            Formation newformation = new Formation();
            newformation.m_enemiesType = new List<EnemiesType>();
            for (int j = 0; j < 10; j++)
            {
                if (Random.value < 0.5f)
                {
                    newformation.m_enemiesType.Add(null);

                }
                else
                {

                   
                    newformation.m_enemiesType.Add(GameManager.instance.GetRandomEnemies);
                }

            }
            wave.m_formations.Add(newformation);

        }

        m_waves.Add(wave);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemySpawner))]
public class ShowScanner : Editor
{
    void OnSceneGUI()
    {
        EnemySpawner t = (EnemySpawner)target;
        

   

            if (t.TargetScanner != null)
            {
                Vector3 start = t.transform.position;
                Vector3 end = t.TargetScanner.position;







                // Midpoint for curved arrow
                Vector3 mid = (start + end) * 0.5f;
                 mid.y += 2.5f;

                // Determine color based on index
                Color arrowColor = Color.HSVToRGB(0.6f, 1f, 1f);




                Handles.DrawBezier(start, end, mid, mid, arrowColor, null, 1f);

                // Arrowhead at the end
                Handles.ArrowHandleCap(
                    0,
                    end,
                    Quaternion.LookRotation(end - mid),
                    1f,
                    EventType.Repaint
                );

                // Optional: label with index
                Handles.Label(end + Vector3.up * 0.5f, $"Scanning_Area");

                Handles.color = Color.cyan;

                Handles.DrawWireDisc(end, Vector3.up, t.m_radiusTriggered);


            }
        



    }
}
#endif


