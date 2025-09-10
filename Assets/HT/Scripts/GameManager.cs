using System.Collections.Generic;
using UnityEngine;





public enum EntityType
{
    Walker,Fly
}

[System.Serializable]
public class EnemiesType
{
    public  GameObject gameObject;
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

    public void Awake()
    {
        if (instance !=  null)
            Destroy(this.gameObject);
        else
            instance = this;

    }
}
