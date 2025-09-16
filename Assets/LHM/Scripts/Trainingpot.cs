using UnityEngine;

public class Trainingpot : MonoBehaviour
{
    public int potLevel = 1;
    public int maxPotLevel = 5;
    public int maxHp = 10;
    public int currentHp;
    
    public float growthTime = 60f;
    public float EndTime;
    public float speed = 1f;

    //양옆으로 이동
    public float moveRange = 5f;
    public void Movepot()
    {
        float newX = Mathf.PingPong(Time.time * speed, moveRange * 2) - moveRange;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
    //맞았나 판정
    //맞으면 뒤로 쓰러지기
    //쓰러지면 일정시간 뒤에 다시 일어나기
    //일어나면 다시 움직이기
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
