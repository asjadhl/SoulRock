using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CircleHit : MonoBehaviour
{
    public static CircleHit Instance { get; private set; }
    [Header("타겟 이미지")]
    [SerializeField] Transform targetImage;

    [Header("원크기")]
    public float circleBig = 11f;
    [Header("BPM (박자 속도)")]
    public double bpm = 120.0;

    [Header("판정 거리")]
    [SerializeField] public float minDis = 0.01f;
    [SerializeField] public float exDis = 70f;
    [SerializeField] public float maxDis = 120f;

    [Header("원 도트 프리팹")]
    [SerializeField] GameObject CirclePrefab;

    [Header("풀 사이즈")]
    [SerializeField] int poolSize = 15;

    [Header("Combo")]
    public int combo = 0;
    [SerializeField] GameObject comboText;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI comboNumText;
    [SerializeField] GameObject[] feverImage;
    //AudioSource a;

    List<AudioSource> listaudio = new List<AudioSource>();
    int writeIndex_ = -1;
    int writeIndex
    {
        get
        {

            writeIndex_ = ((writeIndex_ + 1) % 8);

            return writeIndex_;


        }
        set { writeIndex_ = value; }
    }
    public void StartCreatingAudio()
    {

        var SFXMixerGroup = GetComponent<AudioSource>().outputAudioMixerGroup;
        for (int i = 0; i < 8; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.outputAudioMixerGroup = SFXMixerGroup;
            listaudio.Add(src);
        }
    }

    public void DynamicPlayClip(AudioClip _clip)
    {
        var src = listaudio[writeIndex];
        src.clip = _clip;
        src.pitch = UnityEngine.Random.Range(0.90f, 1.05f);
        src.Play();
    }


    [SerializeField] AudioClip clip;
    [SerializeField] PlayerShoot playerShoot;
    private PlayerHP playerHPSc;
    public GameObject[] poolCircle;
    private int pivot = 0;
    private double secondsPerBeat;
    private List<CircleMove> activeCircles = new List<CircleMove>();
    public bool getDamage = false;
    CanvasGroup cg;
    Color randomColor;
    public Color feverColor;
    public bool isScale = false;
    public bool isHighLight = false;
    double firstBpm = 0;
    Image image;
    Color originalColor;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        poolCircle = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            GameObject circleDot = Instantiate(CirclePrefab, transform);
            circleDot.SetActive(false);
            poolCircle[i] = circleDot;
        }
        for (int i = 0; i < feverImage.Length; i++)
        {
            feverImage[i].SetActive(false);
        }
        secondsPerBeat = 60.0 / bpm;
        firstBpm = secondsPerBeat;
        cg = comboText.GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    private void Start()
    {
        //a = GetComponent<AudioSource>();
        playerShoot = FindAnyObjectByType<PlayerShoot>();
        playerHPSc = FindAnyObjectByType<PlayerHP>();
        StartCreatingAudio();

        CircleGen().Forget();
    }

    private void Update()
    {
        CheckCol();
        if (isHighLight)
        {
            maxDis = 80f;
            bpm = 170;
            secondsPerBeat = 60.0 / bpm;
            for (int i = 0; i < feverImage.Length; i++)
            {
                feverImage[i].SetActive(true);
            }
        }
        else
        {
            maxDis = 140f;
            secondsPerBeat = firstBpm;
            for (int i = 0; i < feverImage.Length; i++)
            {
                feverImage[i].SetActive(false);
            }
        }
    }

    void CheckCol()
    {
        if (Input.GetMouseButtonDown(0) && !playerHPSc.isPlayerDead)
        {
            for (int i = activeCircles.Count - 1; i >= 0; i--)
            {
                var circle = activeCircles[i];
                float distance = Vector2.Distance(targetImage.position, circle.hitRect.position);

                if (minDis <= distance && exDis >= distance && !isHighLight)
                {
                    OnClickSuccessEx().Forget();
                    comboNumText.text = combo.ToString();

                    ReturnCircle(circle.gameObject);
                    activeCircles.RemoveAt(i);
                }
                else if (exDis < distance && maxDis >= distance && !isHighLight)
                {
                    OnClickSuccess().Forget();
                    comboNumText.text = combo.ToString();
                    ReturnCircle(circle.gameObject);
                    activeCircles.RemoveAt(i);
                }
                else if (minDis > distance || maxDis < distance && !isHighLight)
                {
                    OnClickSuccessBad().Forget();
                    comboNumText.text = combo.ToString();
                    ReturnCircle(circle.gameObject);
                    activeCircles.RemoveAt(i);
                }
                else if (isHighLight)
                {
                    OnClickFever().Forget();
                    comboNumText.text = combo.ToString();
                    //ReturnCircle(circle.gameObject);
                    //activeCircles.RemoveAt(i);
                }

            }
        }
    }
    private void RanTextColor()
    {
        randomColor = new Color(Random.value, Random.value, Random.value);
        comboNumText.color = randomColor;
    }

    public GameObject GetCircle()
    {
        if (poolCircle.Length == 0)
        {
            return null;
        }

        GameObject circleDot = poolCircle[pivot];
        circleDot.transform.localScale = Vector3.one * circleBig;
        circleDot.transform.position = transform.position;
        circleDot.GetComponent<CircleMove>().Initialize(this);

        if (getDamage)
            circleDot.GetComponent<CircleMove>().ChangeColor().Forget();
        else
            circleDot.GetComponent<CircleMove>().SetColor();
        if (isHighLight)
        {
            image.color = feverColor;
            circleDot.GetComponent<CircleMove>().FeverTime();
        }
        else
        {
            image.color = originalColor;
            circleDot.GetComponent<CircleMove>().FeverTimeFIn();
        }

        circleDot.SetActive(true);

        activeCircles.Add(circleDot.GetComponent<CircleMove>());
        pivot = (pivot + 1) % poolCircle.Length;
        return circleDot;
    }
    public void ReturnCircle(GameObject circleDot)
    {
        if (circleDot == null || !circleDot.activeSelf) return;
        circleDot.GetComponent<CircleMove>().SetColor();
        circleDot.SetActive(false);
        circleDot.transform.localScale = Vector3.one * circleBig;
    }
    public async UniTask OnClickSuccess()
    {
        if (!isHighLight)
        {
            combo++;
        }
        RanTextColor();
        text.text = "Good";
        isScale = true;
        cg.alpha = 1;
        playerShoot.PlayerShoot_();
        //a.PlayOneShot(clip);
        DynamicPlayClip(clip);

        playerHPSc.PlayerHPPlus(2);
        await UniTask.Delay(100);
        isScale = false;
        await UniTask.Delay(300);
        cg.alpha = 0;
    }

    public async UniTask OnClickSuccessEx()
    {
        combo++;
        RanTextColor();
        text.text = "Perfect";
        isScale = true;
        cg.alpha = 1;
        //a.PlayOneShot(clip);
        DynamicPlayClip(clip);

        playerShoot.PlayerShoot_();
        playerHPSc.PlayerHPPlus(4);
        await UniTask.Delay(150);
        isScale = false;
        await UniTask.Delay(350);
        cg.alpha = 0;
    }

    public async UniTask OnClickSuccessBad()
    {
        combo = 0;
        RanTextColor();
        text.text = "Bad";
        isScale = true;
        cg.alpha = 1;
        await UniTask.Delay(150);
        isScale = false;
        await UniTask.Delay(350);
        cg.alpha = 0;
    }
    public async UniTask OnClickFever()
    {
        RanTextColor();
        text.text = "Fever!!";
        isScale = true;
        cg.alpha = 0;
        // a.PlayOneShot(clip);
        DynamicPlayClip(clip);
        playerShoot.PlayerShoot_();
        playerHPSc.PlayerHPPlus(1);
        await UniTask.Delay(150);
        isScale = false;
        await UniTask.Delay(350);
        cg.alpha = 0;
    }
    private bool isRunning = true;
    private async UniTask CircleGen()
    {
        while (isRunning && this != null && gameObject != null)
        {
            await UniTask.Delay((int)(secondsPerBeat * 1000.0));
            Debug.Log((int)(secondsPerBeat * 1000.0));
            if (!isRunning || this == null || gameObject == null)
                break;

            var circle = GetCircle();
            if (circle != null)
                circle.transform.position = transform.position;
        }
    }
    private void OnDisable()
    {
        isRunning = false;
    }

    private void OnDestroy()
    {
        isRunning = false;
    }
}
