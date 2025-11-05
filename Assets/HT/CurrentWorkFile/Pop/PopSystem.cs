using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PopSystem : MonoBehaviour
{
    public AudioMixer myMixer;
    private Lazy<CancellationTokenSource> cts = new(() => new CancellationTokenSource());

    private AudioSource audiosource;
    private Dictionary<string, GameObject> panelMap = new();

    private Lazy<Transform> m_canvas = new(() =>
    {
      //  var canvasObj = GameObject.FindAnyObjectByType<Canvas>();
      var canvasObj = GameObject.FindGameObjectWithTag("Hitbox");
        if (canvasObj == null)
        {
        //Create New Canvas
        GameObject canvas = new GameObject("Canvas");
        canvas.AddComponent<RectTransform>();
            var canvascom = canvas.AddComponent<Canvas>();//.renderMode = RenderMode.ScreenSpaceOverlay;
            canvascom.renderMode = RenderMode.ScreenSpaceOverlay;
            canvascom.sortingOrder = 5;
        canvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.AddComponent<GraphicRaycaster>();
        canvasObj = canvas;
        }
        return canvasObj.transform;
    });

    public RuntimeAnimatorController runtimeAnimatorController;
    private int currentActiveCount = 0;
  private void Start()
  {
        Initialization();
    audiosource =GetComponent<AudioSource>();
    audiosource.ignoreListenerPause = true;

  }


   private void Initialization()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            LoadMasterVolume();
            Debug.Log("AA");
        }
        else
            SetMasterVolume();

        if (PlayerPrefs.HasKey("MusicVolume"))
            LoadMusicVolume();
        else
            SetMusicVolume();

        if (PlayerPrefs.HasKey("SFXVolume"))
            LoadSFXVolume();
        else
            SetSFXVolume();

    }
    public void SetMasterVolume() => myMixer.SetFloat("Master", Mathf.Log10(1) * 20);
    public void SetMusicVolume() => myMixer.SetFloat("Music", Mathf.Log10(1) * 20);
    public void SetSFXVolume() => myMixer.SetFloat("SFX", Mathf.Log10(1) * 20);
    private void LoadMasterVolume() => myMixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
    private void LoadMusicVolume() => myMixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
    private void LoadSFXVolume() => myMixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume")) * 20);


    public void PopUp(GameObject prefab,string key)
    {     
            audiosource.Play();
        if (m_canvas.Value == null) return;

       if (prefab == null) return;


        
        if (panelMap.TryGetValue(key, out GameObject existingPanel))
        {

      //Check if is already setactive true
      if (existingPanel.activeSelf)
        return;   

            existingPanel.SetActive(true);
            Animator anim = existingPanel.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("Show", 0, 0f);
            }
            var canvasgroup1 = existingPanel.GetComponent<CanvasGroup>();
            canvasgroup1.interactable = true;
            currentActiveCount++;
                 

           if(currentActiveCount == 1)
               DisableUnwantedThreat();
            return;
        }

     
        GameObject panel = Instantiate(prefab, m_canvas.Value);
        panel.SetActive(true);
        var canvasgroup = panel.AddComponent<CanvasGroup>();
        canvasgroup.interactable = true;
        Animator animator = panel.GetComponent<Animator>();
        if (animator == null)
            animator = panel.AddComponent<Animator>();

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.runtimeAnimatorController = runtimeAnimatorController;
        animator.Play("Show", 0, 0f);

         
        panelMap.Add(key, panel);
    currentActiveCount++;
    if (currentActiveCount == 1)
      DisableUnwantedThreat();
  }

  
    public void PopDown(string key)
    {

    audiosource.Play();
    if (panelMap.TryGetValue(key, out GameObject panel))
        {   
          var canvasgroup =  panel.GetComponent<CanvasGroup>();
            canvasgroup.interactable = false;
            
            Animator animator = panel.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("Close", 0, 0f);
                HideAfterAnimation(animator, panel, cts.Value.Token).Forget();
                currentActiveCount--;
            }
            else
            {
                  
        // Fallback if no animator → just hide
            panel.SetActive(false);
              currentActiveCount--;
            }

      if (currentActiveCount <= 0)
        EnableUnwantedThreat();
        }
        
    }
   
    private async UniTaskVoid HideAfterAnimation(Animator animator, GameObject panel, CancellationToken token)
    {
        
        try
        {
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Close"),
                                    cancellationToken: token);
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f,
                                    cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Close animation canceled — forcing hide.");
        }
        finally
        {
            if (panel != null)
                panel.SetActive(false);

            
        }
    }
    public AudioSource GetAudioSource()
    {
        return audiosource;
    }
    private void OnDestroy()
    {
        if (cts.IsValueCreated)
        {
            cts.Value.Cancel();
            cts.Value.Dispose();
        }
    }
  public void EnableUnwantedThreat()
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
        MainQuitGhost[i].enabled = true;
      }
    }
  }
  public void DisableUnwantedThreat()
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
}
