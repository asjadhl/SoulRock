
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

[SerializeField]
[System.Serializable]
public class SpawnPointType
{
    public EntityType type;
    public GameObject SpawnPoint;

    public SpawnPointType(EntityType _type,GameObject _gameobject)
    {

    type = _type; 
    SpawnPoint = _gameobject;}
}

public class EnemySpawner : MonoBehaviour
{
    public Transform m_currentPlayer;
    public Dictionary<int,SpawnPointType> DictSpawnPoint;
    [Range(0, 10)] public float m_SpawnRate =5f;
    [Range(0f, 1f)] public float m_CreateChanceRate;
    public List<Wave> m_waves;

    public float m_radiusTriggered;
    public Transform TargetScanner;
    private CancellationTokenSource cts;

    public static bool IsInsideCircle(Vector3 pos, Vector3 center, float radius)
    {
        return (pos - center).sqrMagnitude <= radius * radius;
    }

    public void Start()
    {
        
      
        gameObject.SetActive(true);

        m_currentPlayer = GameObject.FindWithTag("Player").transform;

        if (m_currentPlayer == null)
        {
            gameObject.SetActive(false);
            Debug.LogError("CANT FIND THE PLAYER WITH TAG");
        }
        if(TargetScanner == null)
        {
           GameObject find = GameObject.Find("Scanner");
            if(find == null)
            {
                Debug.LogError("ADD Scanner");
                gameObject.SetActive(false);
            }
        }
        //Get All SpawnPoint Auto
        DictSpawnPoint = new();
        for (int i = 0; i < transform.childCount; i++)
        {

            if ('0' == transform.GetChild(i).gameObject.name[0])
            {
                DictSpawnPoint.Add(i, new SpawnPointType(EntityType.Walker, transform.GetChild(i).gameObject));
              
            }
            else if ('1' == transform.GetChild(i).gameObject.name[0])
            {
                DictSpawnPoint.Add(i, new SpawnPointType(EntityType.Walker, transform.GetChild(i).gameObject));
            }
            else
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        m_waves = new List<Wave>();
        cts = new CancellationTokenSource();
        CreateRandomEnemy();
    }
    public void Update()
    {
        for (int i = 0; i < m_waves.Count; i++)
        {
            if (IsInsideCircle(m_currentPlayer.position, TargetScanner.position, m_radiusTriggered))
            {
                 
                UniSpawnNow(m_waves[i],cts.Token).Forget();
                m_waves.Remove(m_waves[i]);
            }

        }
    }
    public async UniTaskVoid UniSpawnNow(Wave _wave, CancellationToken token, System.Action callback = null)
    {   
        bool canceled = false;
        List<GameObject> newEnemy = new List<GameObject>();
        try 
        {
          
            for (int formationIndex = 0; formationIndex < _wave.m_formations.Count; formationIndex++)
            { 

                List<GameObject> Walker = new List<GameObject>();
                List<GameObject> Fly = new List<GameObject>();
                
                for (int l = 0; l < DictSpawnPoint.Count; l++)
                {
                    if (DictSpawnPoint[l].type == EntityType.Walker && DictSpawnPoint[l].SpawnPoint.transform.childCount <= 0)
                        Walker.Add(DictSpawnPoint[l].SpawnPoint);
                    else if (DictSpawnPoint[l].type == EntityType.Fly && DictSpawnPoint[l].SpawnPoint.transform.childCount <= 0)
                        Fly.Add(DictSpawnPoint[l].SpawnPoint);
                }

                for (int enemiesIndex = 0; enemiesIndex < _wave.m_formations[formationIndex].m_enemiesType.Count; enemiesIndex++)
                {
                    if (_wave.m_formations[formationIndex].m_enemiesType[enemiesIndex] == null)
                        continue;

                   

                    if (_wave.m_formations[formationIndex].m_enemiesType[enemiesIndex].enemyobject != null)
                    {
                        if (_wave.m_formations[formationIndex].m_enemiesType[enemiesIndex].type == EntityType.Walker && Walker.Count >= 1)
                        {



                            newEnemy.Add(Instantiate(_wave.m_formations[formationIndex].m_enemiesType[enemiesIndex].enemyobject, Walker[Walker.Count-1].transform));
                             Walker.Remove(Walker[Walker.Count - 1]);
                            // Debug.LogWarning("ENEMY NOT SPAWNING DUE TO NOT HAVING ENOUGH ROOM TO SPAWN FROM WALKER SPAWN");
                        }
                        else if (_wave.m_formations[formationIndex].m_enemiesType[enemiesIndex].type == EntityType.Fly && Fly.Count >= 1)
                        {

                            newEnemy.Add(Instantiate(_wave.m_formations[formationIndex].m_enemiesType[enemiesIndex].enemyobject, Fly[Fly.Count - 1].transform));
                            Fly.Remove(Fly[Fly.Count - 1]);
                            // Debug.LogWarning("ENEMY NOT SPAWNING DUE TO NOT HAVING ENOUGH ROOM TO SPAWN FROM FLY SPAWN");
                        }


                    }
                }

                 foreach(GameObject child in newEnemy)
                    child.transform.SetParent(null);

                     newEnemy.Clear();

                await UniTask.WaitForSeconds(m_SpawnRate, cancellationToken: token);
            }
        }
        catch(System.OperationCanceledException)
        {
            canceled = true;
        
        }
        finally
        {
            if (!canceled)
            {
                CreateRandomEnemy();
                callback?.Invoke();
            }  

            if(canceled)
            {
                foreach (GameObject child in newEnemy)
                    child.transform.SetParent(null);
            }
        }
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
                if (Random.value < m_CreateChanceRate)
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


