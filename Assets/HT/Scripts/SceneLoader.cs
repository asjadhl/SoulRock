using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.Audio;
using Unity.VisualScripting;


public class SceneLoader : MonoBehaviour
{  
   
    public static SceneLoader Instance;

    private string targetScene;

    public GameObject Canvas;
    public Image Fill;
    public CancellationTokenSource cts;
    public bool isScene = false;
    public AudioMixer audiomixer;
    public float MusicSoundFadeDuration = 2f;
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


    private void FindAudioMixer()
    {
        var popsystem = FindAnyObjectByType<PopSystem>();
        if (popsystem != null)
        {
            audiomixer = popsystem.myMixer;
        }
    }
    private void Start()
    {
	    Canvas.SetActive(false);

        FindAudioMixer();

        if (PlayerPrefs.HasKey("MasterVolume"))
            realvolume = PlayerPrefs.GetFloat("MasterVolume");
        else
            realvolume = 1;
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
    
   

    float Tlerp = 0;
    float timer = 0;
     
    
    while (timer < 1f)
    {
      Tlerp+= Time.deltaTime/MusicSoundFadeDuration;
      timer += Time.deltaTime/3f;
      Fill.fillAmount = Mathf.Lerp(0, 1, timer);


      Lerping(Tlerp);
      await UniTask.Yield();
     
    }

    await UniTask.WaitForSeconds(2f);
    
    op.allowSceneActivation = true;
    while (!op.isDone)
    {
      await UniTask.Yield();

    }
    Canvas.SetActive(false);   
    isScene = false;
     
        if(audiomixer != null) 
    audiomixer.SetFloat("Master", realvolume);
        
        FindMusicBox();
  }

    private void OnApplicationQuit()
    {
        Debug.Log("Application is quitting!");
        PlayerPrefs.SetInt("IsTutorial", 0);
    }
    private void FindMusicBox()
    {
        var audiosources = FindObjectsByType<AudioSource>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);

        if(audiosources != null)
        {
            if (audiosources.Length > 0)
            {     
               for(int i=0;i<audiosources.Length;i++)
                {
                    if (audiosources[i].outputAudioMixerGroup != null) //Ignored if they have it
                               continue;

                    switch (audiosources[i].name)
                    {
                        case "MusicBox":
                            audiosources[i].outputAudioMixerGroup = audiomixer.FindMatchingGroups("Master")[0];//Music
                            break;
                        case "CircleHit":
                            audiosources[i].outputAudioMixerGroup = audiomixer.FindMatchingGroups("Master")[1];//SFX
                            break;
                        default:
                            Debug.Log($"audiosource.name:  {audiosources[i].name}");
                            break;
                    }
                }
               
                    
            }
        }
    }
   
}
