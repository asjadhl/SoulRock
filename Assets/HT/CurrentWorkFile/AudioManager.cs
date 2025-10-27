using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
  [SerializeField] private AudioMixer myMixer;
  [SerializeField] private Slider masterSlider;
  [SerializeField] private Slider musicSlider;
  [SerializeField] private Slider SFXSlider;


  public Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();


  private Stack<string> AnimationStack = new Stack<string>();

   

  public GameObject MainPanel;
  public GameObject MusicSettingPanel;
  public GameObject LicsenseSettingPanel;
  public AudioSource OurAudioSource;
  private int currentIndex = -1;
  public List<AudioClip> ListTestMusicClip;
  public List<AudioClip> ListTestSFXClip;
  public Dictionary<int, List<AudioClip>> dictest;
  private int currenttestindex = 0;
  public AudioMixerGroup MusicGroup;
  public AudioMixerGroup SFXGroup;
  public TextMeshProUGUI textMeshPro;
  public bool isNextClip = false;
  public ScrollRect _scrollrect;
  public ScrollButton ScrollButtonTop;
  public ScrollButton ScrollButtonBottom;

  [Header("Animation-Shader")]
  public Canvas main_canvas;
  public Material mat1;
  private static readonly int UnscaledTimeID = Shader.PropertyToID("_UnscaledTime");
  public bool isSingle = false;
  public void OnClickSetMaterial(string paremeter)
  {
    if (mat1 != null)
    {
      isSingle = !isSingle;
      Debug.Log(isSingle);
      if (isSingle == true)
        mat1.SetFloat(paremeter, 1);
      else mat1.SetFloat(paremeter, 0);
    }
  }

  void UpdateTimeMaterial()
  {
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

  public void ScrollBottom()
  {
    if (_scrollrect.verticalNormalizedPosition <= 1)
    {

      _scrollrect.verticalNormalizedPosition += 0.001f;
    }
  }
  public void ScrollTop()
  {

    if (_scrollrect.verticalNormalizedPosition >= 0)
    {

      _scrollrect.verticalNormalizedPosition -= 0.001f;

    }
  }
  #region Animation


  public void InitializePanel()
  {
    panels.Add("AudioManager", MainPanel);
    panels.Add("MusicSetting", MusicSettingPanel);
    panels.Add("LicsenseSetting", LicsenseSettingPanel);
  }
  public void StopPlay()
  {
    OurAudioSource.Stop();
  }
   
  public void NextClip(int dir)
  {
    if (!isNextClip) return;

    //Queue Algorithm   Phython 
    currentIndex = (((currentIndex + dir) % dictest[currenttestindex].Count) + dictest[currenttestindex].Count) % dictest[currenttestindex].Count;
    OurAudioSource.clip = dictest[currenttestindex][currentIndex];
    OurAudioSource.Play();
    textMeshPro.text = dictest[currenttestindex][currentIndex].name;
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
  void EnableUnwantedThreat()
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
  void DisableUnwantedThreat()
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

    InitializePanel();
    
    OurAudioSource.ignoreListenerPause = true;


    InitTest();
    main_canvas.renderMode = RenderMode.ScreenSpaceCamera;
    main_canvas.worldCamera = Camera.main;
    main_canvas.planeDistance = 1;
  }

  public void Update()
  {


    if (ScrollButtonTop != null)
    {

      if (ScrollButtonTop.IsDown)
      {
        ScrollTop();
      }
    }
    if (ScrollButtonBottom != null)
    {

      if (ScrollButtonBottom.IsDown)
      {

        ScrollBottom();
      }

    }
    UpdateTimeMaterial();
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
    myMixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
  }

  public void SetMusicVolume(float dir) //slider
  {
    musicSlider.value += dir;
    myMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
  }

  public void SetSFXVolume(float dir) //slider
  {
    SFXSlider.value += dir;
    myMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);
  }



  private void LoadMasterVolume()
  {
    masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
    myMixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
  }
  private void LoadMusicVolume()
  {
    musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    myMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
  }
  private void LoadSFXVolume()
  {
    SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    myMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);
  }


}
