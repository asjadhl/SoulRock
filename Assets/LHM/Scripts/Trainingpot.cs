using UnityEngine;

public class Trainingpot : MonoBehaviour
{
    public GameObject potPrefab;
    public GameObject brokenPotPrefab;
    public GameObject[] spawnPoint;

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
    //플레이어 총에 맞았나 판정
    public void Hitpot(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            //맞았을 때 처리
            Debug.Log("Pot Hit!");
            //뒤로 쓰러지기
            FallDown();
        }
    }
    //맞으면 뒤로 쓰러지기
    public void FallDown()
    {
        //brokenPotPrefab 으로 교체
        Instantiate(brokenPotPrefab, transform.position, transform.rotation);
        Destroy(gameObject);

    }


    //재생성후 다시 움직이기
    public void RespawnPot()
    {
        int randomIndex = Random.Range(0, spawnPoint.Length);
        Vector3 spawnPos = spawnPoint[randomIndex].transform.position;
        Instantiate(potPrefab, spawnPos, Quaternion.identity);
        Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
