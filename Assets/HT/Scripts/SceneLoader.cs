using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private string targetScene;


    public string SceneName;
    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    private void Start()
    {
         SceneLoader.Instance.LoadScene(SceneName);
         
    }

    public void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene(targetScene);
    }

    
    public void LoadTargetScene()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        yield return null;  

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        op.allowSceneActivation = false;

        // Wait until scene is 90% loaded
        while (op.progress < 0.999f)
        {
            yield return null;
        }

        // Small wait before activating (for animation/fade)
        yield return new WaitForSeconds(0.5f);

        op.allowSceneActivation = true;
    }
}
