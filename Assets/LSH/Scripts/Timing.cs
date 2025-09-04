using UnityEngine;

//일단 해야할 것.
//1. 제거는 끝났으니 범위 또 만들어서 성공 히트박스 만들기.
//2. 큰 히트박스는 크리티컬용도 작은거는 일반데미지용도?
public class Timing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Clicker();
    }
    void Clicker()
    {
       if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("클릭 감지");
        }
    }
}
