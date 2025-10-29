using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;  // 대화 텍스트
    public RawImage ghostImage;              // 유령 이미지
    public AudioSource audioSource;       // 대사 사운드 재생용
    public GameObject nextButton;         // [다음] 버튼
    public GameObject speechBubble;       // 말풍선 UI (선택적으로 꺼줄 수 있음)
    private CancellationTokenSource imageCTS;
    private CancellationTokenSource typingCTS;
    private bool isAnimating = false;
    private bool isTyping = false;
    private string currentFullText = "";
    public event Action OnDialogueClick;
    [SerializeField] private AudioSource typingAudio;
    [SerializeField] private bool randomPitch = true;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    [Header("스테이지별 이미지 세트")]
    public Texture stage1_img1;
    public Texture stage1_img2;

    [Header("타이핑 설정")]
    [SerializeField] private float typingSpeed = 0.04f;
    private void Start()
    {

    }

    // UI 표시/숨기기
    public void ShowDialogueUI(bool active)
    {
        if (speechBubble != null)
            speechBubble.SetActive(active);
        if (ghostImage != null)
            ghostImage.gameObject.SetActive(active);
        
    }

    // 텍스트 초기화
    public void ClearText()
    {
        if (dialogueText != null)
            dialogueText.text = "";
    }
    public void ShowDialogueText(string message, AudioClip clip = null)
    {
        typingCTS?.Cancel();
        typingCTS = new CancellationTokenSource();
        _ = TypeTextAsync(message, clip, typingCTS.Token);
    }
    // 텍스트 추가
    public void AppendText(string letter)
    {
        if (dialogueText != null)
            dialogueText.text += letter;
    }
    private async UniTask TypeTextAsync(string message, AudioClip clip, CancellationToken token)
    {
        isTyping = true;
        currentFullText = message;
        dialogueText.text = "";

        foreach (char c in message)
        {
            if (token.IsCancellationRequested) break;

            dialogueText.text += c;

            if (typingAudio != null && clip != null)
            {
                typingAudio.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y);
                typingAudio.PlayOneShot(clip);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(typingSpeed), cancellationToken: token);
        }

        isTyping = false;
    }
    // 효과음 재생
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // 이미지 애니메이션 시작
    public void StartImageAnimation()
    {
        imageCTS?.Cancel();
        imageCTS = new CancellationTokenSource();

        Texture img1 = null, img2 = null;
        
        img1 = stage1_img1; img2 = stage1_img2;
     

        _ = ChangeImageLoopAsync(img1, img2, imageCTS.Token);
    }
    private async UniTaskVoid ChangeImageLoopAsync(Texture img1, Texture img2, CancellationToken token)
    {
        if (ghostImage == null || img1 == null || img2 == null)
            return;

        while (!token.IsCancellationRequested)
        {
            ghostImage.texture = img1;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
            if (token.IsCancellationRequested) break;

            ghostImage.texture = img2;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
        }
    }

    public void StopImageAnimation()
    {
        imageCTS?.Cancel();
    }
    // 버튼 클릭 시 실행 (TutorialManager에서 이벤트 등록 가능)
    public void OnNextButtonClick()
    {
        OnDialogueClick?.Invoke();
    }
}
