using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditScene : MonoBehaviour
{
     public Animator animator;
    public RectTransform CanvasRect;
    private RectTransform ContentRectTransform;
    public Vector2 startPos;
    public Vector2 endPos;
    public float duration = 10f;
#if UNITY_EDITOR
  [Range(0, 10f)]
#endif
  public float PlayedStart;



    public bool HideGuide;
    public Image OurImage;
    public Image ContentImage;
    public TextMeshProUGUI textMeshProUGUI;
    public Animator anim;
#if UNITY_EDITOR
    [Range(0, 1f)]

    public float alpha;
    private float prevalpha;
    private Color black = new Color(0f, 0f, 0f, 1f);
    private Color White = new Color(0f, 0f, 0f, 0f);
#endif

    public void Start()
    {   
        
        CanvasRect = transform.parent.GetComponent<RectTransform>();
        ContentImage = transform.GetChild(0).GetComponent<Image>();
        ContentImage.rectTransform.sizeDelta = new Vector2(CanvasRect.rect.width,ContentImage.rectTransform.rect.height);
    }
    

 

    //    public void Start()
    //    {

    //        try
    //        {
    //            OurImage = GetComponent<Image>();

    //            ContentImage = transform.GetChild(0).GetComponent<Image>();
    //            if (HideGuide)
    //            {
    //                ContentImage.enabled = false;
    //            }

    //            if (animator == null)
    //            {
    //#if UNITY_EDITOR
    //                Debug.LogWarning($"{this.name}: Animator is Null");
    //#endif
    //                return;
    //            }


    //            CanvasRect = transform.parent.GetComponent<RectTransform>();
    //            ContentRectTransform = transform.GetChild(0).GetComponent<RectTransform>();


    //            ContentRectTransform.sizeDelta = new Vector2(CanvasRect.rect.width, ContentRectTransform.rect.height);

    //            startPos.y = -(ContentRectTransform.rect.height / 2f);
    //            endPos.y = (ContentRectTransform.rect.height / 2f);//- CanvasRect.rect.height;


    //            AnimationClip clip = new();
    //            clip.legacy = false;

    //            AnimationCurve curveX = AnimationCurve.Linear(0, startPos.x, duration, endPos.x);
    //            AnimationCurve curveY = AnimationCurve.Linear(0, startPos.y, duration, endPos.y);

    //            clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curveX);
    //            clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.y", curveY);
    //            clip.wrapMode = WrapMode.Once;

    //            AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);

    //            aoc["CreditSceneClip"] = clip;
    //            aoc.name = "overridecontroller";
    //            animator.runtimeAnimatorController = aoc;
    //            animator.speed = 0;
    //            Invoke(nameof(Play), PlayedStart);
    //        }
    //        catch (System.Exception ex)
    //        {
    //            textMeshProUGUI.text = ex.Message;


    //        }

    //    }


  //  private void Play()
  //{
  //  animator.speed = 1;
  //  animator.Play("CreditSceneClip", 0, 0f);
  //}


#if UNITY_EDITOR
    public void UpdateImageAlpha()
    {
        if (prevalpha != alpha)
        {
            ContentImage.color = Color.Lerp(black, White, alpha);
            prevalpha = alpha;
        }
    }
#endif 
    private  void Update()
    {
#if UNITY_EDITOR
        UpdateImageAlpha();
#endif
    }
}
