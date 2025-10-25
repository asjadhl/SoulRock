using UnityEngine;

public class WallBoom : MonoBehaviour
{
    ParticleManager particleManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GhostBoss"))
        {
            Vector3 effectPos = transform.position + new Vector3(0, 0, 0.1f);
            particleManager.PlaySkullEffect(effectPos);
            gameObject.SetActive(false);
        }
    }
}
