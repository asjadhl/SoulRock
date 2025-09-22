using System.Collections;
using UnityEngine;

public class LazerTrap : MonoBehaviour
{
    public GameObject[] lazerObject = new GameObject[4];
    public int MaxHealth = 5;
    public int CurrentHealth;
    public float RotateSpeed = 300f;

    public void Awake()
    {
        CurrentHealth = MaxHealth;

        foreach (GameObject lazer in lazerObject)
        {
            lazer.SetActive(false);
        }

        StartCoroutine(ActivateLazers());
    }

    private IEnumerator ActivateLazers()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        for (int i = 0; i < lazerObject.Length; i++)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            if (!lazerObject[i].activeSelf)
            {
                lazerObject[i].SetActive(true);
                
            }
        }

        
    }

    
}
