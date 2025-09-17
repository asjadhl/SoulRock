using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnTimer : MonoBehaviour
{
    
    [Header("GettingFast")]
    [SerializeField] int[] firstTimerFast; //부터
    [Header("BacktoNormal")]
    [SerializeField] int[] lastTimerFast; //까지
    [Header("GettingSlow")]
    [SerializeField] int[] firstTimerSlow; //부터
    [Header("BacktoNormal")]
    [SerializeField] int[] lastTimerSlow; //까지

    [Header("Dot Partents")]
    [SerializeField] GameObject DotBoxGeneL;
    [SerializeField] GameObject DotBoxGeneR;
    DotBoxGeneratorL dotBoxGenL;
    DotBoxGeneratorR dotBoxGenR;
    int normalSpeed = 0;
    bool calones =true;

    //이거는 각 스피드 Bool
    public bool doubleDotSpeed = false; //2배속용 bool값(DotBoxGenerator에 사용할꺼임)
    public bool lowDoubleDotSpeed =false; //0.5배속용 bool값
    int i = 0;
    int j = 0;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dotBoxGenL = DotBoxGeneL.GetComponent<DotBoxGeneratorL>();
        dotBoxGenR = DotBoxGeneR.GetComponent<DotBoxGeneratorR>();
        normalSpeed = dotBoxGenL.dotboxTime;
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.LogError((int)realTimer);
        realTimer =  AudioSettings.dspTime - dotBoxGenL.musicStartDspTime;
        CheckTimer();
    }
    
    void CheckTimer()
    {
        timer += Time.deltaTime;
        if (i <firstTimer.Length)
        {
            if (((int)realTimer == firstTimerFast[i] && calones))
            {
                Debug.Log(i);
                calones = false;
                dotBoxGenL.dotboxTime = dotChageSpeed[i];
                dotBoxGenR.dotboxTime = dotChageSpeed[i];
                Debug.Log(dotBoxGenL.dotboxTime);
            }
            if ((int)timer == lastTimer[i] && !calones)
            {
                calones = true;
                i++;
                dotBoxGenL.dotboxTime = normalSpeed;
                dotBoxGenR.dotboxTime = normalSpeed;
                Debug.Log(dotBoxGenL.dotboxTime);
            }
        }
        
    }
}
