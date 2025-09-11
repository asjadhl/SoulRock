
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor;
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
    [Range(0,10f)]public float arcHeight = 2f;
    
}

public class EnemySpawner : MonoBehaviour
{
    public Transform m_currentPlayer;
    public List<GameObject> SpawnPoint;
    [Range(0, 10)] public float m_SpawnRate;
    public List<Wave> m_waves;

    public float m_radiusTriggered;
    Transform prevScanArea;

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
    }
    public void Update()
    {
        for (int i = 0; i < m_waves.Count; i++)
        {
            if (IsInsideCircle(m_currentPlayer.position, m_waves[i].m_ScanningArea.position, m_radiusTriggered))
            {
               _ = SpawnNow(m_waves[i]);
                m_waves.Remove(m_waves[i]);
            }
         
        }
    }

    [Obsolete]
    IEnumerator SpawnNoww(Wave _wave)
    {

        prevScanArea = _wave.m_ScanningArea;

        for (int i = 0; i < _wave.m_formations.Count; i++)
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

            yield return new WaitForSeconds(m_SpawnRate);

           
        }
        CreateRandomEnemy();
    }


    private async UniTask SpawnNow(Wave _wave)
    {
        prevScanArea = _wave.m_ScanningArea;

        for (int i = 0; i < _wave.m_formations.Count; i++)
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

            await UniTask.Delay((int)m_SpawnRate);


        }
        CreateRandomEnemy();
    }


    void CreateRandomEnemy()
    {
         

       Wave wave = new Wave();
        wave.m_ScanningArea = prevScanArea;
       wave.m_formations = new List<Formation>();
        for(int i=0;i<3;i++) //three formation
        {
            Formation newformation = new Formation();
            newformation.m_enemies = new List<GameObject>();
            for(int j=0;j<10;j++)
            {
                if (Random.value < 0.5f)
                {
                    newformation.m_enemies.Add(null);

                }
                else
                {
                     

                    //newformation.m_enemies.Add(GameManager.instance.GetRandomEnemies);
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
        if (t.m_waves == null) return;

        for (int i = 0; i < t.m_waves.Count; i++)
        {
            var wave = t.m_waves[i];

            if (wave.m_ScanningArea != null)
            {
                Vector3 start = t.transform.position;
                Vector3 end = wave.m_ScanningArea.position;
 

            

          

 
                // Midpoint for curved arrow
                Vector3 mid = (start + end) * 0.5f;
                mid.y += wave.arcHeight;

                // Determine color based on index
                Color arrowColor = Color.HSVToRGB(i / (float)t.m_waves.Count, 1f, 1f);

               

               
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
                Handles.Label(end + Vector3.up * 0.5f, $"Scanning_Area {i}");

                Handles.color = Color.cyan;

                Handles.DrawWireDisc(end, Vector3.up, t.m_radiusTriggered);


            }
        }



    }
}
#endif