using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    [SerializeField] private Animator animator;
     
    public GameObject MainPanel;
    public GameObject MusicSettingPanel;
    public GameObject LicsenseSettingPanel;
    public AudioSource OurAudioSource;
    private int currentIndex = 0;
    public List<AudioClip> ListOfTestAudio;
    public TextMeshProUGUI textMeshPro;
    public bool isNextClip  = false;
    public ScrollRect _scrollrect;
    public ScrollButton ScrollButtonTop;
    public ScrollButton ScrollButtonBottom;

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
            Debug.Log("Down");
            _scrollrect.verticalNormalizedPosition -= 0.001f;
            Debug.Log($"_scrollrect.verticalNormalizedPosition: {_scrollrect.verticalNormalizedPosition}");
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
    public void UndoOrShowSetting()
    {
        if (0 >= AnimationStack.Count)
        {
            Debug.Log("Count0");
            animator.SetFloat("Flow", 1);
            AnimationStack.Push("AudioManager");
            animator.Play(AnimationStack.Peek(), 0, 0);
            DisableUnwantedThreat();
            StopPlay();
            //Freeze Time
            
            Time.timeScale = 0;
            AudioListener.pause = true;
        }
        else if (0 < AnimationStack.Count)
        {
            animator.SetFloat("Flow", -1);
            Debug.Log("Count1");
            if (AnimationStack.Count == 1) // Exiting Main Setting
            {
                EnableUnwantedThreat();
                //Release Time
                Time.timeScale = 1;
                AudioListener.pause = false;
            }
            else if (AnimationStack.Count == 2) //  Exiting Music Setting  or Licsense Showcase
            {
                if (AnimationStack.Peek() == "MusicSetting")
                {
                    isNextClip = false;
                    StopPlay();
                }
                else if (AnimationStack.Peek() == "LicsenseSetting")
                {
                    //Nothing Yet
                }
            }
            animator.Play(AnimationStack.Pop(), 0, 1);
        }
    }
     

    public void ShowMusicSetting() //Bttn;
    {
        animator.SetFloat("Flow", 1);
        AnimationStack.Push("MusicSetting");
        animator.Play(AnimationStack.Peek(), 0, 0);
        isNextClip = true;

    }
    public void ShowLiscense() //Bttn;
    {
        animator.SetFloat("Flow", 1);
        AnimationStack.Push("LicsenseSetting");
        animator.Play(AnimationStack.Peek(), 0, 0);
    }

    public void NextClip(int dir)
    {
        if (!isNextClip) return;

        //Queue Algorithm   Phython 
        currentIndex = (((currentIndex + dir) % ListOfTestAudio.Count) + ListOfTestAudio.Count) % ListOfTestAudio.Count;
        OurAudioSource.clip = ListOfTestAudio[currentIndex];
        OurAudioSource.Play();
        textMeshPro.text = ListOfTestAudio[currentIndex].name;
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


        if (instance != null)
            Destroy(this.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);


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
            if (animator != null)
            {
                animator.SetFloat("Flow", -1);
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            OurAudioSource.ignoreListenerPause = true;
        }
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
        myMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) *20);
    }

    

    private void LoadMasterVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume"); 
    }
    private void LoadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    }
    private void LoadSFXVolume()
    {
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }


}
