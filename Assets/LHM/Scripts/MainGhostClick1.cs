using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGhostClick1 : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시 검사
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 클릭된 오브젝트가 이 Ghost라면
                if (hit.transform == transform)
                {
                    Debug.Log("Ghost clicked!");

                    SceneManager.LoadScene("Main");
                }
            }
        }
    }
}

