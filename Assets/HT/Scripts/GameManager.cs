using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;





public enum EntityType
{
    Walker = 0,Fly =1
}
public enum EntityI
{
    Friendly,chonglib,Hostile
}
[System.Serializable]
public class EnemiesType
{
    public  GameObject enemyobject;
    public  EntityType type;
    public  EntityI    entityI;

}


 
public class GameManager : MonoBehaviour
{

    public Dictionary<string, List<EnemiesType>> dictionarys;
    public Dictionary<string, List<GameObject>> dict1;
    public int LenghtOfStringData;
    public static GameManager instance;
    
    public List<EnemiesType> GhostEnemies;

    public List<GameObject> GetProjectTiles;

     
    public GameObject CreateEnemy(string datas)
    {
        
        if (dict1.ContainsKey(datas))
        {
            int randomIndex = UnityEngine.Random.Range(0, dict1[datas].Count);
            return Instantiate(dict1[datas][randomIndex], GameObject.Find("EnemySpawner").transform.position, Quaternion.identity);
        }
        else
            return null;
    }
    public GameObject CreateGameObject(EntityType type, EntityI entityI = EntityI.Hostile)
    {
        if (dictionarys.ContainsKey(type.ToString()+entityI.ToString()))
        {  
           
            var list = dictionarys[type.ToString() + entityI.ToString()];
            
            int ran = UnityEngine.Random.Range(0, list.Count);
            var obj  =  Instantiate(list[ran].enemyobject,GameObject.Find("EnemySpawner").transform.position,Quaternion.identity);
           
                 obj.GetComponent<EnemyGhost>().Init(list[ran].type, list[ran].entityI);

            return obj;
        }
        else
        {
            //Debug.LogError("Current Entity Type doesnt Exist on Dictionary");
            return null;
        }
    }
  
    public void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
        {

           


            dictionarys = new();
            if(GhostEnemies != null)
            {
                // New Prototype
                dict1 = new();
                foreach(var each in GhostEnemies)
                {
                  string newdata =  each.type.ToString() + each.entityI.ToString();

                    if (!dict1.ContainsKey(newdata))
                        dict1.Add(newdata, new List<GameObject>());

                    dict1[newdata].Add(each.enemyobject);
                }




                foreach (var child in GhostEnemies)
                {
                    if (dictionarys.ContainsKey(child.type.ToString() + child.entityI.ToString()))
                    {
                        dictionarys[child.type.ToString() + child.entityI.ToString()].Add(child);
                    }
                    else
                    {
                        dictionarys.Add(child.type.ToString() + child.entityI.ToString(), new List<EnemiesType>());
                        dictionarys[child.type.ToString() + child.entityI.ToString()].Add(child);
                    }
                }
            }

            instance = this;

        }
    }


}
