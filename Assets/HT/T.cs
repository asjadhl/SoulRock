using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class T : MonoBehaviour
{



    [SerializeField]
    private float WidthSize;

    
    private float _WidthSize  
    {
        get { return WidthSize/2f ;}
        set { WidthSize = value; }
    }


    [SerializeField]
    private float HeightSize;


    private float _HeightSize
    {
        get { return HeightSize/2f;}
        set { HeightSize = value; }
    }

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
            //m_canvasScaler.matchWidthOrHeight = 0;
           // m_canvasScaler.referenceResolution = CanvasScalar;

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(new Vector2(-speed, 0));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Move(new Vector2(speed, 0));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(new Vector2(0, speed));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(new Vector2(0, -speed));
        }
    }

    void SetSize()
    {
        //rectTransform.anchorMin = new Vector2 (-(WidthSize / 2f)+Position.x, -(HeightSize / 2f) + Position.y);
        //rectTransform.anchorMax = new Vector2((WidthSize / 2f)+Position.x, (HeightSize / 2f)+Position.y);


        

        SetRenderer();
    }

    void SetRenderer()
    {

        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);

        
    }

    void Move(Vector2 _move)
    {

        Position += new Vector2(_move.x/m_canvasScaler.referenceResolution.x,_move.y / m_canvasScaler.referenceResolution.y);
        SetSize();
        SetRenderer();
    }
}
