using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Audio;

public class LanguageToggle : MonoBehaviour
{
    public Toggle englishToggle;
    public Toggle koreanToggle;
    public AudioSource audiosource;
    public AudioMixerGroup SFXGroup;
    Lazy<PopSystem> popSystem = new Lazy<PopSystem>(() =>
    {
        PopSystem system = GameObject.FindAnyObjectByType<PopSystem>();
        if (system == null)
            Debug.LogError("PopSystem not found in the scene!");
        return system;
    });
    public enum Language
    {
        en = 0,
        ko = 1
    }

    private bool active = false;

    private void Start()
    {    

       

        if(GetComponent<AudioSource>() == null)
             audiosource = gameObject.AddComponent<AudioSource>();
             audiosource.playOnAwake = false;
        audiosource.ignoreListenerPause = true;
        audiosource.outputAudioMixerGroup = SFXGroup;
        audiosource.clip = popSystem.Value.GetAudioSource().clip;
        


        // Load saved language, default = English (0)
        int savedLang = PlayerPrefs.GetInt("LocalKey", 0);

        // Apply language on start
        SetLocal((Language)savedLang).Forget();

        // Reflect toggle UI correctly
        englishToggle.SetIsOnWithoutNotify(savedLang == (int)Language.en);
        koreanToggle.SetIsOnWithoutNotify(savedLang == (int)Language.ko);

        // Add Toggle Event Listeners
        englishToggle.onValueChanged.AddListener(isOn =>
        {
            audiosource.Play();
            Debug.Log("AAA");
            if (isOn && !active)
            {
                koreanToggle.SetIsOnWithoutNotify(false);
                SetLocal(Language.en).Forget();
            }
        });

        koreanToggle.onValueChanged.AddListener(isOn =>
        {
            audiosource.Play();
            Debug.Log("BBB");
            if (isOn && !active)
            {
                englishToggle.SetIsOnWithoutNotify(false);
                SetLocal(Language.ko).Forget();
            }
        });
    }

    private async UniTaskVoid SetLocal(Language lang)
    {
        active = true;

        // Make sure Localization System has finished loading
        await LocalizationSettings.InitializationOperation.Task;

        // Apply language
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)lang];

        // Save to PlayerPrefs
        PlayerPrefs.SetInt("LocalKey", (int)lang);
        PlayerPrefs.Save();

        active = false;
    }
}
