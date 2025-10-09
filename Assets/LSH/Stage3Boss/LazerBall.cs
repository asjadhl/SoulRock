using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class LazerBall : MonoBehaviour
{
    [Header("LazerBallSpeed")]
    [SerializeField] float lazerBallspeed = 4f;

    private Transform player;
    private Transform boss;

    //Vector3 oriPos;
    float x;
    float y;

    private void Awake()
    {
        x = transform.position.x;
        y = transform.position.y;
    }

    //private void FixedUpdate()
    //{
    //oriPos = new Vector3(x, y, boss.position.z);
    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        boss = GameObject.FindWithTag("Stage3Boss").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeSelf == true)
        {
            lazerMove();
        }

    }

    public async void lazerMove()
    {
        //Vector3 targetPos = player.position;
        //lazerBallPool[j].transform.position = Vector3.MoveTowards(
        //lazerBallPool[j].transform.position, targetPos, lazerBallspeed * Time.deltaTime);
        //await UniTask.Delay(100);

        await UniTask.Delay(3000);

        transform.LookAt(player.position);
        transform.Translate(Vector3.forward * lazerBallspeed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
            gameObject.SetActive(false);
        }
    }
}
