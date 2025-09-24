
//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using UnityEditor;
//using UnityEngine;





//[System.Serializable]
//public class Wave
//{
//    public List<string> formations;
//    public Wave()
//    {
//        formations = new List<string>();
//    }
//}

//public class EnemyStore
//{
//    List<GameObject> ListOfGameObject;
//    public static IEnumerable<TResult> Selects<TSource,TResult>(List<TSource> source,Func<TSource,TSource,TResult> prec)
//    {
//        foreach (var item in source)
//            yield return prec(item,item);
//    }


//    //Prototype
//    Dictionary<string, List<GameObject>> ListOfGameObj;

//    public void AddGhost(GameObject go)
//    {
//        go.SetActive(false);
//        ListOfGameObject.Add(go);
//    }

//    public void AddGhostPrototype(EntityType type, EntityI entityI, GameObject go)
//    {
       
//        go.SetActive(false);
//        if (ListOfGameObj.ContainsKey(type.ToString()+entityI.ToString()))
//        {    
//            ListOfGameObj[type.ToString() + entityI.ToString()].Add(go);
//        }
//        else
//        {
//            ListOfGameObj.Add(type.ToString() + entityI.ToString(), new List<GameObject>());
//            ListOfGameObj[type.ToString() + entityI.ToString()].Add(go);
//        }
//        go.name += $"[{ListOfGameObj.Count-1}][{entityI}]";
//    }
//    public EnemyStore()
//    {
//        ListOfGameObject = new List<GameObject>();

//        //Prototype
//        ListOfGameObj = new Dictionary<string, List<GameObject>>();
//    }
//    public GameObject GetEnemies()
//    {
//        var a = ListOfGameObject.Find(p => p.activeSelf == false);

//        return a;
//    }

//    public GameObject GetEnemiesPrototype(EntityType type, EntityI entityI)
//    {
//        if (ListOfGameObj.ContainsKey(type.ToString() + entityI.ToString()))
//        {
//            var a = ListOfGameObj[type.ToString() + entityI.ToString()].Find(p => p.activeSelf == false);

//            if (a != null)
//            {  
//               a.GetComponent<EnemyGhost>().Init(type,entityI);
//                return a;
//            }
//            else
//            {
//                AddGhostPrototype(type, entityI, GameManager.instance.CreateGameObject(type, entityI));
//                a = ListOfGameObj[type.ToString() + entityI.ToString()].Find(p => p.activeSelf == false);
                 
//                return a;
//            }
//        }
//        else
//        {
//            var newobj = GameManager.instance.CreateGameObject(type, entityI);
//            if (newobj != null)
//            {
//                AddGhostPrototype(type,entityI, newobj);
//                return ListOfGameObj[type.ToString() + entityI.ToString()].Find(p => p.activeSelf == false);
//            }
//            else return null;
             
//        }
//    }
//}
//public class SpawnPointManager
//{
//    Dictionary<int, List<GameObject>> ListofSpawnPoint;

//    public SpawnPointManager()
//    {
//        ListofSpawnPoint = new();
       
//    }

//    public GameObject GetSpawnPoint(EntityType type,bool IsRandom)
//    {
//       var a =  ListofSpawnPoint[(int)type].FindAll(p=>p.transform.childCount <= 0);

//        if (IsRandom)
//            return a[UnityEngine.Random.Range(0, a.Count - 1)];
//        else
//            return a[0];
//    }

//    public void AddSpawnPoint(EntityType type,GameObject ob)
//    {
//        if(ListofSpawnPoint.ContainsKey((int)type))
//        {
//            ListofSpawnPoint[(int)type].Add(ob);
//        }
//        else
//        {
//            ListofSpawnPoint.Add((int)type, new List<GameObject>());
//            ListofSpawnPoint[(int)type].Add(ob);
//        }
//    }
//}

//public class EnemySpawner : MonoBehaviour
//{
//    public Transform m_currentPlayer;
//    [Range(0, 10)] public float m_SpawnRate =5f;
//    [Range(0, 10)] public int m_maxFormationCount;
//    [Range(0, 10)] public int m_maxEnemiesCount;
//    public float m_current_childCount;
//    public int m_ReuseableEnemyObject; //<--- Obselete
//    //Holding Obj
//    public List<Wave> waves;
//    public EnemyStore m_enemyStore;
//    public SpawnPointManager m_spawnPointManager;
//    public int indexStore = 0;

//    public float m_radiusTriggered;
//    public Transform TargetScanner;
//    private CancellationTokenSource cts;

//    [Space(5)]
//    [Header("BetweenPlayer")]
//    public Vector3 offset;
//    public static bool IsInsideCircle(Vector3 pos, Vector3 center, float radius)
//    {
//        return (pos - center).sqrMagnitude <= radius * radius;
//    }

//    public void Start()
//    {

         
     
//        //gameObject.SetActive(true);

//        m_currentPlayer = GameObject.FindWithTag("Player").transform;

//        if (m_currentPlayer == null)
//        {
//            gameObject.SetActive(false);
//            Debug.LogError("CANT FIND THE PLAYER WITH TAG");
//        }
//        if(TargetScanner == null)
//        {
//           GameObject find = GameObject.Find("Scanner");
//            if(find == null)
//            {
//                Debug.LogError("ADD Scanner");
//                gameObject.SetActive(false);
//            }
//        }
//        ////Get All SpawnPoint Auto
//        m_spawnPointManager = new SpawnPointManager();
//        for (int i = 0; i < transform.childCount; i++)
//        {
//            if (transform.GetChild(i).name[0] == '0')
//            {
//                m_spawnPointManager.AddSpawnPoint(EntityType.Walker, transform.GetChild(i).gameObject);
//            }
//            else if (transform.GetChild(i).name[0] == '1')
//            {
//                m_spawnPointManager.AddSpawnPoint(EntityType.Fly, transform.GetChild(i).gameObject);
//            }
//        }

