using TMPro;
using UnityEngine;

public class VirtualKeyboard : MonoBehaviour
{
    [SerializeField] private TMP_InputField targetInput;

    public void OnKeyPress(string key)
    {
        if (targetInput == null) return;

        switch (key)
        {
            case "Backspace":
                if (targetInput.text.Length > 0)
                    targetInput.text = targetInput.text.Substring(0, targetInput.text.Length - 1);
                break;

            case "Enter":
                Debug.Log("엔터 입력됨! 현재 입력값: " + targetInput.text);
                // 예: 입력창 비활성화
                // gameObject.SetActive(false);
                break;

            default:
                targetInput.text += key;
                break;
        }

        targetInput.caretPosition = targetInput.text.Length; // 커서 유지
    }

    public void SetTarget(TMP_InputField input)
    {
        targetInput = input;
    }
}