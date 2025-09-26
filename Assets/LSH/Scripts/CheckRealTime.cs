using UnityEngine;

public class CheckRealTime : MonoBehaviour
{
    static public double inGamerealTime = 0;

    int plusTime = 0;

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
