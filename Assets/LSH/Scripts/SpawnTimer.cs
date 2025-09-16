using UnityEngine;
using UnityEngine.UIElements;

public class SpawnTimer : MonoBehaviour
{
    float timer = 0;
    [Header("FirstTime")]
    [SerializeField] int[] firstTimer; //부터
    [Header("LastTime")]
    [SerializeField] int[] lastTimer; //까지


    [Header("DotSpeedChange")]
    [SerializeField] int[] dotChageSpeed; //까지
    [Header("Dot Partents")]
    [SerializeField] GameObject DotBoxGeneL;
    [SerializeField] GameObject DotBoxGeneR;

    DotBoxGeneratorL dotBoxGenL;
    DotBoxGeneratorR dotBoxGenR;
    int normalSpeed = 0;
    bool calones = true;

    int i = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        dotBoxGenL = DotBoxGeneL.GetComponent<DotBoxGeneratorL>();
        dotBoxGenR = DotBoxGeneR.GetComponent<DotBoxGeneratorR>();
        //normalSpeed = dotBoxGenL.dotboxTime;

    }

    // Update is called once per frame
    void Update()
    {
        CheckTimer();
        //Debug.LogWarning("i = " + i);
        //Debug.LogError("timer = " + (int)timer);
    }

    void CheckTimer()
    {
        timer += Time.deltaTime;
        if (i < firstTimer.Length)
        {
            if ((int)timer == firstTimer[i] && calones)
            {
                //    Debug.Log(i);
                //    calones = false;
                //    dotBoxGenL.dotboxTime = dotChageSpeed[i];
                //    dotBoxGenR.dotboxTime = dotChageSpeed[i];
                //    Debug.Log(dotBoxGenL.dotboxTime);
                //}
                //if ((int)timer == lastTimer[i] && !calones)
                //{
                //    calones = true;
                //    i++;
                //    dotBoxGenL.dotboxTime = normalSpeed;
                //    dotBoxGenR.dotboxTime = normalSpeed;
                //    Debug.Log(dotBoxGenL.dotboxTime);
                //}
            }

        }
    }
}
