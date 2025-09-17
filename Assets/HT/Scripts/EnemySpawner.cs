
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Unity.Collections;
using Unity.Loading;
using UnityEditor;
using UnityEngine;
using static UnityEngine.InputManagerEntry;




[System.Serializable]
public class Wave
{
    public List<string> formations;
}
 
public class EnemyStore
{
    List<GameObject> ListOfGameObject;

    public void AddGhost(GameObject go)
    {
        ListOfGameObject.Add(go);
    }
    public EnemyStore()
    {
        ListOfGameObject = new List<GameObject>();
    }
    public GameObject GetEnemies()
    {
        var a = ListOfGameObject.Find(p => p.activeSelf == false);
        return a;
    }
}

public class SpawnPointManager
{
    Dictionary<int, List<GameObject>> ListofSpawnPoint;

    public SpawnPointManager()
    {
        ListofSpawnPoint = new();
       
    }

    public GameObject GetSpawnPoint(int ID,bool IsRandom)
    {
       var a =  ListofSpawnPoint[ID].FindAll(p=>p.transform.childCount <= 0);

        if (IsRandom)
            return a[Random.Range(0, a.Count - 1)];
        else
            return a[0];
    }

    public void AddSpawnPoint(int ID,GameObject ob)
    {
        if(ListofSpawnPoint.ContainsKey(ID))
        {
            ListofSpawnPoint[ID].Add(ob);
        }
        else
        {
            ListofSpawnPoint.Add(ID,new List<GameObject>());
            ListofSpawnPoint[ID].Add(ob);
        }
    }
}

public class EnemySpawner : MonoBehaviour
{
    public Transform m_currentPlayer;
    [Range(0, 10)] public float m_SpawnRate =5f;
    [Range(0, 10)] public int m_maxFormationCount;
    [Range(0, 10)] public int m_maxEnemiesCount;
    public float m_current_childCount;
    public int m_ReuseableEnemyObject;
    //Holding Obj
    public Dictionary<int, List<GameObject>> ObjOfflineHolder;
    public List<Wave> waves;
    public EnemyStore m_enemyStore;
    public SpawnPointManager m_spawnPointManager;
    public string walk = "";
    public string fly = "";
    public int indexStore = 0;

    public float m_radiusTriggered;
    public Transform TargetScanner;
    private CancellationTokenSource cts;

    [Space(5)]
    [Header("BetweenPlayer")]
    public Vector3 offset;
    public static bool IsInsideCircle(Vector3 pos, Vector3 center, float radius)
    {
        return (pos - center).sqrMagnitude <= radius * radius;
    }

    public void Start()
    {


        StartCreate();
        //gameObject.SetActive(true);

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
        ////Get All SpawnPoint Auto
        m_spawnPointManager = new SpawnPointManager();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name[0] == '0')
            {
                m_spawnPointManager.AddSpawnPoint(0, transform.GetChild(i).gameObject);
            }
            else if (transform.GetChild(i).name[0] == '1')
            {
                m_spawnPointManager.AddSpawnPoint(1, transform.GetChild(i).gameObject);
            }
        }




        for (int i = 0; i < transform.childCount; i++)
            {

            if ('0' == transform.GetChild(i).gameObject.name[0])
            {
                walk += i;

            }
            else if ('1' == transform.GetChild(i).gameObject.name[0])
            {
                fly += i;
            }
            else
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            }


            m_current_childCount = transform.childCount;
       
        cts = new CancellationTokenSource();
        waves.Add(CreateWave());
    }

    void OffsetPlayerPos()
    {  
        if(m_currentPlayer != null)
        transform.position = m_currentPlayer.transform.position + offset;
    }
    public void Update()
    {

        OffsetPlayerPos();


        if (m_currentPlayer == null)
        {   
            //cts.Cancel();
            Debug.Log("Player With Tag is disappear");
        }

            for (int i = 0; i < waves.Count; i++)
        {
            if (IsInsideCircle(m_currentPlayer.position, TargetScanner.position, m_radiusTriggered))
            {
                 
                UniSpawnNow(waves[i],cts.Token).Forget();
                waves.Remove(waves[i]);
            }

        }


       
    }
   
   



  

    public async UniTask UniSpawnNow(Wave _wave,CancellationToken token, System.Action callback = null)
    {
        
         
         
        try
        {

            Debug.Log("SpawnNow");

          
            List<GameObject> setallfalse = new List<GameObject>();
            List<string> listformation = _wave.formations;
            int x = 0, y = 0, z = 0;
            for(;x<listformation.Count; x++)
            {
                
                for (; y < listformation[x].Length; y++)
                {
                    

                    if (listformation[x][y] == '0')
                    {  //Find Avaible SpawnPoint With No people on room
                     GameObject ob =   m_enemyStore.GetEnemies();
                        ob.SetActive(true);
                        setallfalse.Add(ob);
                      GameObject spawnpoint = m_spawnPointManager.GetSpawnPoint(0, true);
                        ob.transform.position = spawnpoint.transform.position;
                        ob.transform.SetParent(spawnpoint.transform);

                    }
                    else if (listformation[x][y] == '1')
                    {
                        // Find Avaible SpawnPoint With No people on room
                        GameObject ob = m_enemyStore.GetEnemies();
                        ob.SetActive(true);
                        setallfalse.Add(ob);
                        GameObject spawnpoint = m_spawnPointManager.GetSpawnPoint(1, true);
                        ob.transform.position = spawnpoint.transform.position;
                        ob.transform.SetParent(spawnpoint.transform);
                    }
                                       
                }

                //set all setparent null
                foreach(var child in setallfalse)
                {
                    child.SetActive(false);
                }
                await UniTask.WaitForSeconds(m_SpawnRate, cancellationToken: token);
            }

            waves.Add(CreateWave());
        }
        catch(System.OperationCanceledException)
        {

        }
        finally
        {

        }
    }

    
    
    Wave CreateWave()
    {   
        Wave wave = new(); 
        wave.formations = new();

        string formation = "";

        char[] State = { '0', '1' };

        int desiredFormationCount = m_maxFormationCount;
        int desiredEnemyCount = Random.Range(0, m_maxEnemiesCount);

        int x=0,y=0;

        for(;x<desiredFormationCount;x++)
        {
            for(;y<desiredEnemyCount;y++)
            {
                formation += State[Random.Range(0, State.Length - 1)];
            }
            wave.formations.Add(formation);
            formation = "";
        }    
       
        return wave;
    }
    void StartCreate()
    {
        int tempCount = m_ReuseableEnemyObject;
        string front = "";
        m_enemyStore = new();
        // j= 0 = EntityType.Walker
        // j= 1 = EntityType.Fly
        for (int j = 0; j < GameManager.instance.GhostEnemies.Count; j++)
        {    
            for (int i = 0; i < tempCount; i++)
            {
                m_enemyStore.AddGhost(Instantiate(GameManager.instance.GhostEnemies[j].enemyobject,transform.position,Quaternion.identity));
            }
        }
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


