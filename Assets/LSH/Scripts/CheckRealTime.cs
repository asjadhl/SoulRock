using Unity.VisualScripting;
using UnityEngine;

public class CheckRealTime : MonoBehaviour
{
	public static CheckRealTime Instance { get; private set; }
	static public double inGamerealTime = 0;

    int plusTime = 0;
    public double startTime;

	void Awake()
	{
		// ΩÃ±€≈Ê
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);
	}
	private void Start()
	{
        startTime = AudioSettings.dspTime;
	}
	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            plusTime += 10;
        }
        inGamerealTime = AudioSettings.dspTime - startTime + plusTime;
		Debug.LogWarning((int)inGamerealTime);
	}
}
