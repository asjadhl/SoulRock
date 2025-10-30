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
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAllCol)
             CheckClearLight();
    }

    void CheckClearLight()
    {
        if(DialogueLineTrueORFalse.stage1True)
        {
            clownCol.GetComponent<BoxCollider>().enabled = false;
        }
        else if(!DialogueLineTrueORFalse.stage1True)
        {
            clownCol.GetComponent<BoxCollider>().enabled = true;
        }
        if (DialogueLineTrueORFalse.stage2True)
        {
            skullLight.SetActive(true);
            clownLight.SetActive(false);
            //clownCol.enabled = false;
            clownCol.GetComponent<BoxCollider>().enabled = false;
            if(!DialogueLineTrueORFalse.stage2True)
            {
                skull.GetComponent<BoxCollider>().enabled = true;
            }
        }
        if (DialogueLineTrueORFalse.stage3_1True)
        {
            isAllCol = true;
            skullLight.SetActive(false);
            skull.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
