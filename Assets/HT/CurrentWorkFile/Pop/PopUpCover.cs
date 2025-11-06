using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
public class PopUpCover : MonoBehaviour
{


  public CancellationTokenSource m_cts = new();
  private bool isActiveTask = false;
 private static System.Lazy<RectTransform> Canvas = new(() => {
    
    var unknown = FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None);
    var result = unknown.Where(p => p.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize).FirstOrDefault().GetComponent<RectTransform>();
    return result;
  });
  System.Lazy<GameObject> CoverPanel = new(() =>
  {

    GameObject CoverPanel = AutoPanelCreationSettings(true);
    return CoverPanel;
  });
  
  public void OnEnable()
  {



    if (CoverPanel.Value != null && CoverPanel.IsValueCreated)
    {
      CoverPanel.Value.SetActive(true);
      FadingEffect(true).Forget();
    }
    else if (CoverPanel.Value == null && CoverPanel.IsValueCreated)
    {
      CoverPanel = new(() =>
      {
        GameObject CoverPanel = AutoPanelCreationSettings(false);
        return CoverPanel;
      });
      CoverPanel.Value.SetActive(true);
      FadingEffect(true).Forget();
    }
    else
    { CoverPanel.Value.SetActive(true); FadingEffect(true).Forget(); }
  }



  public void OnDisable()
  {       
    if (CoverPanel.Value != null && CoverPanel.IsValueCreated)
      FadingEffect(false, () => { CoverPanel.Value.SetActive(false); }).Forget();
    else if (CoverPanel.Value == null && CoverPanel.IsValueCreated)
    {
      CoverPanel = new(() =>
      {
        GameObject CoverPanel = AutoPanelCreationSettings(true);
        return CoverPanel;
      });

      FadingEffect(false, () => { CoverPanel.Value.SetActive(false); }).Forget();
    }
    else
      FadingEffect(false, () => { CoverPanel.Value.SetActive(false); }).Forget();
     
    
  }
  private async UniTaskVoid FadingEffect(bool isActive,Action callback = null)
  {
    if (!isActiveTask)
    {
      isActiveTask = true;
       
        var images = CoverPanel.Value.GetComponent<Image>();
        if (images == null)
        { isActiveTask = false;           
          return; }

        var cts = images.GetCancellationTokenOnDestroy();

        float t = 0;
        Color prev = images.color;
        Color newColor = isActive ? Color.black * (220f / 225f) : Color.black * 0f;

        while (!cts.IsCancellationRequested || m_cts.IsCancellationRequested)
        {
          images.color = Color.Lerp(prev, newColor, Mathf.Clamp01(t));
          t += Time.unscaledDeltaTime * 3f;
          if (t > 1f)
            break;
          await UniTask.Yield();
        }
         
       
        isActiveTask = false;
     
      if (cts.IsCancellationRequested || m_cts.IsCancellationRequested)
      { Debug.Log("SAFE"); 
       
      }
      else if(!cts.IsCancellationRequested || !m_cts.IsCancellationRequested)
      {
        callback?.Invoke();
      }
    }
    else
    {
      m_cts.Cancel();
      isActiveTask = false;
      m_cts.Dispose();
      m_cts = new();
      FadingEffect(isActive, callback).Forget();
    }
    




  }


  private static GameObject AutoPanelCreationSettings(bool isActive) //Fix Colors 
  {
    GameObject CoverPanel = new GameObject("AutoPanel");
    RectTransform rect = CoverPanel.AddComponent<RectTransform>();
    rect.SetParent(Canvas.Value);
    rect.SetSiblingIndex(1);
    rect.anchorMin = Vector2.zero;
    rect.anchorMax = Vector2.one;
    rect.offsetMin = Vector2.zero;
    rect.offsetMax = Vector2.zero;
    rect.localScale = Vector3.one;
    CoverPanel.AddComponent<UnityEngine.UI.Image>().color = isActive ? Color.black * (220 / 255f) : Color.black * 0f; ;

 
    return CoverPanel;
  }
}
