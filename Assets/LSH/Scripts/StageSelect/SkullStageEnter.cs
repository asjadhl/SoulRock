using UnityEngine;
using UnityEngine.SceneManagement;

public class SkullStageEnter : MonoBehaviour
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
                    LoadScene();
                }
            }
        }
    }
    private void LoadScene()
    {
        SceneLoader.Instance.LoadScene("stage3");
    }
}
