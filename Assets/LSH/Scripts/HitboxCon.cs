using Unity.VisualScripting;
using UnityEngine;

//일단 해야할 것. 0.밥먹으러가기
//1. 거리로 제거랑 클릭판정만들기.
public class HitBoxCon: MonoBehaviour
{
    [Header("Click distance")]
    public float minClick = 100f;
    public float maxClick = 200f;

    [Header("Left Right dot parents")]
    public Transform leftDotBox;
    public Transform rightDotBox;
    AudioSource a;
    [SerializeField] AudioClip clip;
    [SerializeField] GameObject player;
    private void Start()
    {
        a = GetComponent<AudioSource>();
    }

    private void Update()
    {
            TryClick();
        
    }

    private void TryClick() //GURI GUHASU 
    {
        RectTransform[] leftDots = leftDotBox.GetComponentsInChildren<RectTransform>();
        RectTransform[] rightDots = rightDotBox.GetComponentsInChildren<RectTransform>();

        for (int i = leftDots.Length - 1; i >= 0; i--)
        {
            RectTransform left = leftDots[i];
            if (!left.gameObject.activeSelf) continue;

            for (int j = rightDots.Length - 1; j >= 0; j--)
            {
                RectTransform right = rightDots[j];
                if (!right.gameObject.activeSelf) continue;

                float distLeftRight = Vector2.Distance(left.position, right.position);

                if (Input.GetMouseButtonDown(0)&&distLeftRight >= minClick && distLeftRight <= maxClick)
                {
                    OnClickSuccess();
                    DotBoxGeneratorL.Instance.ReturnDot(left.gameObject);
                    DotBoxGeneratorR.Instance.ReturnDot(right.gameObject);
                    break;
                }
            }
        }
    }

    private void OnClickSuccess()
    {

        a.PlayOneShot(clip);
        player.GetComponent<PlayerShoot>().PlayerShoot_();
        Debug.Log("CLICK SUNGGONG!");
        
        // 클릭 성공 시 처리할 로직
        // 예: 좌/우 도트 비활성화, 점수 증가 등
    }
}
