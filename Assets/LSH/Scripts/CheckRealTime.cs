using UnityEngine;

public class CheckRealTime : MonoBehaviour
{
    public static CheckRealTime Instance { get; private set; }

    public double inGamerealTime = 0;
    void Awake()
    {
        // ΩÃ±€≈Ê
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        inGamerealTime = AudioSettings.dspTime - DotBoxGeneratorL.Instance.musicStartDspTime;
    }
}
