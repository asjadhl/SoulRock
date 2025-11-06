using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardButton : MonoBehaviour
{
    [SerializeField] private string keyValue; // ¿¹: "A", "B", "1", "!" µî
    private VirtualKeyboard keyboard;

    void Start()
    {
        keyboard = GetComponentInParent<VirtualKeyboard>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        keyboard.OnKeyPress(keyValue);
    }
}
