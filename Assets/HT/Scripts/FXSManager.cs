using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


 

public class FXSManager : MonoBehaviour
{

  public static FXSManager  Instance;
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
  

  Dictionary<int,List<AudioClip>> dict;
  public int currentIndex;
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
     
     
     MasterSlider.value = MusicSource.volume;
     MusicSlider.value = MusicSource.volume;
    //SfXSource = SfXSource.volume;
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

    //NextClip(0);
  }


  public void SetMasterVolume(float Op)
  {
    MasterVolume +=Op;
    MasterVolume = Mathf.Clamp01(MasterVolume);
    MasterSlider.value = MasterVolume;
    MusicSource.volume = MasterVolume * MusicVolume;
    SfXSource.volume   = MasterVolume * SfXVolume;
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

  public void PlayClip(int key,int index)
  {

    if (!dict.ContainsKey(key))
      return;

    if (dict[key].Count <= index)
    {                 return;}

    switch(key)
    {
      case 0:
        MusicSource.clip = dict[key][index];
        MusicSource.volume = MasterVolume*MusicVolume;  
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
  

}
