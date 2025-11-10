using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{   


  public enum State
  {
     Pressed,Hovering,ReturningToHovering,ReturningToEnd,EnteringToHovering,Waiting,Held
  };
  [System.Serializable]
  public enum Direction
  {  

    West =0,North= 270,South=90,East=180,North_West=45,South_West=315,South_East=225,North_East=135
  }
  public float Rotation;
  public Direction MyDirection;
    private Material mat;
    
    public State mystate;
     private bool IsDown;
    float Deg = 0;
    float duration = 1.3f;
    float value = 0;
   float IntensityValue = 0;
     float sinevalue = 0;
  float Deg2 = 0;
  float EmissionDuration = 0.7f;

  void Awake()
    {
     
        

        
       mat = Instantiate(GetComponent<Image>().material);
       GetComponent<Image>().material = mat;
       mat.SetFloat("_Rotation", (float)MyDirection);
       mystate = State.Waiting;

     
             
         
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
            if(!IsDown)
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

      case State.Held:
        {
          mat.SetFloat("_value", value);
        }
        break;
        case State.Waiting:
        {
          Deg2 += (360f / EmissionDuration) * Time.unscaledDeltaTime;
          if (Deg2 > 360f) Deg2 -= 360f;

          float rad = Mathf.Deg2Rad * Deg2;
          IntensityValue = Mathf.Lerp(0.4f, 0.7f, (Mathf.Sin(rad) + 1f) * 0.5f);
          mat.SetFloat("_Colorvalue", IntensityValue);
        }
        break;
      }
    }
    
  
    public void OnPointerEnter(PointerEventData eventData)
    {

    mat.SetFloat("_Colorvalue", 1);
    mystate = State.EnteringToHovering;
    }

    public void OnPointerExit(PointerEventData eventData)
    {     

     mystate = State.ReturningToEnd;
           
    }

  public void OnPointerDown(PointerEventData eventData)
  {
    IsDown = true;
    mystate = State.Pressed;
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    IsDown = false;
  }


    public void SetColor(GameObject obj)
    {
        SafeGuard();
       
        var a = obj.GetComponent<Image>().color;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_Color",a*8f);
    }

    private void SafeGuard()
    {
        if (mat == null)
            mat = GetComponent<Image>().material;
    }

}