using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;




public class FXSManager : MonoBehaviour
{
  [System.Serializable]
  public struct PreviousData
    {
       public AudioClip clip;
        public float audioTime;
        public double scheduledStart;
        public double pausedDSP;
        public bool isLoop;
    }
    [System.Serializable]
    public struct AudioData
    {
        public AudioClip clip;
        public bool isLoop;
    }
    public static FXSManager Instance;

  public AudioSource MusicSource;
  public AudioSource SfXSource;
  public bool isMute;
  public UnityEngine.UI.Slider MasterSlider;
  public UnityEngine.UI.Slider MusicSlider;
  public UnityEngine.UI.Slider SfXSlider;
  public List<AudioData> MusicList;
  [Space(2f)]
  public List<AudioData> SfXList;
  public float MasterVolume;
  public float MusicVolume;
  public float SfXVolume;
  [Space(2f)]
  public Animator anim;
   private Stack<string> m_Stack = new();
    public PreviousData previousMusicData;
    private PreviousData previousSfxData; 
    private Dictionary<int, List<AudioData>> dict;
  public int currentIndex;
  public float AnimationProgress;
   
  public TextMeshProUGUI textMeshPro;

 
  public void Awake()
  {
    if (Instance != null)
      Destroy(this.gameObject);
    else
    {
      Instance = this;
      DontDestroyOnLoad(Instance);
    }

  }

  public void Start()
  {

    MasterVolume = PlayerPrefs.GetFloat("MSR");
    MusicVolume = PlayerPrefs.GetFloat("MSC");
    SfXVolume = PlayerPrefs.GetFloat("SFX");

    MasterSlider.value = MasterVolume;
    MusicSlider.value = MusicVolume;
    SfXSlider.value = SfXVolume;
    dict = new();
    if (MusicList.Count > 0)
    {
      dict.Add(0, new());
      for (int i = 0; i < MusicList.Count; i++)
      {        
         dict[0].Add(MusicList[i]);
      }
    }

    if (SfXList.Count > 0)
    {
      dict.Add(1, new());
      for (int i = 0; i < SfXList.Count; i++)
      {
        dict[1].Add(SfXList[i]);
      }
    }

    anim.SetFloat("S", -1);

  }


 

