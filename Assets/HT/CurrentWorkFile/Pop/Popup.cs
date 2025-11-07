using System;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public string ID;
    public GameObject prefab;
    Lazy<PopSystem> popSystem = new Lazy<PopSystem>(() =>
    {
        PopSystem system = GameObject.FindAnyObjectByType<PopSystem>();
#if UNITY_EDITOR
      if (system == null)
      {
        Debug.LogError("PopSystem not found in the scene!");
        Debug.LogError("File At:  Asset/HT/CurrentWorkFile/Pop/PopUpSystem");
      }
#endif
      return system;
    });

    public bool isAwake = false;
    public void Awake()
    {
        if (isAwake)
        {
            Open();
        }
    }
    public void Open()
    {
    if (popSystem.Value == null && popSystem.IsValueCreated)
    {
      popSystem = new Lazy<PopSystem>(() =>
        {
          PopSystem system = GameObject.FindAnyObjectByType<PopSystem>();
                   #if UNITY_EDITOR
          if (system == null)
          {
            Debug.LogError("PopSystem not found in the scene!");
            Debug.LogError("File At:  Asset/HT/CurrentWorkFile/Pop/PopUpSystem");
          }
                    #endif
          return system;
        });
    }
    if (popSystem.Value == null  || prefab == null)
            return;


        popSystem.Value.PopUp(prefab,ID);
    }

    public void Close()
    {

        if (popSystem.Value == null)
            return;
        popSystem.Value.PopDown(ID);
    }
}
