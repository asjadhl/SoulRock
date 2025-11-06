using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopUpCover : MonoBehaviour
{


 
 private static System.Lazy<RectTransform> Canvas = new(() => {
    
    var unknown = FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None);
    var result = unknown.Where(p => p.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize).FirstOrDefault().GetComponent<RectTransform>();
    return result;
  });
  System.Lazy<GameObject> CoverPanel = new(() =>
  {

    GameObject CoverPanel = AutoPanelCreationSettings();
    return CoverPanel;
  });

  public void OnEnable()
  {
    if (CoverPanel.Value != null && CoverPanel.IsValueCreated)
    {
      CoverPanel.Value.SetActive(true);
    }
    else if (CoverPanel.Value == null && CoverPanel.IsValueCreated)
    {
      CoverPanel = new(() =>
      {
        GameObject CoverPanel = AutoPanelCreationSettings();
        return CoverPanel;
      });

      CoverPanel.Value.SetActive(true);
    }
    else
      CoverPanel.Value.SetActive(true);
  }

  

  public void OnDisable()
  {
    if (CoverPanel.Value != null && CoverPanel.IsValueCreated)
      CoverPanel.Value.SetActive(false);
    else if (CoverPanel.Value == null && CoverPanel.IsValueCreated)
    {
      CoverPanel = new(() =>
      {

        GameObject CoverPanel = AutoPanelCreationSettings();
        return CoverPanel;
      });

      CoverPanel.Value.SetActive(false);
    }
    else
      CoverPanel.Value.SetActive(false);
  }

  private static GameObject AutoPanelCreationSettings()
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
    CoverPanel.AddComponent<UnityEngine.UI.Image>().color = Color.black * (220 / 255f);
    return CoverPanel;
  }
}
