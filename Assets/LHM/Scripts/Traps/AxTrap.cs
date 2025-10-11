using UnityEngine;

public class AxTrap : MonoBehaviour
{
    [Header("Pivot")]
    public Transform pivotTransform;        // 힌지로 사용할 Transform (옵션)
    public Vector3 pivotPoint = Vector3.zero; // pivotTransform을 안쓸 때 사용하는 월드 좌표

    [Header("Motion")]
    [Tooltip("MaxAngle")]
    public float amplitude = 50f;           // 최대 각도 (degree)
    [Tooltip("1hz")]
    public float frequency = 0.5f;         // Hz (사인파의 주파수)
    [Tooltip("damping")]
    public float damping = 0f;             // 진폭 감쇠 계수
    [Tooltip("pivot")]
    public Vector3 axis = Vector3.forward;

    [Header("Control")]
    public bool autoStart = true;
    public float startDelay = 0f;          // 시작 지연(초)
    [Range(0f, 1f)]
    public float initialAmplitudeMultiplier = 1f; // 시작시 진폭 배율
    public float initialPhase = 0f;        // 시작 위상(라디안)

    // 상태
    [HideInInspector] public bool isPlaying = false;

    // 내부
    private float _time = 0f;
    private float _prevAngle = 0f; // 이전 프레임의 각도 (degrees)
    private float _currentAmplitude; // 현재 감쇠 적용된 진폭

    public int maxHealth = 3;   
    int currentHealth;
    RaycastHit hit;

    void Start()
    {
        _currentAmplitude = amplitude * initialAmplitudeMultiplier;
        _time = 0f;

        // 초기 각도 세팅 (사인 기반)
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
            // 문 파괴
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

        // pivot 위치 설정
        Vector3 pivot = pivotTransform != null ? pivotTransform.position : pivotPoint;

        // 계산: angle(t) = sin(2π * f * t + phase) * amplitude
        float angle = Mathf.Sin((2f * Mathf.PI * frequency * _time) + initialPhase) * _currentAmplitude;

        // 이번 프레임에서 회전해야 할 '증분' 각도 (degrees)
        float delta = angle - _prevAngle;

        // pivot 기준으로 axis축을 따라 delta 만큼 회전 (degrees)
        transform.RotateAround(pivot, axis.normalized, delta);

        // 다음 프레임을 위해 저장
        _prevAngle = angle;

        // 시간 증가
        _time += Time.deltaTime;

        // 감쇠 적용 (지수 감쇠)
        if (damping > 0f)
        {
            // amplitude를 지속적으로 감소시키되, 최소 0 이하가 되지 않음
            _currentAmplitude *= Mathf.Exp(-damping * Time.deltaTime);
            if (_currentAmplitude < 0.001f) _currentAmplitude = 0f;
        }
    }

    /// <summary>스윙 시작</summary>
    public void StartSwing()
    {
        // 시간과 이전 각도 보정: 현재 시점에서 자연스럽게 이어지도록 초기화
        isPlaying = true;
        _time = 0f;
        _currentAmplitude = amplitude * initialAmplitudeMultiplier;
        _prevAngle = Mathf.Sin(initialPhase) * _currentAmplitude;
    }

    /// <summary>스윙 멈춤 (현재 위치에 정지)</summary>
    public void StopSwing()
    {
        isPlaying = false;
    }

    /// <summary>진폭을 즉시 설정 (도 단위)</summary>
    public void SetAmplitude(float newAmplitude)
    {
        amplitude = Mathf.Max(0f, newAmplitude);
        _currentAmplitude = amplitude;
    }

    /// <summary>주파수 설정 (Hz)</summary>
    public void SetFrequency(float newFreq)
    {
        frequency = Mathf.Max(0f, newFreq);
    }

    // 편의: 에디터에서 축 시각화 (선택)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pivot = pivotTransform != null ? pivotTransform.position : pivotPoint;
        Gizmos.DrawSphere(pivot, 0.05f);

        // 축 방향 화살표
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