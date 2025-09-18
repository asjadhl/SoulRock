using UnityEngine;

public class CheckRealTime : MonoBehaviour
{
    public static CheckRealTime Instance { get; private set; }

    public double inGamerealTime = 0;

    int plusTime = 0;
    void Awake()
    {
        // ΩÃ±€≈Ê
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            plusTime += 10;
        }
        inGamerealTime = AudioSettings.dspTime - DotBoxGeneratorL.Instance.musicStartDspTime + plusTime;
        Debug.Log((int)inGamerealTime);
    }
}
