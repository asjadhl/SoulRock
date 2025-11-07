using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class MusicSetting : MonoBehaviour
{


  [System.Serializable]
  public enum ModeType
  {
    Master, Music, SFX
  }



  [SerializeField] private AudioMixer myMixer;
  [SerializeField] private Slider masterSlider;
  [SerializeField] private Slider musicSlider;
  [SerializeField] private Slider SFXSlider;





  public AudioSource OurAudioSource;
  public AudioClip OurAudioClip;
  public Dictionary<int, List<AudioClip>> dictest;
  public AudioMixerGroup MasterGroup;
  public AudioMixerGroup MusicGroup;
  public AudioMixerGroup SFXGroup;
 
  private void Start()
  {
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

    OurAudioSource.clip = OurAudioClip;
    OurAudioSource.ignoreListenerPause = true;
    masterSlider.minValue = 0.0001f;
    musicSlider.minValue = 0.0001f;
    SFXSlider.minValue = 0.0001f;
  }



  public void SetMasterVolume(float dir) //slider
  {

    masterSlider.value += dir;
    myMixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);

    OurAudioSource.outputAudioMixerGroup = MasterGroup;
    OurAudioSource.Play();
  }



  public void SetMusicVolume(float dir) //slider
  {
    musicSlider.value += dir;
    

    myMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
    OurAudioSource.outputAudioMixerGroup = MusicGroup;
    OurAudioSource.Play();
  }

  public void SetSFXVolume(float dir) //slider
  {
    SFXSlider.value += dir;

  
      myMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);
   

    
    OurAudioSource.outputAudioMixerGroup = SFXGroup; 
    OurAudioSource.Play();
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


  public void OnDestroy()
  {
    PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
  }
}
