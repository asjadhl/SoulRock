using System.Collections;
using UnityEngine;

public class LazerTrap : MonoBehaviour
{
    public GameObject[] lazerObject = new GameObject[4];
    public GameObject[] lazers = new GameObject[4];
    public int MaxHealth = 1;
    public int CurrentHealth;

    public float firstScle = 0.1f;
    public float lastScale = 7f;
    public float RotateSpeed = 300f;
    private RaycastHit hit;

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
    public void OnHit()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f))
        {
            for (int i = 0; i < lazerObject.Length; i++)
            {
                if (hit.collider.gameObject == lazerObject[i])
                {
                    CurrentHealth--;


                    if (CurrentHealth <= 0)
                    {
                        lazerObject[i].SetActive(false);
                        if (i < lazers.Length && lazers[i] != null)
                        {
                            lazers[i].SetActive(false);
                        }
                    }
                    break;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _ = GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
        }
    }
    public void FixedUpdate()
    {
        foreach (GameObject lazer in lazerObject)
        {
            if (lazer.activeSelf)
            {
                lazer.transform.Rotate(Vector3.up * RotateSpeed * Time.fixedDeltaTime, Space.World);
                if (lazer.transform.localScale.x < lastScale)
                {
                    float newScale = lazer.transform.localScale.x + firstScle;
                    lazer.transform.localScale = new Vector3(newScale, 1f, newScale);
                    
                }
            }

        }

    }

}
