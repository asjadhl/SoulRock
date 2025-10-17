using System;
using UnityEngine;


public class Slider
{
  public Action<float> onValueChanged;

}

public class FXSManager : MonoBehaviour
{

  public static FXSManager  Instance;
  public AudioSource ad;
  public AudioClip clip;
  public bool isMute;
  public UnityEngine.UI.Slider MasterSlider;
  public void Awake()
  {
    if (Instance != null)
      Destroy(this.gameObject);
    else
    {
      Instance = this;
    }
  }

  public void Start()
  {
    ad = GetComponent<AudioSource>();
    PlayFxs(clip);
     MasterSlider.value = ad.volume;
     
  }


  public void SetMasterVolume(float Op)
  {
    MasterSlider.value  = ad.volume += Op;
  }
  public void UpdateAudioSource()
  {
    ad.mute = isMute;
  }


  public void PlayFxs(AudioClip clip)
  {
    ad.clip = clip;
    ad.Play();
  }
}
