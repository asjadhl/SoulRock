using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
 



public class EnemyStore1
{
    Dictionary<string, List<GameObject>> dict = new();
    Dictionary<string, List<GameObject>> SpawnPointStore = new();

    public void AddSpawnPoint(Transform SpawnPointParent)
    {
    //0 is walk and 1 is fly

                                             //юс╫ц
    for (int i = 0; i < SpawnPointParent.childCount-2; i++)
        {
                                                                                                     
            if (!SpawnPointStore.ContainsKey(SpawnPointParent.GetChild(i).name[0].ToString()))
            {
                SpawnPointStore.Add(SpawnPointParent.GetChild(i).name[0].ToString(),
                                     new List<GameObject>());
            }
            
            SpawnPointStore[SpawnPointParent.GetChild(i).name[0].ToString()].Add(SpawnPointParent.GetChild(i).gameObject);
        }
    }
    public Transform GetSpawnPoint(string datas)
    {
        
        if (SpawnPointStore.ContainsKey(datas[0].ToString()))
        {
        
            GameObject SpawnObj = SpawnPointStore[datas[0].ToString()].Find(p => p.transform.childCount <= 0);
            if (SpawnObj == null)
                return null;
            else return SpawnObj.transform;
        }
        return null;
    }
    public void AddEnemies(string datas)
    {
        var obj = GameManager.instance.CreateEnemy(datas);


        if (obj == null)
            return;

        obj.SetActive(false);

          if(!dict.ContainsKey(datas))
            dict.Add(datas, new List<GameObject>());
         
            dict[datas].Add(obj);

    }

    public GameObject GetEnemies(string datas)
    {
        if (!dict.ContainsKey(datas))
            AddEnemies(datas);

        if (!dict.ContainsKey(datas))
        {
            Debug.Log("GameManager.CreateEnemies is Null");  
            return null; }

        var list = dict[datas];
        var x = list.Find(p => p.activeSelf == false);
        if (x != null)
        {
            x.SetActive(true);
            return x; }
        else
        {
            
            AddEnemies(datas); 

            //Check if it is Added

            x = list.Find(p => p.activeSelf == false);
            if (x == null)
            {
                Debug.LogError("It seem the GameManager.object is been removed during runtime");
                return null;  
            }

            x.SetActive(true);
            return x;
        }
    }

}

 
public class EnemySpawner1 : MonoBehaviour
{

    EnemyStore1 enemyStore1;
    CancellationTokenSource m_cts;
    [SerializeField] Transform m_currentPlayer;
    public Transform TargetScanner;
    public float m_radiusTriggered = 2f;
    [UnityEngine.Range(0, 10)] public float m_SpawnRate = 7f;
    private float timer = 0;
    [SerializeField] Vector3 offset;
    private void Start()
    {   
         enemyStore1 = new EnemyStore1();

        enemyStore1.AddSpawnPoint(transform);

        m_cts = new CancellationTokenSource();

        

        m_currentPlayer = GameObject.FindWithTag("Player").transform;

    GameObject scanner = new GameObject("Scanner");
    scanner.transform.SetParent(m_currentPlayer);

    scanner.gameObject.SetActive(false);
    }
    void OffsetPlayerPos()
    {
        if (m_currentPlayer != null)
            transform.position = m_currentPlayer.transform.position + offset;
    }
    public string CreateEnemyWave()
   {     //Type/EntityHostile        State
        // [00]-                      [00]-                 [00]
        string[] EntityType ={"0","1"};
        string[] EntityI    ={"1","2"};

        string formation = "";

        int desireCount = UnityEngine.Random.Range(3,10);

        for (int i = 0; i < desireCount; i++)
        {
            formation +=    EntityType[UnityEngine.Random.Range(0, EntityType.Length)]
                      +     EntityI   [UnityEngine.Random.Range(0, EntityType.Length)];
        }    
        
        return formation;
   }

    public void Update()
    {
        OffsetPlayerPos();

        if (m_currentPlayer == null)
        {
            
            Debug.LogError("Player With Tag is disappear");
        }

                timer += Time.deltaTime;
            if (IsInsideCircle(m_currentPlayer.position, TargetScanner.position, m_radiusTriggered) && timer >= m_SpawnRate)
            {
              timer = 0;
              Spawning(m_cts).Forget();
                 
            }

        
    }
    public static bool IsInsideCircle(Vector3 pos, Vector3 center, float radius)
    {
        return (pos - center).sqrMagnitude <= radius * radius;
    }
    public async UniTaskVoid Spawning(CancellationTokenSource cts)
    {
        List<GameObject> Holder = new List<GameObject>();
        try
        {
            string newformation;
            newformation = CreateEnemyWave();
            

             
                for (int i = 0; i < newformation.Length; i += GameManager.instance.LenghtOfStringData)
                {
                    string datas =  newformation[i].ToString() + newformation[i + 1].ToString();
                                 
                    Transform SpawnPoint = enemyStore1.GetSpawnPoint(datas);
                    if (SpawnPoint == null)
                        continue;
                 
                GameObject newEnemy = enemyStore1.GetEnemies(datas);
                    if (newEnemy == null)
                        continue;

                    Holder.Add(newEnemy);
                    newEnemy.transform.position = SpawnPoint.position;
                    newEnemy.transform.SetParent(SpawnPoint);
                
                }
            foreach (var child in Holder)
                child.transform.SetParent(null);

            await UniTask.Yield(cancellationToken: cts.Token);
             
        }
        catch (System.OperationCanceledException)
        {
                foreach (var child in Holder)
                child.transform.SetParent(null);
        }
    }


    private void OnDisable()
    {  

        m_cts.Cancel();
    }
}
#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(EnemySpawner1))]
public class ShowScanner : UnityEditor.Editor
{
    void OnSceneGUI()
    {
        EnemySpawner1 t = (EnemySpawner1)target;




        if (t.TargetScanner != null)
        {
            Vector3 start = t.transform.position;
            Vector3 end = t.TargetScanner.position;







            // Midpoint for curved arrow
            Vector3 mid = (start + end) * 0.5f;
            mid.y += 2.5f;

            // Determine color based on index
            Color arrowColor = Color.HSVToRGB(0.6f, 1f, 1f);




            UnityEditor.Handles.DrawBezier(start, end, mid, mid, arrowColor, null, 1f);

            // Arrowhead at the end
            UnityEditor.Handles.ArrowHandleCap(
                0,
                end,
                Quaternion.LookRotation(end - mid),
                1f,
                EventType.Repaint
            );

            // Optional: label with index
            UnityEditor.Handles.Label(end + Vector3.up * 0.5f, $"Scanning_Area");

            UnityEditor.Handles.color = Color.cyan;

            UnityEditor.Handles.DrawWireDisc(end, Vector3.up, t.m_radiusTriggered);


        }




    }
}

#endif


