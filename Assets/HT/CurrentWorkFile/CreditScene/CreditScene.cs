using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreditScene : MonoBehaviour
{
     public Animator animator;
    public RectTransform CanvasRect;
    private RectTransform myrectransform;
    public Vector2 startPos;
    public Vector2 endPos;
    public float duration = 10f;
#if UNITY_EDITOR
  [Range(0, 10f)]
#endif
  public float PlayedStart;
    public void Start()
    {
   
    if (animator == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{this.name}: Animator is Null");
#endif
            return;
        }
        var unknown = GameObject.FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None);
        var result = unknown.Where(p => p.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize).FirstOrDefault();

        if(result == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{this.name}: Canvas is Null With ScaleMode.ScaleWithScreenSize");
#endif
            return;
        }

        CanvasRect = result.GetComponent<RectTransform>();
        myrectransform = GetComponent<RectTransform>();

      //  myrectransform.sizeDelta = new Vector2(CanvasRect.sizeDelta.x, myrectransform.sizeDelta.y);
          myrectransform.sizeDelta = new Vector2(CanvasRect.rect.width, myrectransform.rect.height);
        startPos.y =  -(myrectransform.rect.height/2f);
        endPos.y =     (myrectransform.rect.height/2f);
        
        endPos.y -= CanvasRect.sizeDelta.y;
        AnimationClip clip = new();
        clip.legacy = false;   

        AnimationCurve curveX = AnimationCurve.Linear(0, startPos.x, duration, endPos.x);
        AnimationCurve curveY = AnimationCurve.Linear(0, startPos.y, duration, endPos.y);

        clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curveX);
        clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.y", curveY);
        clip.wrapMode = WrapMode.Once;

        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);

        aoc["CreditSceneClip"] = clip;
        aoc.name = "overridecontroller";
        animator.runtimeAnimatorController = aoc;
        animator.speed = 0;
         Invoke(nameof(Play), PlayedStart);
       
    }

  private void Play()
  {
    animator.speed = 1;
    animator.Play("CreditSceneClip", 0, 0f);
  }
}
