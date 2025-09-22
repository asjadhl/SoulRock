using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.WSA;



public class EnemyStore1
{
    Dictionary<string, List<GameObject>> dict = new();
    Dictionary<string, List<GameObject>> SpawnPointStore = new();

    public void AddSpawnPoint(Transform SpawnPointParent)
    {
        //0 is walk and 1 is fly
     
        for (int i = 0; i < SpawnPointParent.childCount; i++)
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
       GameObject SpawnObj = SpawnPointStore[datas].Find(p => p.transform.childCount <= 0);
        if (SpawnObj == null)
            return null;
        else return SpawnObj.transform;
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

        if (dict[datas] == null)
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
            Debug.Log("No Available EnemyObj!");
            AddEnemies(datas); Debug.Log("Creating New One");
            if (x == null)
            {
                Debug.LogError("It seem the GameManager.object is been removed during runtime");
                return null;  
            }


            x = list.Find(p => p.activeSelf == false);
            x.SetActive(true);
            return x;
        }
    }

}

 
public class EnemySpawner1 : MonoBehaviour
{

    EnemyStore1 enemyStore1;
    CancellationTokenSource m_cts;

    private void Start()
    {   
         enemyStore1 = new EnemyStore1();

        enemyStore1.AddSpawnPoint(transform);

        m_cts = new CancellationTokenSource();

        Spawning(m_cts).Forget();
    }
    public string CreateEnemyWave()
   {     //Type/EntityHostile        State
        // [00]-                      [00]-                 [00]
        string[] EntityType ={"0","1"};
        string[] EntityI    ={"1","2"};

        string formation = "";

        int desireCount = 10;

        for (int i = 0; i < desireCount; i++)
        {
            formation +=    EntityType[UnityEngine.Random.Range(0, EntityType.Length)]
                      +     EntityI   [UnityEngine.Random.Range(0, EntityType.Length)];
        }    
        
        return formation;
   }

   
    public async UniTaskVoid Spawning(CancellationTokenSource cts)
    {
        List<GameObject> Holder = new List<GameObject>();
        try
        {
            string newformation;
            newformation = CreateEnemyWave();
            

            while (true)
            {
                for (int i = 0; i < newformation.Length; i = GameManager.instance.LenghtOfStringData)
                {

                    Transform SpawnPoint = enemyStore1.GetSpawnPoint((newformation[i] + newformation[i + 1]).ToString());
                    if (SpawnPoint == null)
                        continue;
                    GameObject newEnemy = enemyStore1.GetEnemies((newformation[i] + newformation[i + 1]).ToString());
                    if (newEnemy == null)
                        continue;

                    Holder.Add(newEnemy);
                    newEnemy.transform.SetParent(SpawnPoint);
                }
                    foreach (var child in Holder)
                    child.transform.SetParent(null);
                await UniTask.WaitForSeconds(3f, cancellationToken: cts.Token);
                newformation = CreateEnemyWave();
            }
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
