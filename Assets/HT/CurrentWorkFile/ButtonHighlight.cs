using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Material mat;
    private Button myButton;
    private bool isPressed = false;
    private bool isHovered = false;

    float Deg = 0;
    float duration = 1.3f;

    void Awake()
    {
        myButton = GetComponent<Button>();

        // Get and clone the material so this button has its own instance
        mat = Instantiate(GetComponent<Image>().material);
        GetComponent<Image>().material = mat;

        // Hook into the button click
        myButton.onClick.AddListener(OnClick);
    }

    void Update()
    {
        if (isHovered && !isPressed)
        {
            SinUpdate();
        }
    }

    void SinUpdate()
    {
        Deg += (360f / duration) * Time.unscaledDeltaTime;
        if (Deg > 360f) Deg -= 360f;

        float rad = Mathf.Deg2Rad * Deg;

        float alpha = Mathf.Lerp(0.3f, 0.5f, (Mathf.Sin(rad) + 1f) * 0.5f);

        mat.SetFloat("_value", alpha);
    }

    void OnClick()
    {
        // Example: set shader property when clicked
        mat.SetFloat("_value", 1);
        isPressed = true;
       Wait(0.2f).Forget();
    }
   async UniTaskVoid Wait(float delay)
   {
    await UniTask.WaitForSeconds(delay);
    isPressed = false;
   }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isPressed)
            isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
            isPressed = false;
            isHovered = false;
        mat.SetFloat("_value", 0);
    }
}