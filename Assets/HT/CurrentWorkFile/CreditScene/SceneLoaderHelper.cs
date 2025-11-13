using UnityEngine;

public class SceneLoaderHelper : MonoBehaviour
{
    public string SceneName;

    public void LoadScene()
    {
        if(SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(SceneName);
        }
    }
}
