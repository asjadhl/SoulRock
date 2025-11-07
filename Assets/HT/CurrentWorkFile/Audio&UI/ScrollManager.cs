using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    public ScrollRect _scrollrect;
    public ScrollButton ScrollButtonTop;
    public ScrollButton ScrollButtonBottom;

    [Range(0f, 1f)]
    public float ScrollPerSecond = 0.25f;  

    private float startTime;
    private float startPos;
    private bool isScrollingTop = false;
    private bool isScrollingBottom = false;

    private void Update()
    {
        
        if (ScrollButtonTop != null && ScrollButtonTop.IsDown && !isScrollingTop)
        {
            isScrollingTop = true;
            isScrollingBottom = false;
            startTime = Time.unscaledTime;
            startPos = _scrollrect.verticalNormalizedPosition;
        }
        if (ScrollButtonTop != null && !ScrollButtonTop.IsDown)
            isScrollingTop = false;

        
        if (ScrollButtonBottom != null && ScrollButtonBottom.IsDown && !isScrollingBottom)
        {
            isScrollingBottom = true;
            isScrollingTop = false;
            startTime = Time.unscaledTime;
            startPos = _scrollrect.verticalNormalizedPosition;
        }
        if (ScrollButtonBottom != null && !ScrollButtonBottom.IsDown)
            isScrollingBottom = false;

        
        if (isScrollingTop)
            _scrollrect.verticalNormalizedPosition = Mathf.Clamp01(startPos + (Time.unscaledTime - startTime) * ScrollPerSecond);

        if (isScrollingBottom)
            _scrollrect.verticalNormalizedPosition = Mathf.Clamp01(startPos - (Time.unscaledTime - startTime) * ScrollPerSecond);
    }
}
