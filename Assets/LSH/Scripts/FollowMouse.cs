using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    RectTransform followPos;
    bool changeMode = false;
    Vector3 rememberPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        followPos = GetComponent<RectTransform>();
        rememberPos = followPos.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeMode();
        switch (changeMode)
        {
            case true:
                MousePos();
                break;
            case false:
                NormalMode();
                break;
        }
        
    }
    void ChangeMode()
    {
        if (Input.GetMouseButtonDown(1) && !changeMode)
        {
            Debug.Log("True");
            changeMode = true;
        }
        else if (Input.GetMouseButtonDown(1) && changeMode)
        {
            Debug.Log("False");
            changeMode = false;
        }
    }
    void MousePos()
    {
        //    Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    followPos.position = MousePos;
        Vector2 mousePos = Input.mousePosition;
        followPos.position = mousePos;
        followPos.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
    void NormalMode()
    {
        followPos.localScale = Vector3.one;
        followPos.localPosition = rememberPos;
    }
}
