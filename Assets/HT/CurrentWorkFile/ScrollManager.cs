using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    public ScrollRect _scrollrect;
    public ScrollButton ScrollButtonTop;
    public ScrollButton ScrollButtonBottom;
    [Range(0f,1f)]
    public float ScrollSpeed = 0.001f;

    public void ScrollBottom()
    {
        if (_scrollrect.verticalNormalizedPosition <= 1)
        {

            _scrollrect.verticalNormalizedPosition += ScrollSpeed*Time.unscaledDeltaTime;
        }
    }
    public void ScrollTop()
    {

        if (_scrollrect.verticalNormalizedPosition >= 0)
        {

            _scrollrect.verticalNormalizedPosition -= ScrollSpeed * Time.unscaledDeltaTime;

        }
    }



    private void Update()
    {

        if (ScrollButtonTop != null)
        {

            if (ScrollButtonTop.IsDown)
            {
                ScrollTop();
            }
        }
        if (ScrollButtonBottom != null)
        {

            if (ScrollButtonBottom.IsDown)
            {

                ScrollBottom();
            }

        }
    }
}
