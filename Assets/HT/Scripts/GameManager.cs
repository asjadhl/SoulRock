using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public enum EntityType
{
    Walker = 0,Fly =1
}

[System.Serializable]
public class EnemiesType
{
    public  GameObject enemyobject;
    public  EntityType type;

}

public class GameManager : MonoBehaviour
{

    public Dictionary<int, List<GameObject>> dictionarys;
   
    

    public static GameManager instance;
    
    public List<EnemiesType> GhostEnemies;

    

    public EnemiesType GetRandomEnemies
    {

        get { return GhostEnemies[Random.Range(0,GhostEnemies.Count)]; }
    }

    

    public List<GameObject> GetProjectTiles;



    public GameObject CreateGameObject(EntityType type)
    {
        if (dictionarys.ContainsKey((int)type))
        {
            var list = dictionarys[(int)type];
            return Instantiate(list[Random.Range(0, list.Count)], GameObject.Find("EnemySpawner").transform.position, Quaternion.identity);
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



            dictionarys = new Dictionary<int, List<GameObject>>();
            if(GhostEnemies != null)
            {
                foreach(var child in GhostEnemies)
                {
                    if(dictionarys.ContainsKey((int)child.type))
                    {
                        dictionarys[(int)child.type].Add(child.enemyobject);
                    }
                    else
                    {
                        dictionarys.Add((int)child.type, new List<GameObject>());
                        dictionarys[(int)child.type].Add(child.enemyobject);
                    }
                }
            }

            instance = this;

        }
    }


}
