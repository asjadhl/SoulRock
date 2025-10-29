using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    string currentSceneName;

    private void Awake()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void Retry()
    {
        SceneManager.LoadScene(currentSceneName);
    }

    public void GoMain()
    {
        SceneManager.LoadScene("Main");
    }
}
