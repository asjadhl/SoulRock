using UnityEngine;

public class MainQuitGhost : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                
                if (hit.transform == transform)
                {
                    Debug.Log("게임 종료!");

#if UNITY_EDITOR
                    // 유니티 에디터에서는 플레이모드 종료
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    // 빌드된 게임에서는 실제 종료
                    Application.Quit();
#endif
                }
            }
        }
    }
}

