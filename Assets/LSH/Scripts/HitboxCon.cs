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

    [Header("Combo")]
    public int combo = 0;
    public GameObject[] comboImage;
    public bool test = false;
    AudioSource a;
    [SerializeField] AudioClip clip;
    [SerializeField] PlayerShoot player;



    private void Start()
    {
        a = GetComponent<AudioSource>();
        for(int i = 0; i <comboImage.Length; i++)
        {
            comboImage[i].SetActive(false);
        }
        player = FindAnyObjectByType<PlayerShoot>();

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

                if (Input.GetMouseButtonDown(0) && distLeftRight >= minClick && distLeftRight <= maxClick)
                {
                    OnClickSuccess();
                    DotBoxGeneratorL.Instance.ReturnDot(left.gameObject);
                    DotBoxGeneratorR.Instance.ReturnDot(right.gameObject);
                    break;
                }
            }
        }
    }


    public void OnClickSuccess()
    {
        combo++;
        a.PlayOneShot(clip);
        player.PlayerShoot_();
        switch(combo)
        {
            case <10:
                comboImage[0].SetActive(true);
                break;
            case <20:
                comboImage[1].SetActive(true);
                break;
            case <30:
                comboImage[2].SetActive(true);
                break;
            case <40:
                comboImage[3].SetActive(true);
                break;
            case < 50:
                comboImage[4].SetActive(true);
                break;
        }
    }

    
}
