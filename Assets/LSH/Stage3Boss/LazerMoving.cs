using UnityEngine;

public class LazerMoving : MonoBehaviour
{
    [SerializeField] float lazerSpeed = 10f;
    bool isInitialized = false;
    ParticleManager particleManager;
    private void Awake()
    {
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
    }
    void Start()
    {
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, 0, -1) * lazerSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _ = GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        if (!isInitialized) return;
        Vector3 effectPos = transform.position + new Vector3(0, 1f, 0);
        particleManager.PlayHitEffect(effectPos);
    }
}
