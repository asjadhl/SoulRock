using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{   


  public enum State
  {
     Pressed,Hovering,ReturningToHovering,ReturningToEnd,EnteringToHovering,Waiting
  };

  public float Rotation;
    private Material mat;
    private Button myButton;
    public State mystate;
 
    float Deg = 0;
    float duration = 1.3f;
    float value = 0;
   
     float sinevalue = 0;
  
 
    void Awake()
    {
     
        myButton = GetComponent<Button>();

        // Get and clone the material so this button has its own instance
        mat = Instantiate(GetComponent<Image>().material);
        GetComponent<Image>().material = mat;
    mat.SetFloat("_Rotation", Rotation);
    mystate = State.Waiting;
        // Hook into the button click
        myButton.onClick.AddListener(OnClick);
    }

    void Update()
    {
    StateUpdate();
      
    }

    void SinUpdate()
    {
        Deg += (360f / duration) * Time.unscaledDeltaTime;
        if (Deg > 360f) Deg -= 360f;

        float rad = Mathf.Deg2Rad * Deg;

       value = Mathf.Lerp(0.3f, 0.5f, (Mathf.Sin(rad) + 1f) * 0.5f);
          
    

        mat.SetFloat("_value", value);
    }
     

    void StateUpdate()
    {
      switch(mystate)
      { 
        case State.EnteringToHovering:
        {
          Deg += (360f / duration) * Time.unscaledDeltaTime;
          if (Deg > 360f) Deg -= 360f;

          float rad = Mathf.Deg2Rad * Deg;
          sinevalue = Mathf.Lerp(0.3f, 0.5f, (Mathf.Sin(rad) + 1f) * 0.5f);

          if (Mathf.Abs(value - sinevalue) > 0.01f)
            value = Mathf.MoveTowards(value, sinevalue, 1 * Time.unscaledDeltaTime);
          else
            mystate = State.Hovering;

          mat.SetFloat("_value", value);
        }
        break;
        case State.Hovering:
        {
          Deg += (360f / duration) * Time.unscaledDeltaTime;
          if (Deg > 360f) Deg -= 360f;

          float rad = Mathf.Deg2Rad * Deg;
          sinevalue = Mathf.Lerp(0.3f, 0.5f, (Mathf.Sin(rad) + 1f) * 0.5f);
          value = sinevalue;

          mat.SetFloat("_value", value);
        }
        break;
        case State.Pressed:
        {
          value = Mathf.MoveTowards(value, 1, 5f * Time.unscaledDeltaTime);
          mat.SetFloat("_value", value);

          if(Mathf.Abs(value-1) < 0.01f)
          {
            value = 1;
            mystate = State.ReturningToHovering;
          }
        }
        break;
      case State.ReturningToHovering:
        {
          Deg += (360f / duration) * Time.unscaledDeltaTime;
          if (Deg > 360f) Deg -= 360f;

          float rad = Mathf.Deg2Rad * Deg;
          sinevalue = Mathf.Lerp(0.3f, 0.5f, (Mathf.Sin(rad) + 1f) * 0.5f);

          if (Mathf.Abs(value - sinevalue) > 0.01f)
          {
            value = Mathf.MoveTowards(value, sinevalue, 1 * Time.unscaledDeltaTime);
          }
          else
            mystate = State.Hovering;

          mat.SetFloat("_value", value);
        }
        break;
      case State.ReturningToEnd:
        {
          if (Mathf.Abs(value - 0) > 0.01f)
            value = Mathf.MoveTowards(value,0,1*Time.unscaledDeltaTime);
          else
          {
            value = 0;
            mystate = State.Waiting;
          }
          mat.SetFloat("_value", value);
        }
        break;
        case State.Waiting:
        {
          // NULL
        }
        break;
      }
    }
    void OnClick()
    {
   
    mystate = State.Pressed;
  
    }
  
    public void OnPointerEnter(PointerEventData eventData)
    {
    

    mystate = State.EnteringToHovering;
    }

    public void OnPointerExit(PointerEventData eventData)
    {     

     mystate = State.ReturningToEnd;
           
    }
}