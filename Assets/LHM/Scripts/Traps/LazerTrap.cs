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
        // Raycast로 맞은 대상 감지
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f))
        {
            // 맞은 오브젝트가 lazerObject 배열 중 하나인지 확인
            for (int i = 0; i < lazerObject.Length; i++)
            {
                if (hit.collider.gameObject == lazerObject[i])
                {
                    // HP 감소
                    CurrentHealth--;

                    // 만약 해당 수정 구슬이 체력 0이 되면 비활성화
                    if (CurrentHealth <= 0)
                    {
                        lazerObject[i].SetActive(false);

                        // 연결된 레이저도 함께 비활성화
                        if (i < lazers.Length && lazers[i] != null)
                        {
                            lazers[i].SetActive(false);
                        }
                    }
                    break; // 맞은 구슬 찾았으니 반복문 종료
                }
            }
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
