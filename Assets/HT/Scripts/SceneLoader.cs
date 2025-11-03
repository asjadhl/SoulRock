using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Threading;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private string targetScene;
  public GameObject Canvas;
  public RectTransform Fill;
  public CancellationTokenSource cts;
    //public string SceneName;
  public float m_Animationvalue;
  public float m_LeftValue;
  public float m_RightValue;
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



    private async void Start()
    {
		//await SceneLoader.Instance.LoadScene(SceneName);
		Canvas.SetActive(false);
	}

    public  UniTask LoadScene(string sceneName)
    {
        targetScene = sceneName;
       return  UniLoadSceneAsync();
    }

  public async UniTask RenderingFill(float progress)
  {

   
    if(cts != null)
    {
      cts.Cancel();
      cts.Dispose();
      cts = new();
    }
    m_LeftValue = m_Animationvalue;
    
    m_RightValue = Mathf.Clamp01((progress/0.9f));
    float timer = 0f;
   
    while (timer <= 1f)
    {
       
      timer += (Time.deltaTime/2f);
      m_Animationvalue = Mathf.Lerp(m_LeftValue,m_RightValue,timer);

      Fill.anchorMax = new Vector2(m_Animationvalue, Fill.anchorMax.y);
      Fill.offsetMin = Vector2.zero;
      Fill.offsetMax = Vector2.zero;
      await UniTask.Yield(cancellationToken: cts.Token);

       
    }

    
  }

  

  public  async UniTask UniLoadSceneAsync()
  {
    await UniTask.Yield();
        Canvas.SetActive(true);
        cts = new();
    AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
    op.allowSceneActivation = false;

   
    while(op.progress < 0.9f)
   {
      await  RenderingFill(op.progress);      
    }
    
    await RenderingFill(op.progress);
    

    op.allowSceneActivation = true; 
    while (!op.isDone)
    { 
      await UniTask.Yield();
 
    }
    Canvas.SetActive(false);

  }
    
   
}
