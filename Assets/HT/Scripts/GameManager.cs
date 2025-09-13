using System.Collections.Generic;
using UnityEngine;





public enum EntityType
{
    Walker,Fly
}

[System.Serializable]
public class EnemiesType
{
    public  GameObject enemyobject;
    public  EntityType type;

}

public class GameManager : MonoBehaviour
{ 



    public static GameManager instance;

    public List<EnemiesType> GhostEnemies;

    public EnemiesType GetRandomEnemies
    {

        get { return GhostEnemies[Random.Range(0,GhostEnemies.Count)]; }
    }

    public List<GameObject> GetProjectTiles;

   

   
    public void Start()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }


}
