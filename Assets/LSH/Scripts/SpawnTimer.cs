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
    double realTimer = 0;

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
    }

    // Update is called once per frame
    void Update()
    {
        realTimer =  AudioSettings.dspTime - dotBoxGenL.musicStartDspTime;
        CheckTimer();
    }
    
    void CheckTimer()
    {
        if (i < firstTimerFast.Length)
        {
            if (((int)realTimer == firstTimerFast[i] && calones))
            {
                calones = false;
                doubleDotSpeed = true;
            }
            if ((int)realTimer == lastTimerFast[i] && !calones)
            {
                calones = true;
                doubleDotSpeed = false;
                i++;
                //dotBoxGenL.dotboxTime = normalSpeed;
                //dotBoxGenR.dotboxTime = normalSpeed;
                //Debug.Log(dotBoxGenL.dotboxTime);
            }
        }
        if(j < lastTimerFast.Length)
        {
            if (((int)realTimer == firstTimerSlow[j] && calones))
            {
                calones = false;
                lowDoubleDotSpeed = true;
            }
            if ((int)realTimer == lastTimerSlow[j] && !calones)
            {
                calones = true;
                lowDoubleDotSpeed = false;
                j++;
                //dotBoxGenL.dotboxTime = normalSpeed;
                //dotBoxGenR.dotboxTime = normalSpeed;
                //Debug.Log(dotBoxGenL.dotboxTime);
            }
        }
        
    }
}
