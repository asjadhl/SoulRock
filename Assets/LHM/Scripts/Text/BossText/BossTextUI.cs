using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossTextUI : MonoBehaviour
{
    [Header("UI 오브젝트 연결")]
    [SerializeField] public GameObject speechBubble;
    [SerializeField] private RawImage speakerImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject PlayerBubble;
    [SerializeField] private TextMeshProUGUI PlayerText;
    [Header("폰트 (SDF 폰트 에셋)")]
    [SerializeField] private TMP_FontAsset sdfFontAsset;

    [Header("타이핑 설정")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("사운드 설정")]
    [SerializeField] private AudioSource typingAudio;
    [SerializeField] private bool randomPitch = true;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private CancellationTokenSource typingCTS;
    private CancellationTokenSource imageCTS;

    private bool isTyping = false;
    private string currentFullText = "";

    public event Action OnDialogueClick;
    BossTextManager bossTextManager;

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
        //if (sdfFontAsset != null && dialogueText != null)
        //    dialogueText.font = sdfFontAsset;
        
        if (speechBubble != null)
        {
            Button btn = speechBubble.GetComponent<Button>();
            if (btn == null)
                btn = speechBubble.AddComponent<Button>();

            btn.onClick.AddListener(OnSpeechBubbleClick);
        }
        if(PlayerBubble != null)
        {
            Button ptn = PlayerBubble.GetComponent<Button>();
            if (ptn == null)
                ptn = PlayerBubble.AddComponent<Button>();
            ptn.onClick.AddListener(OnSpeechBubbleClick);
        }
    }
    

    private void OnSpeechBubbleClick()
    {
        if (isTyping)
        {
            typingCTS?.Cancel();
            dialogueText.text = currentFullText;
            isTyping = false;
        }
        else
        {
            OnDialogueClick?.Invoke();
        }
    }

    public void ShowDialogueBySpeaker(BossLine line)
    {
        typingCTS?.Cancel();
        typingCTS = new CancellationTokenSource();

        if (line.speaker == "Player")
        {
            ShowDialogueUI(false);
            ShowDialogueUI2(true); 
            _ = TypeTextAsyncTo(PlayerText, line.text, line.sound, typingCTS.Token);
        }
        else
        {
            ShowDialogueUI(true);
            ShowDialogueUI2(false);
            _ = TypeTextAsyncTo(dialogueText, line.text, line.sound, typingCTS.Token);
        }
    }

    private async UniTask TypeTextAsyncTo(TextMeshProUGUI targetText, string message, AudioClip clip, CancellationToken token)
    {
        isTyping = true;
        currentFullText = message;
        targetText.text = "";

        foreach (char c in message)
        {
            if (token.IsCancellationRequested) break;

            targetText.text += c;

            if (typingAudio != null && clip != null)
            {
                typingAudio.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y);
                typingAudio.PlayOneShot(clip);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(typingSpeed), cancellationToken: token);
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
    public void ShowDialogueUI2(bool isOn)
    {
        if (PlayerBubble != null)
            PlayerBubble.SetActive(isOn);
        if (PlayerText != null)
            PlayerText.gameObject.SetActive(isOn);
    }
    public void StartImageAnimation(int stageNum)
    {
        imageCTS?.Cancel();
        imageCTS = new CancellationTokenSource();

        Texture img1 = null, img2 = null;
        switch (stageNum)
        {
            case 1: img1 = stage1_img1; img2 = stage1_img2; break;
            case 2: 
            case 3: img1 = stage2_img1; img2 = stage2_img2; break;
            case 4: img1 = stage3_img1; img2 = stage3_img2; break;
            case 5:
            case 6: img1 = stage3_img1; img2 = stage3_img2; break;
            case 7:
            case 8: img1 = stage2_img1; img2 = stage2_img2; break;
        }

        _ = ChangeImageLoopAsync(img1, img2, imageCTS.Token);
    }

    private async UniTaskVoid ChangeImageLoopAsync(Texture img1, Texture img2, CancellationToken token)
    {
        if (speakerImage == null || img1 == null || img2 == null)
            return;

        while (!token.IsCancellationRequested)
        {
            speakerImage.texture = img1;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
            if (token.IsCancellationRequested) break;

            speakerImage.texture = img2;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
        }
    }

    public void StopImageAnimation()
    {
        imageCTS?.Cancel();
    }
}
