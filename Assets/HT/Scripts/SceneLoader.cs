using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private string targetScene;

    public GameObject Canvas;
    public Image Fill;
    public CancellationTokenSource cts;
    public bool isScene = false;
    public AudioMixer audiomixer;
    private float realvolume;
    private void Lerping(float t)
    {
        if (audiomixer == null)
            return;

        audiomixer.SetFloat("Master", Mathf.Log10(Mathf.Lerp(realvolume, 0.0001f, Mathf.Clamp01(t))) * 20);
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
        if (PlayerPrefs.HasKey("MasterVolume"))
            realvolume = (Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
        else
            realvolume = 0;
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
        float timer = 0;
    float duration = 3f;
    
    while (t < 1f)
    {
      t+= Time.deltaTime/duration;
      timer += Time.deltaTime/2f;
      Fill.fillAmount = Mathf.Lerp(0, 1, t);


      Lerping(timer);
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
    audiomixer.SetFloat("Master", realvolume);  
  }
    
   
}
