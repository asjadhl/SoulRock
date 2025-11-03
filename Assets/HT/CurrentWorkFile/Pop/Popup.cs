using System;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public string ID;
    public GameObject prefab;
    Lazy<PopSystem> popSystem = new Lazy<PopSystem>(() =>
    {
        PopSystem system = GameObject.FindAnyObjectByType<PopSystem>();
        if (system == null)
            Debug.LogError("PopSystem not found in the scene!");
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

        if (popSystem.Value == null || prefab == null)
            return;
        popSystem.Value.PopUp(gameObject,ID);
    }

    public void Close()
    {

        if (popSystem.Value == null)
            return;
        popSystem.Value.PopDown(ID);
    }
}
