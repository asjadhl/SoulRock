 
using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;
 
using UnityEngine.Audio;
using UnityEngine.UI;
 
public class AudioManager : MonoBehaviour
{

    [System.Serializable]
    public enum ModeType
    {
        Master,Music,SFX
    }



  [SerializeField] private AudioMixer myMixer;
  [SerializeField] private Slider masterSlider;
  [SerializeField] private Slider musicSlider;
  [SerializeField] private Slider SFXSlider;
  private Image masterImage;
  private Image musicImage;
  private Image SFXImage;
  private System.Func<float, Color> Painter = (value) => { return Color.HSVToRGB(Mathf.Lerp(0, 0.83f, value),1f,1f); };  





  public AudioSource OurAudioSource;
  private int currentIndex = -1;
  public List<AudioClip> ListTestMusicClip;
  public List<AudioClip> ListTestSFXClip;
  public Dictionary<int, List<AudioClip>> dictest;
  private int currenttestindex = 0;
  public AudioMixerGroup MusicGroup;
  public AudioMixerGroup SFXGroup;
  
 

  [Header("Animation-Shader")]
  public Canvas main_canvas;
  public Image image;
  public Material mat1;
  private static readonly int UnscaledTimeID = Shader.PropertyToID("_UnscaledTime");
  public bool isSingle = false;
  public void OnClickSetMaterial(string paremeter)
  {

    if (mat1 != image.material) mat1 = image.material;

    AudioListener.pause = true;
        if (mat1 != null)
        {
      isSingle = !isSingle;
    
      if (isSingle == true)
        mat1.SetFloat(paremeter, 1);
      else mat1.SetFloat(paremeter, 0);
    }
  }

    public void StopAllAudio()
    {
        AudioListener.pause = true;
    }
    public void PlayAllAudio()
    {
        AudioListener.pause = false;
    }
  void UpdateTimeMaterial()
  {
    if (mat1 != image.material) mat1 = image.material;  

    if (mat1 != null)
    {
      mat1.SetFloat(UnscaledTimeID, Time.unscaledTime);
    }
  }
  public void InitTest()
  {
    dictest = new();
    if (ListTestMusicClip != null)
    {
      dictest.Add(0, new List<AudioClip>());
      for (int i = 0; i < ListTestMusicClip.Count; i++)
      {
        dictest[0].Add(ListTestMusicClip[i]);
      }
    }

    if (ListTestSFXClip != null)
    {
      dictest.Add(1, new List<AudioClip>());
      for (int i = 0; i < ListTestSFXClip.Count; i++)
      {
        dictest[1].Add(ListTestSFXClip[i]);
      }
    }
  }

 
  #region Animation


 
  public void StopPlay()
  {
    OurAudioSource.Stop();
  }
   
  public void NextClip(int dir)
  {
   

    //Queue Algorithm   Phython 
    currentIndex = (((currentIndex + dir) % dictest[currenttestindex].Count) + dictest[currenttestindex].Count) % dictest[currenttestindex].Count;
    OurAudioSource.clip = dictest[currenttestindex][currentIndex];
    OurAudioSource.Play();
     
  }
  public void ChangeSoundType() // -button-
  {
    currenttestindex += 1;
    if (currenttestindex >= dictest.Count)
    {
      currenttestindex = 0;
    }

    if (currenttestindex == 0)
    {
      OurAudioSource.outputAudioMixerGroup = MusicGroup;
    }
    else if (currenttestindex == 1)
    {
      OurAudioSource.outputAudioMixerGroup = SFXGroup;
    }
    NextClip(0);
  }
  public void EnableUnwantedThreat()
  {
    var mainghostclick = GameObject.FindObjectsByType<MainGhostClick>(
 FindObjectsInactive.Include,
 FindObjectsSortMode.None
                          );

    if (mainghostclick.Length >= 1)
    {
      for (int i = 0; i < mainghostclick.Length; i++)
      {
        mainghostclick[i].enabled = true;
      }
    }

    var ghosttrainingloader = GameObject.FindObjectsByType<GhostTrainingLoader>(
        FindObjectsInactive.Include,
        FindObjectsSortMode.None
        );

    if (ghosttrainingloader.Length >= 1)
    {
      for (int i = 0; i < ghosttrainingloader.Length; i++)
      {
        ghosttrainingloader[i].enabled = true;
      }
    }

    var MainQuitGhost = GameObject.FindObjectsByType<MainQuitGhost>(
       FindObjectsInactive.Include,
       FindObjectsSortMode.None
       );

    if (MainQuitGhost.Length >= 1)
    {
      for (int i = 0; i < MainQuitGhost.Length; i++)
      {
        MainQuitGhost[i].enabled = false;
      }
    }
  }
 public void DisableUnwantedThreat()
  {

    var mainghostclick = GameObject.FindObjectsByType<MainGhostClick>(
     FindObjectsInactive.Include,
     FindObjectsSortMode.None
                        );

    if (mainghostclick.Length >= 1)
    {

      for (int i = 0; i < mainghostclick.Length; i++)
      {
        mainghostclick[i].enabled = false;
      }
    }

    var ghosttrainingloader = GameObject.FindObjectsByType<GhostTrainingLoader>(
        FindObjectsInactive.Include,
        FindObjectsSortMode.None
        );

    if (ghosttrainingloader.Length >= 1)
    {
      for (int i = 0; i < ghosttrainingloader.Length; i++)
      {
        ghosttrainingloader[i].enabled = false;
      }
    }


    var MainQuitGhost = GameObject.FindObjectsByType<MainQuitGhost>(
        FindObjectsInactive.Include,
        FindObjectsSortMode.None
        );

    if (MainQuitGhost.Length >= 1)
    {
      for (int i = 0; i < MainQuitGhost.Length; i++)
      {
        MainQuitGhost[i].enabled = false;
      }
    }
  }
  #endregion


