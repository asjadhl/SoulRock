using UnityEngine;

public class test2 : MonoBehaviour
{
  int c = 0;
  public void OnEnable()
  {
    Debug.Log(++c);
    Debug.Log("OnEnable");
  }

  public void Awake()
  {
    Debug.Log(++c);
    Debug.Log("OnAwake");
  }
  public void Start()
  {
    Debug.Log(++c);
    Debug.Log("OnStart");
  }

  public void OnDisable()
  {
    Debug.Log(++c);
    Debug.Log("OnDisable");
  }
  public void OnDestroy()
  {
    Debug.Log(++c);
    Debug.Log("OnDestroy");
  }
}
