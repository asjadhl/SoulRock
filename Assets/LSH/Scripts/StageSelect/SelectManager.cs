using UnityEngine;

public class SelectManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject clownLight;
    [SerializeField] GameObject skullLight;
    [SerializeField] BoxCollider clownCol;
    [SerializeField] BoxCollider skull;
    bool isAllCol = false;
    private void Awake()
    {
        //clownCol = GetComponent<BoxCollider>();
    }
    void Start()
    {
        skullLight.SetActive(false);
        clownLight.SetActive(true);
        skull.GetComponent<BoxCollider>().enabled = false;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAllCol)
             CheckClearLight();
    }

    void CheckClearLight()
    {
        if(DialogueLineTrueORFalse.stage1True)//대사 시작
        {
            clownCol.GetComponent<BoxCollider>().enabled = false;
        }
        else if(!DialogueLineTrueORFalse.stage1True&&!BossState.isBoss1Dead)//대사 끝
        {
            clownCol.GetComponent<BoxCollider>().enabled = true;
        }
        else if (BossState.isBoss1Dead&&DialogueLineTrueORFalse.stage2True) //보스1이 죽고 대사 시작
        {
            Debug.LogError("보스1 사망");
            skullLight.SetActive(true);
            clownLight.SetActive(false);
            //clownCol.enabled = false;
            clownCol.GetComponent<BoxCollider>().enabled = false;
        }
        else if (!DialogueLineTrueORFalse.stage2True && BossState.isBoss1Dead) //보스1이 죽고 대사 끝
        { 
            clownLight.SetActive(false);
            skull.GetComponent<BoxCollider>().enabled = true;
        }
        else if(BossState.isBoss2Dead&&DialogueLineTrueORFalse.stage3_1True)
        {
            Debug.LogError("보스2 사망"); 
            isAllCol = true;
            skullLight.SetActive(false);
            clownLight.SetActive(false);
            skull.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
