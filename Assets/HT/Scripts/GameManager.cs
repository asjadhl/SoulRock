using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{ 

    public static GameManager instance;

    public List<GameObject> GhostEnemies;

    public GameObject GetRandomEnemies
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
