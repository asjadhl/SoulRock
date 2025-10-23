using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;




public class FXSManager : MonoBehaviour
{
  public static FXSManager Instance;

  public AudioSource MusicSource;
  public AudioSource SfXSource;
  public bool isMute;
  public UnityEngine.UI.Slider MasterSlider;
  public UnityEngine.UI.Slider MusicSlider;
  public UnityEngine.UI.Slider SfXSlider;
  public List<AudioClip> MusicAudioClip;
  [Space(2f)]
  public List<AudioClip> SfXAudioClip;
  public float MasterVolume;
  public float MusicVolume;
  public float SfXVolume;
  [Space(2f)]
  public Animator anim;
  Stack<string> m_Stack = new();
 
  Dictionary<int, List<AudioClip>> dict;
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
    if (MusicAudioClip.Count > 0)
    {
      dict.Add(0, new());
      for (int i = 0; i < MusicAudioClip.Count; i++)
      {
        dict[0].Add(MusicAudioClip[i]);
      }
    }

    if (SfXAudioClip.Count > 0)
    {
      dict.Add(1, new());
      for (int i = 0; i < SfXAudioClip.Count; i++)
      {
        dict[1].Add(SfXAudioClip[i]);
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
    
    }
    else if (Input.GetKeyDown(KeyCode.Escape) && 0 < m_Stack.Count)
    {
 
      anim.SetFloat("S", -1);
      anim.Play(m_Stack.Pop(), 0, 1);
      
    }


  }

  public void ShowFXSSetting()
  {
    
    anim.SetFloat("S", 1);
    m_Stack.Push("SoundsSettingShowUp");
    anim.Play(m_Stack.Peek(), 0, 0);
 
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
        MusicSource.clip = dict[key][index];
        MusicSource.volume = MasterVolume * MusicVolume;
        MusicSource.Play();
         
        break;
      case 1:
        SfXSource.clip = dict[key][index];
        SfXSource.volume = MasterVolume * SfXVolume;
        SfXSource.Play();
        break;
    }

  }

  public void NextClip(int dir)
  {
    //Queue Algorithm   Phython 

    currentIndex = (((currentIndex + dir) % dict[0].Count) + dict[0].Count) % dict[0].Count;
    PlayClip(0, currentIndex);
    textMeshPro.text = MusicSource.clip.name;

  }
  public void StopPlay()
  {
    MusicSource.Stop();
    SfXSource.Stop();
        Debug.Log("StopPlaying");
  }
  public AnimationClip GetClipByName(string clipName)
  {
    foreach (var clip in anim.runtimeAnimatorController.animationClips)
    {
      if (clip.name == clipName) return clip;
    }
    return null;
  }
 
   

  public void OnDestroy()
  {

    PlayerPrefs.SetFloat("MSR", MasterVolume);
    PlayerPrefs.SetFloat("MSC", MusicSource.volume);
    PlayerPrefs.SetFloat("SFX", SfXSource.volume);
  }
}


