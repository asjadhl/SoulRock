using UnityEngine;

public class SelectManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject clownLight;
    [SerializeField] GameObject skullLight;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckClearLight();
    }

    void CheckClearLight()
    {
        if(BossState.isBoss1Dead == true)
        {
            clownLight.SetActive(false);
        }
        if (BossState.isBoss2Dead == true)
        {
            skullLight.SetActive(false);
        }
    }
}
