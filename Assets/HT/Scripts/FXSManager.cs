using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class FXSManager : MonoBehaviour
{
    [System.Serializable]
    public struct PreviousData
    {
        public AudioClip clip;
        public float audioTime;
        public double scheduledStart;
        public double pausedDSP;
        //public bool isLoop;
    }
    [System.Serializable]
    public struct AudioData
    {
        public AudioClip clip;
        //public bool isLoop;
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
    public PreviousData previousSfxData;
    private Dictionary<int, List<AudioData>> dict;
    public int currentIndex;
    public float AnimationProgress;

    public TextMeshProUGUI textMeshPro;

    public ScrollRect _scrollrect;
    public ScrollButton ScrollButtonTop;
    public ScrollButton ScrollButtonBottom;
    public void ScrollTop()
    {
        if (_scrollrect.verticalNormalizedPosition <= 1)
        {
            _scrollrect.verticalNormalizedPosition += 0.01f;
        }
    }
    public void ScrollBottom()
    {
       
        if (_scrollrect.verticalNormalizedPosition >= 0)
        {
            Debug.Log("Down");
            _scrollrect.verticalNormalizedPosition -= 0.01f;
            Debug.Log($"_scrollrect.verticalNormalizedPosition: {_scrollrect.verticalNormalizedPosition}");
        }
    }

    public void UndoOrShowSetting()
    {
        if (0 >= m_Stack.Count)
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
        else if (0 < m_Stack.Count)
        {
            anim.SetFloat("S", -1);

            if (m_Stack.Count == 1) // Exiting Main Setting
            {
                EnableUnwantedThreat();

                double scheduledTime = AudioSettings.dspTime + 0.05f;

                if (MusicSource.clip != null)
                {

                   
                    MusicSource.clip = previousMusicData.clip;
                    MusicSource.time = previousMusicData.audioTime;
                    if (!SafePlayScheduled(MusicSource, scheduledTime))  // Error CounterMeasure
                    {
                        scheduledTime = AudioSettings.dspTime + 0.05f;
                        SafePlayScheduled(MusicSource, scheduledTime);
                    }
                }
            
                if (SfXSource.clip != null)
                {
                    SfXSource.clip = previousSfxData.clip;
                    SfXSource.time = previousSfxData.audioTime;
                    


                    if (!SafePlayScheduled(SfXSource, scheduledTime)) // Error CounterMeasure
                    {
                        scheduledTime = AudioSettings.dspTime + 0.05f;
                        SafePlayScheduled(SfXSource, scheduledTime);
                    }
                }

                //Release Time
                Time.timeScale = 1;
            }
            else if (m_Stack.Count == 2) //  Exiting Music Setting  or Licsense Showcase
            {
                if (m_Stack.Peek() == "SoundsSettingShowUp")
                {
                    isNextClip = false;
                    StopPlay();
                }
                else if(m_Stack.Peek() == "LicenseSettingShowUp")
                {
                    //Nothing Yet
                }
            }
            anim.Play(m_Stack.Pop(), 0, 1);
        }
    }

    public void ShowMusicSetting() //Bttn;
    {
        anim.SetFloat("S", 1);
        m_Stack.Push("SoundsSettingShowUp");
        anim.Play(m_Stack.Peek(), 0, 0);
        isNextClip = true;

    }
    public void ShowLiscense() //Bttn;
    {
        anim.SetFloat("S", 1);
        m_Stack.Push("LicenseSettingShowUp");
        anim.Play(m_Stack.Peek(), 0, 0);
    }

    public void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

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

            anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetFloat("S", -1);
                anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
        }



    }

    

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
       // StopPlay();
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

    }

  //Helper
    private bool SafePlayScheduled(AudioSource source, double time)
    {
        if (source == null) return false;
        if (double.IsNaN(time) || double.IsInfinity(time)) return false;
        if (time < AudioSettings.dspTime) return false;

        source.PlayScheduled(time);
        return true;
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

    void SaveCurrentClip()
    {
        if (MusicSource.clip != null)
        {
            previousMusicData.clip = MusicSource.clip;
            previousMusicData.audioTime = MusicSource.time;
            //previousMusicData.isLoop = MusicSource.loop;
        }

        if (SfXSource.clip != null)
        {
            previousSfxData.clip = SfXSource.clip;
            previousSfxData.audioTime = SfXSource.time;
            //previousSfxData.isLoop = MusicSource.loop;
        }
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
        if (dict == null)
            Debug.Log("Dic null");

        if (!dict.ContainsKey(key))
            return;

        if (dict[key].Count <= index)
        { return; }

        switch (key)
        {
            case 0:
                Debug.Log($"dict[key][index].clip: {dict[key][index].clip.name}");
                MusicSource.clip = dict[key][index].clip;
                MusicSource.volume = MasterVolume * MusicVolume;
                //MusicSource.loop = dict[key][index].isLoop;
                MusicSource.Play();

                break;
            case 1:
                SfXSource.clip = dict[key][index].clip;
                SfXSource.volume = MasterVolume * SfXVolume;
                //SfXSource.loop = dict[key][index].isLoop;
                SfXSource.Play();
                break;
        }

    }
    public void PlayClip(int key, AudioClip audioClip) // override
    {
        var index = dict[key].FindIndex(x => x.clip == audioClip);
        if (index != -1)
            PlayClip(key, index);


    }
    public void PlayClip(int key, AudioClip audioClip, double ScheduledStart) // override
    {
        var index = dict[key].FindIndex(x => x.clip == audioClip);
        if (index != -1)
            PlayClip(key, index, ScheduledStart);


    }
    public void PlayClip(int key, int index, double ScheduledStart)
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
                //MusicSource.loop = dict[key][index].isLoop;
                MusicSource.PlayScheduled(AudioSettings.dspTime);

                break;
            case 1:
                previousSfxData.scheduledStart = ScheduledStart;
                SfXSource.clip = dict[key][index].clip;
                SfXSource.volume = MasterVolume * SfXVolume;
                //SfXSource.loop = dict[key][index].isLoop;
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
        MusicSource.Stop();
        SfXSource.Stop();

    }

    public void OnDestroy()
    {
        //Save
        PlayerPrefs.SetFloat("MSR", MasterVolume);
        PlayerPrefs.SetFloat("MSC", MusicSource.volume);
        PlayerPrefs.SetFloat("SFX", SfXSource.volume);

        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}


