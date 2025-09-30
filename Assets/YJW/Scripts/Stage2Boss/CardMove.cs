using Cysharp.Threading.Tasks;
using UnityEngine;

public class CardMove : MonoBehaviour
{
    float x = 0;
    float y = 0;
    private Vector3 oriPos;

    [SerializeField] GameObject boss;
    [SerializeField] GameObject player;

    private float moveSpeed = 6;

    private bool canMove = true;

    private void Awake()
    {
        x = transform.position.x;
        y = transform.position.y;
    }

    private void FixedUpdate()
    {
        if(gameObject.activeSelf == true)
        {
            oriPos = new Vector3(x, y, boss.transform.position.z);
            if(canMove == true)
            {
                CardRotate();
                CardMove_();
            }
            else
            {
                transform.Translate(0, -1 * Time.fixedDeltaTime * 3, 0);
            }
        }
    }

    private void CardRotate()
    {
        transform.Rotate(0, 15, 0);
    }

    private void CardMove_()
    {
        transform.Translate(0, 0, -1 * moveSpeed * Time.fixedDeltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어한테 닿음");
            if (gameObject.tag == "RedCard")
                player.GetComponent<PlayerHP>().PlayerHPMinus();
            else if(gameObject.tag == "GoldCard")
            {
                _=dotBoxTrans();
            }
            ReturnOriPos();
        }

        if(other.gameObject.CompareTag("Ground"))
        {
            ReturnOriPos();
        }
    }

    private void ReturnOriPos()
    {
        transform.position = oriPos;
        canMove = true;
        gameObject.SetActive(false);
    }

    public void CardGetDam()
    {
        canMove = false;
    }

    private async UniTask dotBoxTrans()
    {
        DotBoxGeneratorL.Instance.getDamage = true;
        DotBoxGeneratorR.Instance.getDamage= true;

        await UniTask.Delay(3000);

        DotBoxGeneratorL.Instance.getDamage = false;
        DotBoxGeneratorR.Instance.getDamage = false;

    }
}
