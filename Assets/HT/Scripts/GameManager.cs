using System.Collections.Generic;
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


      Vector3 Min;
      Vector3 Max; 

    public Vector3 Clamp(Vector3 current)
    {
     
    Vector3 result;
    float xmin = Min.x;
   
    float xmax = Max.x;
    
    float ymin = Min.y;
  
    float ymax = Max.y;
    
    result.x = Mathf.Clamp(current.x, xmin, xmax);
    
    result.y = Mathf.Clamp(current.y, ymin, ymax);
 
    result.z = current.z;
    return result;
    }
    public GameObject CreateEnemy(string datas)
    {

        if (dict1.ContainsKey(datas))
        {   
         
            int randomIndex = UnityEngine.Random.Range(0, dict1[datas].Count);
            var obj = Instantiate(dict1[datas][randomIndex], GameObject.Find("EnemySpawner").transform.position, Quaternion.identity);
            obj.GetComponent<Enemy>().SetData(datas);
            obj.name += $"[{(EntityI)int.Parse(datas[1].ToString())}]";
            return obj;
        }
        else  
            return null;
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
                    string newdata = ((int)each.type).ToString() + ((int)each.entityI).ToString();
                  
                    if (!dict1.ContainsKey(newdata))
                        dict1.Add(newdata, new List<GameObject>());

                    dict1[newdata].Add(each.enemyobject);
                     
                }
               
            }

      //-------------------------//
      GameObject EnemySpawner = GameObject.Find("EnemySpawner");
      if (EnemySpawner != null)
      {
        int childcount = EnemySpawner.transform.childCount;
        Max = EnemySpawner.transform.GetChild(childcount - 1).localPosition;
        Min = EnemySpawner.transform.GetChild(childcount - 2).localPosition;
      }
       
      //-------------------------//
      instance = this;

        }
    }


}
