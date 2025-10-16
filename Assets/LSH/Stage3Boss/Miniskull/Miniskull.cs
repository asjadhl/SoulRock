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
	private void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("Player"))
        {
			skull.transform.LookAt(playertransform);
		}
	}
}
