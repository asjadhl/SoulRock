using System;
 

using UnityEngine;
using UnityEngine.UI;






interface IDamagable
{
   public void TakeHit(int _damage);
}
 
public enum UISpace
{
    ScreenSpaceUI,   // UI inside a Canvas set to Screen Space (Overlay/Camera)
    WorldSpaceUI     // UI on a World Space Canvas
}

public class Health: MonoBehaviour, IDamagable
{

    [Header("Health(0.4v)")]
    [Space(5)]
    [Header("Canvas")]
    [SerializeField] private GameObject m_Canvas;
    [SerializeField] private UISpace uiSpace;
    [Header("HIDE")]
    [SerializeField] private bool IsHide;
    [Header("Prefab")]
    [SerializeField] GameObject m_Prefab;

    #region Health-Properties
    [Space(5f)]
    [Header("HealthBar Inspector")]
    [SerializeField] private int m_MaxHealth = 100;
    [SerializeField] private int m_CurrentHealth = 100;

    [SerializeField] private Transform TargetParent;
    [SerializeField] private Vector3 CanvasPosition;
    [SerializeField] private GameObject m_healthBar;
    [SerializeField] private RectTransform m_fill;
    [Range(0f, 1f)]
    [SerializeField] float m_LeftValue;
    [SerializeField] float m_RightValue;
    [Range(0f, 1f)]
    [SerializeField] public float m_Animationvalue;

    [SerializeField] float m_time = 1;
    [SerializeField] float m_duration = 1.2f;

    

 

    #endregion


    




    



    #region Initialize
    public void Start()
    {
        
        switch (uiSpace)
        {
            case UISpace.ScreenSpaceUI:
                //if Canvas not existing find Canvas
                if(m_Canvas != null)
                m_healthBar = Instantiate(m_Prefab, m_Canvas.transform, false);
                else
                {
                    m_Canvas = GameObject.Find("Canvas");
                    if (m_Canvas == null)
                    {
                        m_Canvas = new GameObject("Canvas");
                        #region CreateCanvas.AddComponent
                        m_Canvas.AddComponent<Canvas>();
                        m_Canvas.AddComponent<CanvasScaler>();
                        m_Canvas.AddComponent<GraphicRaycaster>();
                        m_Canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                        #endregion
                    }


                    m_healthBar = Instantiate(m_Prefab, m_Canvas.transform, false);
                }
                break;
            case UISpace.WorldSpaceUI:
                #region CreateCanvas.AddComponent

                //Basic
                GameObject m_canvas = new GameObject("Canvas");
                m_canvas.transform.SetParent(TargetParent);
                m_canvas.AddComponent<Canvas>();
                m_canvas.AddComponent<CanvasScaler>();
                m_canvas.AddComponent<GraphicRaycaster>();
                #endregion

                #region ModifyCanvasComponent
                m_canvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
                RectTransform canvasrectransform = m_canvas.GetComponent<RectTransform>();
                m_canvas.GetComponent<Canvas>().worldCamera = Camera.main;
                canvasrectransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                canvasrectransform.localPosition = CanvasPosition;
                canvasrectransform.sizeDelta = new Vector3(847, 475,0);
                #endregion

                #region HealthBar On WorldSpace
                m_healthBar = Instantiate(m_Prefab, m_canvas.transform, false);
                RectTransform m_healthrect = m_healthBar.GetComponent<RectTransform>();
                //Set Anchor
                m_healthrect.anchorMin = new Vector2(0, 0.25f);
                m_healthrect.anchorMax = new Vector2(1, 0.75f);
                //Set healthbar's size

                m_healthrect.offsetMin = new Vector2(0, 0);
                m_healthrect.offsetMax = new Vector2(0, 0);
                #endregion

                break;
        }
       


       
        m_fill = m_healthBar.transform.GetChild(0).GetComponent<RectTransform>();

        m_LeftValue = ((float)m_CurrentHealth / (float)m_MaxHealth);
        m_RightValue = m_LeftValue;
        m_Animationvalue = m_LeftValue;

        m_fill.anchorMax = new Vector2(m_Animationvalue, m_fill.anchorMax.y);
        m_fill.offsetMin = new Vector2(0, 0);
        m_fill.offsetMax = new Vector2(0, 0);

        if(IsHide)
            m_healthBar.SetActive(false);
       
    }
    #endregion


    #region When_Hit
    public void TakeHit(int _damage)
    {
        if (m_CurrentHealth - _damage < 0) return;
        m_CurrentHealth -= _damage;
        float newDamage = (float)_damage / (float)m_MaxHealth;
        m_LeftValue -= newDamage;
        m_time = 0;
        m_RightValue = m_Animationvalue;

        if (m_CurrentHealth <= 0)
        {
            IDying dying = gameObject.GetComponent<IDying>();
            dying?.PlayDyingAnimation(true);

        }
    }
    #endregion

    #region Fill-Animation
    public void HealthBarAnimation()
    {


        if (m_time >= 1f)
        { return; }
        else
        {
            m_time += Time.deltaTime / m_duration;
            m_Animationvalue = Mathf.Lerp(m_RightValue, m_LeftValue, m_time);

            m_fill.anchorMax = new Vector2(m_Animationvalue, m_fill.anchorMax.y);
            m_fill.offsetMin = new Vector2(0, 0);
            m_fill.offsetMax = new Vector2(0, 0);
        }


    }
    #endregion
    public void Update()
    {

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("??");
            IDying dying = gameObject.GetComponent<IDying>();
            if (dying != null)
                Debug.Log("A");
            dying?.PlayDyingAnimation(true);
        }
      
        HealthBarAnimation();
    }
}


