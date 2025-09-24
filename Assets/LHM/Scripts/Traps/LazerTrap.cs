using System.Collections;
using UnityEngine;

public class LazerTrap : MonoBehaviour
{
    public GameObject[] lazerObject = new GameObject[4];
    public GameObject[] lazers = new GameObject[4];
    public int MaxHealth = 5;
    public int CurrentHealth;

    public float firstScle = 0.1f;
    public float lastScale = 7f;
    public float RotateSpeed = 300f;

    public void Awake()
    {
        CurrentHealth = MaxHealth;

        foreach (GameObject lazer in lazerObject)
        {
            lazer.SetActive(false);
            
        }
        foreach (GameObject l in lazers)
        {
            l.SetActive(false);
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
        for (int i = 0; i < lazers.Length; i++)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            if (!lazers[i].activeSelf)
            {
                lazers[i].SetActive(true);
            }
        }

    }
    public void FixedUpdate()
    {
       // Rotate(new Vector3(360, 0, 0) * Time.deltaTime);
    }

}
