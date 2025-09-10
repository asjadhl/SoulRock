using UnityEngine;
using UnityEngine.UIElements;

public class SpawnTimer : MonoBehaviour
{
    float timer = 0;
    [Header("언제부터 빨라지는지")]
    [SerializeField] int firstTimer = 0; //부터
    [Header("언제까지 빨라지는지")]
    [SerializeField] int lastTimer = 0; //까지
    DotBoxGeneratorL dotBoxGenL;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        dotBoxGenL = GetComponent<DotBoxGeneratorL>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTimer();
    }

    void CheckTimer()
    {
        timer += Time.deltaTime;
        if((int)timer == firstTimer)
        {
            dotBoxGenL.dotboxTime *= 2;
            Debug.Log(dotBoxGenL.dotboxTime);
        }
        if ((int)timer >= lastTimer)
        {
            dotBoxGenL.dotboxTime /= 2;
            Debug.Log(dotBoxGenL.dotboxTime);
        }
    }
}
