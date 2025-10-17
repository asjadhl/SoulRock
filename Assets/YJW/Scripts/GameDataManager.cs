using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public ParticleSystem explosionParticle;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

}
