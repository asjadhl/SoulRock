using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

public static class StageBool
{
    public static bool Stage1Clear = false;
    public static bool Stage2Clear = false;
    public static bool Stage3Clear = false;
}

public class StageSelect : MonoBehaviour
{
    public GameObject stage1on;
    public GameObject stage1off;
    public GameObject stage2Lock;

    void Start()
    {
        
    }

    void Update()
    {
        if (StageBool.Stage1Clear == true)
        {
            stage2Lock.SetActive(false);
            stage1on.SetActive(false);
            stage1off.SetActive(true);
        }
    }
    
}