//            m_current_childCount = transform.childCount;
//            m_enemyStore = new();
//        cts = new CancellationTokenSource();
//        waves = new List<Wave>();
//        waves.Add(CreateWave());
//    }

//    void OffsetPlayerPos()
//    {  
//        if(m_currentPlayer != null)
//        transform.position = m_currentPlayer.transform.position + offset;
//    }
//    public void Update()
//    {

//        OffsetPlayerPos();


//        if (m_currentPlayer == null)
//        {   
//            //cts.Cancel();
//            Debug.Log("Player With Tag is disappear");
//        }

//            for (int i = 0; i < waves.Count; i++)
//        {
//            if (IsInsideCircle(m_currentPlayer.position, TargetScanner.position, m_radiusTriggered))
//            {
                 
//                UniSpawnNow(waves[i],cts.Token).Forget();
//                waves.Remove(waves[i]);
//            }

//        }


       
//    }
   
   



  

//    public async UniTask UniSpawnNow(Wave _wave,CancellationToken token, System.Action callback = null)
//    {
        
         
         
//        try
//        {

//            Debug.Log("SpawnNow");

          
//            List<GameObject> setallfalse = new List<GameObject>();
//            List<string> listformation = _wave.formations;
//            int listIndex = 0, stringIndex = 0;
//            for(;listIndex<listformation.Count; listIndex++)
//            {
                
//                for (; stringIndex < listformation[listIndex].Length; stringIndex+=2)
//                {

                   
                    
//                    if (listformation[listIndex][stringIndex] == '0')
//                    {  //Find Avaible SpawnPoint With No people on room


                         
//                        GameObject ob = m_enemyStore.GetEnemiesPrototype(EntityType.Walker, (EntityI)int.Parse(listformation[listIndex][stringIndex+1].ToString()));


//                        if (ob == null)
//                            continue;
//                        ob.SetActive(true);
                          
                        
//                        IResetable ir = ob.GetComponent<IResetable>();

//                        ir?.Resetable();


                       
//                        setallfalse.Add(ob);
//                      GameObject spawnpoint = m_spawnPointManager.GetSpawnPoint(EntityType.Walker, true);
//                        ob.transform.position = spawnpoint.transform.position;
//                        ob.transform.SetParent(spawnpoint.transform);

//                    }
//                    else if (listformation[listIndex][stringIndex] == '1')
//                    {
//                        // Find Avaible SpawnPoint With No people on room
//                        // Original  GameObject ob =   m_enemyStore.GetEnemies();
//                        GameObject ob = m_enemyStore.GetEnemiesPrototype(EntityType.Fly, (EntityI)int.Parse(listformation[listIndex][stringIndex + 1].ToString()));
//                        if (ob == null)
//                            continue;

//                        ob.SetActive(true);

                        
//                        IResetable ir = ob.GetComponent<IResetable>();

//                        ir?.Resetable();

//                        setallfalse.Add(ob);
//                        GameObject spawnpoint = m_spawnPointManager.GetSpawnPoint(EntityType.Fly, true);
//                        ob.transform.position = spawnpoint.transform.position;
//                        ob.transform.SetParent(spawnpoint.transform);
//                    }
                                       
//                }

//                //set all setparent null
//                foreach(var child in setallfalse)
//                {
//                    child.transform.SetParent(null);
//                }
//                await UniTask.WaitForSeconds(m_SpawnRate, cancellationToken: token);
//            }

//            waves.Add(CreateWave());
//        }
//        catch(System.OperationCanceledException)
//        {

//        }
//        finally
//        {

//        }
//    }

    
    
//    Wave CreateWave()
//    {   
//        Wave wave = new(); 
//        wave.formations = new();

//        string formation = "";
//        // front 0 = walker, 1 = fly
//        // end 0 = Friendly, 1 = Áß¸³ 2 = Hostiles
//        string[] Source = { "01","02","11","12" };

//        int desiredFormationCount = m_maxFormationCount;
//        int desiredEnemyCount = UnityEngine.Random.Range(0, m_maxEnemiesCount);

//        int x=0,y=0;

//        for(;x<desiredFormationCount;x++)
//        {
//            for(;y<desiredEnemyCount;y++)
//            {
//                formation += Source[UnityEngine.Random.Range(0, Source.Length)];
//            }
//            wave.formations.Add(formation);
//            formation = "";
//        }    
       
//        return wave;
//    }

    

     
//}
//#if UNITY_EDITOR
//    [CustomEditor(typeof(EnemySpawner))]
//    public class ShowScanner : Editor
//    {
//        void OnSceneGUI()
//        {
//            EnemySpawner t = (EnemySpawner)target;




//            if (t.TargetScanner != null)
//            {
//                Vector3 start = t.transform.position;
//                Vector3 end = t.TargetScanner.position;







//                // Midpoint for curved arrow
//                Vector3 mid = (start + end) * 0.5f;
//                mid.y += 2.5f;

//                // Determine color based on index
//                Color arrowColor = Color.HSVToRGB(0.6f, 1f, 1f);




//                Handles.DrawBezier(start, end, mid, mid, arrowColor, null, 1f);

//                // Arrowhead at the end
//                Handles.ArrowHandleCap(
//                    0,
//                    end,
//                    Quaternion.LookRotation(end - mid),
//                    1f,
//                    EventType.Repaint
//                );

//                // Optional: label with index
//                Handles.Label(end + Vector3.up * 0.5f, $"Scanning_Area");

//                Handles.color = Color.cyan;

//                Handles.DrawWireDisc(end, Vector3.up, t.m_radiusTriggered);


//            }




//        }
//    }
 
//#endif


