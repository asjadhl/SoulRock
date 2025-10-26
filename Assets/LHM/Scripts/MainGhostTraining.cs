using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동에 필요

public class GhostTrainingLoader : MonoBehaviour
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
                    Debug.Log("오브젝트 클릭 감지됨");
                    MainGhostTrainingState.isClicked = true;
                    SceneManager.LoadScene("TraingRoom");
                }
            }
        }
    }
}
