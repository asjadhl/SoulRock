using UnityEngine;
using UnityEngine.Audio;

public class SongTaster : MonoBehaviour
{
  [SerializeField] AudioClip       OurMusicClip;
  [SerializeField] AudioClip       OurSFXClip;
  [SerializeField] AudioSource     OurAudioSouce;
  [SerializeField] AudioMixerGroup master;
  [SerializeField] AudioMixerGroup music;
  [SerializeField] AudioMixerGroup SFX;
  private          bool            IsMusic = false;


  public void OnEnable()
  {
    Play();
  }

  private void Start()
  {
    OurAudioSouce.ignoreListenerPause = true; 
  }
  public void OnDisable()
  {
    OurAudioSouce.Stop();
  }
  public void Toggle()
  {
    IsMusic = !IsMusic;
    Play();
  }

 

  private void Play()
  {
    switch(IsMusic)
    {
      case false:
        OurAudioSouce.clip = OurMusicClip;
        OurAudioSouce.outputAudioMixerGroup = music;
        break;
      case true:
        OurAudioSouce.clip = OurSFXClip;
        OurAudioSouce.outputAudioMixerGroup = SFX;
        break;
    }


    OurAudioSouce.Play();
  }
  
  public void StopAllAudio()
  {
    AudioListener.pause = true;
  }
  public void PlayAllAudio()
  {
    AudioListener.pause = false;
  }









}