  public void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape) && 0 >= m_Stack.Count)
    {
      anim.SetFloat("S", 1);
      m_Stack.Push("MainSettingShowUp");
      anim.Play(m_Stack.Peek(), 0, 0);
            DisableUnwantedThreat();
            SaveCurrentClip();
            StopPlay();
            //Freeze Time
            Time.timeScale = 0;
        }
    else if (Input.GetKeyDown(KeyCode.Escape) && 0 < m_Stack.Count)
    {
     
      anim.SetFloat("S", -1);

      if(m_Stack.Count == 1) // Exiting Main Setting
        {       
                EnableUnwantedThreat();

        if (MusicSource.clip != null)
        {
          MusicSource.clip = previousMusicData.clip;
          MusicSource.time = previousMusicData.audioTime;
          MusicSource.loop = previousMusicData.isLoop;
          MusicSource.PlayScheduled(AudioSettings.dspTime + 0.05);
        }

        if (SfXSource.clip != null)
        {
          SfXSource.clip = previousSfxData.clip;
          SfXSource.time = previousSfxData.audioTime;
          SfXSource.loop = previousSfxData.isLoop;
          SfXSource.PlayScheduled(AudioSettings.dspTime + 0.05);
        }    

               //Release Time
                Time.timeScale = 1;
            }
        if(m_Stack.Count == 2) //  Exiting Music Setting
            {
                isNextClip = false;
                StopPlay();
              
            }
      anim.Play(m_Stack.Pop(), 0, 1);
           
    }


  }

    void EnableUnwantedThreat()
    {
        var temp = GameObject.FindObjectsByType<MainGhostClick>(
     FindObjectsInactive.Include,
     FindObjectsSortMode.None
                              );

        if (temp.Length >= 1)
        {
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].enabled = true;
            }
        }

        var temp2 = GameObject.FindObjectsByType<GhostTrainingLoader>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
            );

        if (temp2.Length >= 1)
        {
            for (int i = 0; i < temp2.Length; i++)
            {
                temp2[i].enabled = true;
            }
        }

        var temp3 = GameObject.FindObjectsByType<MainQuitGhost>(
           FindObjectsInactive.Include,
           FindObjectsSortMode.None
           );

        if (temp3.Length >= 1)
        {
            for (int i = 0; i < temp3.Length; i++)
            {
                temp3[i].enabled = false;
            }
        }
    }
   void DisableUnwantedThreat()
    {
        
              var temp = GameObject.FindObjectsByType<MainGhostClick>(
         FindObjectsInactive.Include,
         FindObjectsSortMode.None
                                  );

        if (temp.Length >= 1)
        {
            
            for ( int i = 0; i < temp.Length; i++)
            {
                temp[i].enabled = false;
            }
        }

        var temp2 = GameObject.FindObjectsByType<GhostTrainingLoader>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
            );

        if (temp2.Length >= 1)
        {
            for (int i = 0; i < temp2.Length; i++)
            {
                temp2[i].enabled = false;
            }
        }


        var temp3 = GameObject.FindObjectsByType<MainQuitGhost>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
            );

        if (temp3.Length >= 1)
        {
            for (int i = 0; i < temp3.Length; i++)
            {
                temp3[i].enabled = false;
            }
        }
    }

    void SaveCurrentClip()
    {
    if (MusicSource.clip != null)
    {
      previousMusicData.clip = MusicSource.clip;
      previousMusicData.audioTime = MusicSource.time;
      previousMusicData.isLoop = MusicSource.loop;
    }

    if (SfXSource.clip != null)
    {
      previousSfxData.clip = SfXSource.clip;
      previousSfxData.audioTime = SfXSource.time;
      previousSfxData.isLoop = MusicSource.loop;
    }  
    }

    
  public void ShowFXSSetting() //bttn
  {
       
        anim.SetFloat("S", 1);
    m_Stack.Push("SoundsSettingShowUp");
    anim.Play(m_Stack.Peek(), 0, 0);
    isNextClip = true;



    }

  public void SetMasterVolume(float Op)
  {
    MasterVolume += Op;
    MasterVolume = Mathf.Clamp01(MasterVolume);
    MasterSlider.value = MasterVolume;
    MusicSource.volume = MasterVolume * MusicVolume;
    SfXSource.volume = MasterVolume * SfXVolume;
  }

  public void SetMusicVolume(float Op)
  {
    MusicVolume += Op;
    MusicVolume = Mathf.Clamp01(MusicVolume);
    MusicSlider.value = MusicVolume;
    MusicSource.volume = MasterVolume * MusicVolume;
  }
  public void SetSfXVolume(float Op)
  {
    SfXVolume += Op;
    SfXVolume = Mathf.Clamp01(SfXVolume);
    SfXSlider.value = SfXVolume;
    SfXSource.volume = MasterVolume * SfXVolume;
  }

  public void PlayClip(int key, int index)
  {

    if (!dict.ContainsKey(key))
      return;

    if (dict[key].Count <= index)
    { return; }

    switch (key)
    {
      case 0:
         MusicSource.clip = dict[key][index].clip;
        MusicSource.volume = MasterVolume * MusicVolume;
        MusicSource.loop = dict[key][index].isLoop;
        MusicSource.Play();
         
        break;
      case 1:
          SfXSource.clip = dict[key][index].clip;
          SfXSource.volume = MasterVolume * SfXVolume;
                SfXSource.loop = dict[key][index].isLoop;
                SfXSource.Play();
        break;
    }

  } 
    public void PlayClip(int key,AudioClip audioClip)
    {
          var index = dict[key].FindIndex(x => x.clip == audioClip);
           if(index != -1)
           PlayClip(key, index);


    }
    public void PlayClip(int key, AudioClip audioClip,double ScheduledStart)
    {
        var index = dict[key].FindIndex(x => x.clip == audioClip);
        if (index != -1)
            PlayClip(key, index);


    }
    public void PlayClip(int key, int index,double ScheduledStart)
  {
        
    if (!dict.ContainsKey(key))
      return;

    if (dict[key].Count <= index)
    { return; }

    switch (key)
    {
      case 0:   
                previousMusicData.scheduledStart = ScheduledStart;
                
        MusicSource.clip = dict[key][index].clip;
        MusicSource.volume = MasterVolume * MusicVolume;
        MusicSource.loop = dict[key][index].isLoop;
        MusicSource.PlayScheduled(previousMusicData.scheduledStart);
         
        break;
      case 1:
                previousSfxData.scheduledStart = ScheduledStart;
          SfXSource.clip = dict[key][index].clip;
          SfXSource.volume = MasterVolume * SfXVolume;
                SfXSource.loop = dict[key][index].isLoop;
                SfXSource.PlayScheduled(previousSfxData.scheduledStart);
        break;
    }

  }
  bool isNextClip = false;
  public void NextClip(int dir)
  {
    if (!isNextClip) return;

    //Queue Algorithm   Phython 
    currentIndex = (((currentIndex + dir) % dict[0].Count) + dict[0].Count) % dict[0].Count;
    PlayClip(0, currentIndex);
    textMeshPro.text = MusicSource.clip.name;
  }
  public void StopPlay()
  {
    MusicSource.Pause();
    SfXSource.Pause();
       
  }
   
   

  public void OnDestroy()
  {

    PlayerPrefs.SetFloat("MSR", MasterVolume);
    PlayerPrefs.SetFloat("MSC", MusicSource.volume);
    PlayerPrefs.SetFloat("SFX", SfXSource.volume);
  }
}


