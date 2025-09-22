using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Invisible dotPrefab")]
    public RawImage dotL;
    public RawImage dotR;

    //Combo 상승
    [Header("Combo")]
    public int combo = 0;
    public GameObject[] comboImage;
    //public Color color;

    //실험용 Bool값
    public bool test = false;
    AudioSource a;
    [SerializeField] AudioClip clip;
    [SerializeField] GameObject player;



    private void Start()
    {
        int[] colorValue = new int[4];
        //color = new Color(colorValue[0], colorValue[1], colorValue[2], colorValue[3]);
        a = GetComponent<AudioSource>();
    }

    private void Update()
    {
        TryClick();
    }

    private void TryClick() 
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
        combo++;
        a.PlayOneShot(clip);
        player.GetComponent<PlayerShoot>().PlayerShoot_();
        Debug.Log("클릭성공!");
        // 클릭 성공 시 처리할 로직
        // 예: 좌/우 도트 비활성화, 점수 증가 등
        switch(combo)
        {
            case <25:
                comboImage[0].SetActive(true);
                break;
            case <50:
                comboImage[1].SetActive(true);
                break;
            case < 75:
                comboImage[2].SetActive(true);
                break;
            case < 100:
                comboImage[3].SetActive(true);
                break;
        }
    }

    
}
