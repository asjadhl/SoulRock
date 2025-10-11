using UnityEngine;

public class CanvasCancel : MonoBehaviour
{
    [Header("닫힐 Canvas")]
    public GameObject targetCanvas;

    // 버튼 OnClick()에서 이 함수 연결
    public void CloseCanvas()
    {
        if (targetCanvas != null)
        {
            Debug.Log("Close");
            targetCanvas.SetActive(false);
        }
    }
}