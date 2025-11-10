using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    RectTransform followPos;
    bool changeMode = false;
    Vector3 rememberPos;
    void Start()
    {
        followPos = GetComponent<RectTransform>();
    }

    void Update()
    {
        MousePos();

	}
    void MousePos()
    {
        Vector2 mousePos = Input.mousePosition;
        followPos.position = mousePos;
    }
}
