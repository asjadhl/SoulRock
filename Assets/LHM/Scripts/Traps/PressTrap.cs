using UnityEngine;
using System.Collections;

public class PressTrap : MonoBehaviour
{
    public float downSpeed = 10f;   
    public float upSpeed = 5f;    
    public float waitTime = 1f;     
    public float downDistance = 5f;
    public float firstSpeed = 10f;
    public float firstdownDistace = 5f;

    public int maxhp = 2;
    public int currenthp;

    private Vector3 startPos;
    private Vector3 downPos;
    private Vector3 firstdownPos;

    RaycastHit hit;

    void Start()
    {
        startPos = transform.position;
        downPos = startPos + Vector3.down * downDistance;
        firstdownPos = startPos + Vector3.down * firstdownDistace;
        StartCoroutine(PressCycle());
        currenthp = maxhp;
    }

    IEnumerator PressCycle()
    {
        while (true)
        {
            while (Vector3.Distance(transform.position, firstdownPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, firstdownPos, firstSpeed * Time.deltaTime);
                yield return null;
            }
            while (Vector3.Distance(transform.position, downPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, downPos, downSpeed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);


            while (Vector3.Distance(transform.position, startPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, upSpeed * Time.deltaTime);
                yield return null;
            }

  
            yield return new WaitForSeconds(waitTime);
        }
    }
    public void OnHit()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {
            currenthp--;
            if (currenthp <= 0)
            {
                this.gameObject.SetActive(false);
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
}
