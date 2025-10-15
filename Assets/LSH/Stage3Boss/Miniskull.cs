using UnityEngine;

public class Miniskull : MonoBehaviour
{
    private Transform playertransform;
    [SerializeField] GameObject skull;
    void Awake()
    {
		playertransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
	}

    // Update is called once per frame
    void Update()
    {
         
    }

    void MoveToPlayer()
    {
        
    }
	private void OnTriggerStay(Collider col)
	{
		if(col.CompareTag("Player"))
        {
			skull.transform.LookAt(playertransform);
			skull.transform.Translate(Vector3.forward * 10 * Time.deltaTime);
		}
	}
}
