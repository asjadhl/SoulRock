using UnityEngine;

public class AxTrap : MonoBehaviour
{
    [Header("Pivot")]
    public Transform pivotTransform; 
    public Vector3 pivotPoint = Vector3.zero;

    [Header("Motion")]
    [Tooltip("MaxAngle")]
    public float amplitude = 50f;    
    [Tooltip("1hz")]
    public float frequency = 0.5f;      
    [Tooltip("damping")]
    public float damping = 0f;      
    [Tooltip("pivot")]
    public Vector3 axis = Vector3.forward;

    [Header("Control")]
    public bool autoStart = true;
    public float startDelay = 0f;      
    [Range(0f, 1f)]
    public float initialAmplitudeMultiplier = 1f;
    public float initialPhase = 0f;   

    // 상태
    [HideInInspector] public bool isPlaying = false;

    // 내부
    private float _time = 0f;
    private float _prevAngle = 0f;
    private float _currentAmplitude; 

    public int maxHealth = 3;   
    int currentHealth;
    RaycastHit hit;

    void Start()
    {
        _currentAmplitude = amplitude * initialAmplitudeMultiplier;
        _time = 0f;


        _prevAngle = Mathf.Sin(initialPhase) * _currentAmplitude;

        if (autoStart)
        {
            if (startDelay <= 0f) StartSwing();
            else Invoke(nameof(StartSwing), startDelay);
        }
    }
    public void OnHit()
    {
        if (currentHealth <= 0)
        {

            if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
            {
                currentHealth--;
                if (currentHealth <= 0)
                {
                    this.gameObject.SetActive(false);
                }
            }

        }
    }
    void Update()
    {
        if (!isPlaying || (!Application.isPlaying && !autoStart)) return;

        Vector3 pivot = pivotTransform != null ? pivotTransform.position : pivotPoint;

        float angle = Mathf.Sin((2f * Mathf.PI * frequency * _time) + initialPhase) * _currentAmplitude;

        float delta = angle - _prevAngle;

        transform.RotateAround(pivot, axis.normalized, delta);


        _prevAngle = angle;


        _time += Time.deltaTime;


        if (damping > 0f)
        {

            _currentAmplitude *= Mathf.Exp(-damping * Time.deltaTime);
            if (_currentAmplitude < 0.001f) _currentAmplitude = 0f;
        }
    }
    public void StartSwing()
    {

        isPlaying = true;
        _time = 0f;
        _currentAmplitude = amplitude * initialAmplitudeMultiplier;
        _prevAngle = Mathf.Sin(initialPhase) * _currentAmplitude;
    }

    public void StopSwing()
    {
        isPlaying = false;
    }

    public void SetAmplitude(float newAmplitude)
    {
        amplitude = Mathf.Max(0f, newAmplitude);
        _currentAmplitude = amplitude;
    }


    public void SetFrequency(float newFreq)
    {
        frequency = Mathf.Max(0f, newFreq);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pivot = pivotTransform != null ? pivotTransform.position : pivotPoint;
        Gizmos.DrawSphere(pivot, 0.05f);

        Vector3 dir = axis.normalized * 0.5f;
        Gizmos.DrawLine(pivot, pivot + dir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _ = GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
        }
    }
}