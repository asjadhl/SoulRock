using UnityEngine;
using UnityEngine.SceneManagement;

public class ClownStageEnter : MonoBehaviour
{
    private Camera mainCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
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
                if (hit.transform.CompareTag("Stage2"))
                {
                    LoadScene();
                }
            }
        }
    }
    private async void LoadScene()
    {
        SceneLoader.Instance.LoadScene("Stage2");
    }
}