  public AudioManager instance;
  public void Start()
  {



    masterSlider.minValue = 0.0001f;
    musicSlider.minValue = 0.0001f;
    SFXSlider.minValue = 0.0001f;
    masterImage = masterSlider.fillRect.GetComponent<Image>();
    musicImage = musicSlider.fillRect.GetComponent<Image>();
    SFXImage = SFXSlider.fillRect.GetComponent<Image>();
     
    if (PlayerPrefs.HasKey("MasterVolume"))
    {
      LoadMasterVolume();
    }
    else
    {
      SetMasterVolume(0);
    }

    if (PlayerPrefs.HasKey("MusicVolume"))
    {
      LoadMusicVolume();
    }
    else
    {
      SetMusicVolume(0);
    }

    if (PlayerPrefs.HasKey("SFXVolume"))
    {
      LoadSFXVolume();
    }
    else
    {
      SetSFXVolume(0);
    }

   
    OurAudioSource.ignoreListenerPause = true;


    InitTest();
    main_canvas.renderMode = RenderMode.ScreenSpaceCamera;
    main_canvas.worldCamera = Camera.main;
    main_canvas.planeDistance = 1;

       //Initialize
      var child =  main_canvas.transform.Find("Panel");
        if (child != null)
        {
          child.gameObject.SetActive(false);
          child = child.Find("Licsense_Setting");
        if(child != null)
           child.gameObject.SetActive(false);
        }

 
  }

  public void Update()
  {
 
    UpdateTimeMaterial();

        if (Input.GetKeyDown(KeyCode.Q))
        {

            Application.Quit();
 
        }
  }

  public void OnDestroy()
  {
    PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
  }
   
  public void SetMasterVolume(float dir) //slider
  {
       
        masterSlider.value += dir;
        masterImage.color = Painter(masterSlider.value);
         masterSlider.onValueChanged.Invoke(0);
         masterSlider.onValueChanged.Invoke(1);
        myMixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
  }

     

  public void SetMusicVolume(float dir) //slider
  {
    musicSlider.value += dir;
    musicImage.color = Painter(musicSlider.value);
        musicSlider.onValueChanged.Invoke(0);
        musicSlider.onValueChanged.Invoke(1);
        myMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
  }

  public void SetSFXVolume(float dir) //slider
  {
        SFXSlider.value += dir;
        SFXImage.color = Painter(SFXSlider.value);
        SFXSlider.onValueChanged.Invoke(0);
        SFXSlider.onValueChanged.Invoke(1);
        myMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);
  }

    

  private void LoadMasterVolume()
  {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        masterImage.color = Painter(masterSlider.value);
        masterSlider.onValueChanged.Invoke(0);
        masterSlider.onValueChanged.Invoke(1);
        myMixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
  }
  private void LoadMusicVolume()
  {
    musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    musicImage.color = Painter(musicSlider.value);
        musicSlider.onValueChanged.Invoke(0);
        musicSlider.onValueChanged.Invoke(1);
        myMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
  }
  private void LoadSFXVolume()
  {
    SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    SFXImage.color = Painter(SFXSlider.value);
        SFXSlider.onValueChanged.Invoke(0);
        SFXSlider.onValueChanged.Invoke(1);
        myMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);
  }


}
