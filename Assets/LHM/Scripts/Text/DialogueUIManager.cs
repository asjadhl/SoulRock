using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogueUIManager : MonoBehaviour
{
    [Header("UI 오브젝트 연결")]
    [SerializeField] public GameObject speechBubble;
    [SerializeField] private RawImage speakerImage;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("폰트 (SDF 폰트 에셋)")]
    [SerializeField] private TMP_FontAsset sdfFontAsset;

    [Header("타이핑 설정")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("사운드 설정")]
    [SerializeField] private AudioSource typingAudio;
    [SerializeField] private bool randomPitch = true;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private Coroutine typingCoroutine;
    private Coroutine imageChangeRoutine;
    private bool isTyping = false;
    private string currentFullText = "";

    // 클릭 시 다음 대사로 넘기기 위한 이벤트
    public event Action OnDialogueClick;

    //  스테이지별 두 장의 이미지 (a1, a2)
    [Header("스테이지별 이미지 세트")]
    public Texture stage1_img1;
    public Texture stage1_img2;
    public Texture stage2_img1;
    public Texture stage2_img2;
    public Texture stage3_img1;
    public Texture stage3_img2;
    public Texture boss_img1;
    public Texture boss_img2;
    public Texture boss_img3;
    public Texture boss_img4;

    void Start()
    {
        if (sdfFontAsset != null && dialogueText != null)
            dialogueText.font = sdfFontAsset;

        // 말풍선 클릭 이벤트 등록
        if (speechBubble != null)
        {
            Button btn = speechBubble.GetComponent<Button>();
            if (btn == null)
                btn = speechBubble.AddComponent<Button>();

            btn.onClick.AddListener(OnSpeechBubbleClick);
        }
    }

    private void OnSpeechBubbleClick()
    {
        if (isTyping)
        {
            // 현재 문장 즉시 완성
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentFullText;
            isTyping = false;
        }
        else
        {
            // 이미 다 나왔으면 다음 문장으로 스킵
            OnDialogueClick?.Invoke();
        }
    }

    public void ShowDialogueText(string message, AudioClip clip = null)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message, clip));
    }

    private IEnumerator TypeText(string message, AudioClip clip)
    {
        isTyping = true;
        currentFullText = message;
        dialogueText.text = "";

        foreach (char c in message)
        {
            dialogueText.text += c;

            if (typingAudio != null && clip != null)
            {
                typingAudio.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y);
                typingAudio.PlayOneShot(clip);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void ShowDialogueUI(bool isOn)
    {
        if (speechBubble != null)
            speechBubble.SetActive(isOn);
        if (speakerImage != null)
            speakerImage.gameObject.SetActive(isOn);
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(isOn);
    }



/// <summary>
///  스테이지별 유령 이미지 2장을 3초 간격으로 바꾸기
/// </summary>
public void StartImageAnimation(int stageNum)
    {
        if (imageChangeRoutine != null)
            StopCoroutine(imageChangeRoutine);

        Texture img1 = null, img2 = null;

        switch (stageNum)
        {
            case 1: img1 = stage1_img1; img2 = stage1_img2; break;
            case 2: img1 = stage2_img1; img2 = stage2_img2; break;
            case 3: img1 = stage3_img1; img2 = stage3_img2; break;
            case 4: img1 = boss_img1; img2 = boss_img2; break;
            case 5: img1 = boss_img3; img2 = boss_img4; break;
        }

        imageChangeRoutine = StartCoroutine(ChangeImageLoop(img1, img2));
    }

    /// <summary>
    ///  3초마다 번갈아 표시되는 루프
    /// </summary>
    private IEnumerator ChangeImageLoop(Texture img1, Texture img2)
    {
        if (speakerImage == null || img1 == null || img2 == null)
            yield break;

        while (true)
        {
            speakerImage.texture = img1;
            yield return new WaitForSeconds(0.5f);
            speakerImage.texture = img2;
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    ///  대화 종료 시 이미지 애니메이션 중단
    /// </summary>
    public void StopImageAnimation()
    {
        if (imageChangeRoutine != null)
            StopCoroutine(imageChangeRoutine);
      
        
    }
}
