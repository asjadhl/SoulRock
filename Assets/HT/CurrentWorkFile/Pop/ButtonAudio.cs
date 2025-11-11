using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour
{
    public AudioSource audios;
    public AudioMixerGroup SFXmixer;
    public Button bttn;
    public AudioClip clip;
    public void Start()
    {
       audios = GetComponent<AudioSource>();
        if (audios == null)
        audios =  this.AddComponent<AudioSource>();

        audios.outputAudioMixerGroup = SFXmixer;
        audios.clip = clip;
         bttn = GetComponent<Button>();
        if (bttn == null)
        {

#if UNITY_EDITOR
            Debug.LogError("ButtonComponent Null");
#endif
        }
        else
        bttn.onClick.AddListener(PlayButtonSFX);
    }


    public void PlayButtonSFX()
    {   
        audios.Play();
    }
}
