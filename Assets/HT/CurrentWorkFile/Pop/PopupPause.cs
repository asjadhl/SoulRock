using UnityEngine;

public class PopupPause : MonoBehaviour
{
  private void OnEnable()
  {
    AudioListener.pause = true;
    Time.timeScale = 0;
  }
  private void OnDisable()
  {
    AudioListener.pause = false;
    Time.timeScale = 1;
  }
}
