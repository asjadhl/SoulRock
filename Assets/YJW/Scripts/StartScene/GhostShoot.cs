using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostShoot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject.tag == "start")
                {
                    SceneManager.LoadScene("Stage1");
                }
                if (hit.collider.gameObject.tag == "training")
                {
                    SceneManager.LoadScene("TrainingRoom");
                }
                if (hit.collider.gameObject.tag == "Quit")
                {
                    Application.Quit();
                }
            }
        }
    }
}
