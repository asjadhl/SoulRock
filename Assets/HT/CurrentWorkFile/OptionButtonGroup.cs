using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class OptionButtonGroup : MonoBehaviour
{

 

  [Header("Assign your option buttons here")]
    public List<Button> buttons;
  [Header("Assign your option colors here")]
  public List<Color> colors;
    [Header("Shader Property")]
    public string highlightProperty = "_Value"; // float 0..1 in your shader

  [Header("Shader GlowColorProperty")]
  public string highlightGlowColorProperty = "_GlowColor";

    [Header("Hover Animation")]
    public float duration = 1.3f;   // seconds per pulse cycle

    private OptionButton currentSelected;
    private readonly List<OptionButton> optionButtons = new List<OptionButton>();

    void Awake()
    {
        optionButtons.Clear();
         
       for(int i=0;i< buttons.Count;i++)
       {
           if(buttons[i] == null) continue;
           optionButtons.Add(new OptionButton(buttons[i],this, highlightProperty, i < colors.Count ? colors[i] : Color.black, highlightGlowColorProperty, duration));
       }
     SetSelected(optionButtons[1]);
  }

    void Update()
    {
        // Per-button hover pulse & safety rebind
        foreach (var opt in optionButtons)
            opt.Tick();
    }

    // ★ private 로 변경: 매개변수 타입과 접근성 일치
    private void SetSelected(OptionButton newSelected)
    {
        if (currentSelected != null && currentSelected != newSelected)
            currentSelected.Deselect();

        currentSelected = newSelected;
        currentSelected.Select();
    }

   
    // ========================
    // Inner non-MonoBehaviour
    // ========================
    private class OptionButton
    {
        private readonly Button button;
        private readonly Image image;
        private Material mat;
        private Color glowcolor;
        private readonly OptionButtonGroup group;
        private readonly string prop;
        private readonly string glowcolorprop;
        private readonly float duration;

        private bool hovered;
        private bool selected;
        private float deg;

        public OptionButton(Button button, OptionButtonGroup group, string shaderFloatProperty,Color glowcolor, string shaderColorProperty, float duration)
        {
            this.button = button;
            this.group = group;
            this.prop = shaderFloatProperty;
            this.glowcolor = glowcolor;
            this.glowcolorprop = shaderColorProperty;
            this.duration = duration;

            image = button.GetComponent<Image>();

            // 머티리얼 개별 복제 & 할당
            EnsureMaterial();

            // 클릭 이벤트
            button.onClick.AddListener(OnClick);

            // Hover 이벤트
            var trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
            AddEvent(trigger, EventTriggerType.PointerEnter, _ =>
            {
                if (!selected) hovered = true;
            });
            AddEvent(trigger, EventTriggerType.PointerUp, _ =>
            {
              hovered = false;
              if (!selected) Set(0f);

            });
            AddEvent(trigger, EventTriggerType.PointerExit, _ =>
            {
                hovered = false;
                if (!selected) Set(0f);
            });

          
           mat.SetColor(this.glowcolorprop,this.glowcolor*25f);
        }

        public void Tick()
        {
            // 비활성→활성 전환 등으로 Unity가 material을 바꿨다면 재할당
            if (image.material != mat) image.material = mat;

            if (hovered && !selected)
            {
                deg += (360f / duration) * Time.unscaledDeltaTime;
                if (deg > 360f) deg -= 360f;

                float a = Mathf.Lerp(0.1f, 0.9f, (Mathf.Sin(deg * Mathf.Deg2Rad) + 1f) * 0.5f);
                Set(a);
            }
        }

        public void Select()
        {
            selected = true;
            hovered = false;
            
            Set(1f);
        }

        public void Deselect()
        {
            selected = false;
            hovered = false;
            deg = 0f;
            Set(0f);
        }

        private void OnClick()
        {
      if (this.button.name == group.optionButtons[0].button.name)
        return;
            
            // 그룹의 private SetSelected 호출 (중첩 클래스이므로 접근 가능)
            group.SetSelected(this);
             
        }

        private void EnsureMaterial()
        {
            var current = image.material;
            mat = Object.Instantiate(current);
            image.material = mat;
            Set(0f);
        }

        private void Set(float v)
        {
            if (mat != null)   mat.SetFloat(prop, v);    
        }

        private static void AddEvent(EventTrigger trigger, EventTriggerType type, System.Action<BaseEventData> cb)
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(_ => cb(_));
            trigger.triggers.Add(entry);
        }
    }
}
