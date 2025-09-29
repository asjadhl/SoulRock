using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{

  public Transform pp;
   
    void Update()
    {

    if (Input.GetKeyDown(KeyCode.Space))
      UpdateClamp();
    }

  void UpdateClamp()
  {
    float width = Screen.width; // 100
    float height = Screen.height; //100
    //Clamp ScreenWorld
    Vector3 newresult = Camera.main.WorldToScreenPoint(pp.transform.position);
    float xclamp = Mathf.Clamp(newresult.x, width / 100f * 10, width / 100f * 90);

    
    float yclamp = Mathf.Clamp(newresult.y, height / 100f * 10, height / 100f * 70);
 

    newresult = new Vector3(xclamp, yclamp, newresult.z);

     
      pp.transform.position = Camera.main.ScreenToWorldPoint(newresult);
      Debug.Log($"pp.transform.position: {pp.transform.position}");
      Debug.Log($"newresult: {newresult}");
   
  }
}
