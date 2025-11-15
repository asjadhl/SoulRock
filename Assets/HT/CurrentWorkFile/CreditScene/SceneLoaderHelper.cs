using UnityEngine;

public class SceneLoaderHelper : MonoBehaviour
{
    public string SceneName;

    [SerializeField] private GameObject scorePanel;

    public void LoadScene()
    {
            scorePanel.SetActive(true);

        
    }

    public void LoadsCene()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(SceneName);
        }
    }
}

