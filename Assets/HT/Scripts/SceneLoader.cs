using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using Unity.VisualScripting;
using static CartoonFX.CFXR_ParticleTextFontAsset;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private string targetScene;

    public GameObject Canvas;
    public Image Fill;
    public CancellationTokenSource cts;
    public bool isScene = false;
    private System.Lazy<AudioSource[]> audiosources = new System.Lazy<AudioSource[]> (() =>
    {


        var find = GameObject.FindObjectsByType<AudioSource>(
        FindObjectsInactive.Exclude,
        FindObjectsSortMode.None
        );
        return find;
         
    });

    private void Lerping(float t)
    {
        for (int i = 0; i < audiosources.Value.Length; i++)
        {
            audiosources.Value[i].volume = Mathf.Lerp(1, 0, t);
        }
    }
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






        
        




























	    Canvas.SetActive(false);
	}

    public void LoadScene(string sceneName)
    {
        if (!isScene)
        {
            targetScene = sceneName;
            UniLoadSceneAsync().Forget();
        }
        else
            return;
            
    }

  public async UniTask UniLoadSceneAsync()
  { 
        isScene = true;
     Canvas.SetActive(true);
    
        cts = new();
    AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
    op.allowSceneActivation = false;

   
    while(op.progress < 0.9f)
    {
      Fill.fillAmount = (op.progress / 0.9f);
    }
    
   

    float t = 0;
    float duration = 3f;

    while (t < 1f)
    {
      t+= Time.deltaTime/duration;
      Fill.fillAmount = Mathf.Lerp(0, 1, t);


            //Lerping(t);
      await UniTask.Yield();
      Debug.Log(t);
    }

    await UniTask.WaitForSeconds(2f);
    
    op.allowSceneActivation = true;
    while (!op.isDone)
    {
      await UniTask.Yield();

    }
    Canvas.SetActive(false);   
    isScene = false;
  }
    
   
}
