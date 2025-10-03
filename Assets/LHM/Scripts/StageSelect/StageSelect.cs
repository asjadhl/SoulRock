using UnityEngine;

public class StageSelect : MonoBehaviour
{
    Vector3 mousePos = Input.mousePosition;
    public GameObject stage1on;
    public GameObject stage1off;
    public GameObject stage2on;
    public GameObject stage2off;

    void Start()
    {
        stage1on.SetActive(false);
        stage1off.SetActive(true);

        stage2on.SetActive(false);
        stage2off.SetActive(true);
    }

    void Update()
    {
        MouseSelect();
    }
    void MouseSelect()
    {
        if (mousePos.x < -0.0001f)
        {
            stage1on.SetActive(true);
            stage1off.SetActive(false);
        }
        else if(mousePos.x > 0.0001)
        {
            stage2on.SetActive(true);
            stage2off.SetActive(false);
        }
    }
    
}
