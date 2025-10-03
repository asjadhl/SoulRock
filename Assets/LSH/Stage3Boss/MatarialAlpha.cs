using UnityEngine;
using UnityEngine.UIElements;

public class MatarialAlpha : MonoBehaviour
{
    private Renderer rend;
    bool changeColor = false;
    float rotate = 30;
    public bool successMirror = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!changeColor)
        {
            ChageCollor();
        }
        SuccessMirror();

    }

    void SuccessMirror()
    {
        float rotX = transform.eulerAngles.x % 360f;

        if (Mathf.Abs(rotX - 0f) <= 1f || Mathf.Abs(rotX - 180f) <= 1f)
        {
            successMirror = true;
        }
        else
        {
            successMirror  = false;
        }
    }
    void ChageCollor()
    {
        changeColor = true;
        Color c = rend.material.GetColor("_BaseColor");
        if (successMirror)
        {
            c.a = 1f;
            rend.material.SetColor("_BaseColor", c);
        }
        else
        {
            c.a = 0.3f;
            rend.material.SetColor("_BaseColor", c);
        }
        changeColor = false;
    }

    public void mirrorRotate()
    {
        transform.Rotate(-rotate, 0, 0);
    }

}
