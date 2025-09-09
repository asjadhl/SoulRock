using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class T : MonoBehaviour
{



    [SerializeField]
    private float WidthSize;

    
  


    [SerializeField]
    private float HeightSize;


 

    [SerializeField]
    private Vector2 Position;
    public float speed;

    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    private CanvasScaler m_canvasScaler;

    [SerializeField]
    private Vector2 CanvasScalar;

    private void Awake()
    {   
        if (rectTransform == null)
            gameObject.SetActive(false);

        FindCanvasScalar();

        SetSize();
       
       
    }


    void FindCanvasScalar()
    {
        m_canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();

        if (m_canvasScaler != null)
        {
             m_canvasScaler.matchWidthOrHeight = 0;
             m_canvasScaler.referenceResolution = CanvasScalar;

        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Move(new Vector2(-speed*Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            Move(new Vector2(speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.W))
        {
            Move(new Vector2(0, speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.S))
        {
            Move(new Vector2(0, -speed * Time.deltaTime));
        }
    }

    void SetSize()
    {
        //rectTransform.anchorMin = new Vector2 (-(WidthSize / 2f)+Position.x, -(HeightSize / 2f) + Position.y);
        //rectTransform.anchorMax = new Vector2((WidthSize / 2f)+Position.x, (HeightSize / 2f)+Position.y);

        Vector2 half = new Vector2(WidthSize / 2f, HeightSize / 2f);

        rectTransform.anchorMin = new Vector2((Position.x- half.x) / m_canvasScaler.referenceResolution.x, ( Position.y-half.y) / m_canvasScaler.referenceResolution.y);
        rectTransform.anchorMax = new Vector2((Position.x+half.x) / m_canvasScaler.referenceResolution.x, (Position.y+half.y)/ m_canvasScaler.referenceResolution.y);


        // pos 2,2
        // size 7,7
        // half   3.5,3.5
        // max = 2+3.5,2+3.5
        // min = 2-3.5,2-3.5
        SetRenderer();
    }

    void SetRenderer()
    {

        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);

        
    }

    void Move(Vector2 _move)
    {

        Position += new Vector2(_move.x,_move.y);
        SetSize();
        SetRenderer();
    }
}
