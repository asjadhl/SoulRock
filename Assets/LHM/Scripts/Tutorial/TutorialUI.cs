using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUIS : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;      // 대화 전체 패널 (말풍선 등)
    public TextMeshProUGUI dialogueText;  // 대화 텍스트
    public Image ghostImage;              // 유령 이미지
    public AudioSource audioSource;       // 대사 사운드 재생용
    public GameObject nextButton;         // [다음] 버튼
    public GameObject speechBubble;       // 말풍선 UI (선택적으로 꺼줄 수 있음)

    [Header("Ghost Animation")]
    public Sprite[] ghostSprites;         // 교체될 유령 이미지 배열
    private bool isAnimating = false;

    public event Action OnDialogueClick;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (speechBubble != null)
            speechBubble.SetActive(false);
        if (nextButton != null)
            nextButton.SetActive(false);
    }

    // UI 표시/숨기기
    public void ShowDialogueUI(bool active)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(active);
        if (speechBubble != null)
            speechBubble.SetActive(active);
    }

    // 텍스트 초기화
    public void ClearText()
    {
        if (dialogueText != null)
            dialogueText.text = "";
    }

    // 텍스트 추가
    public void AppendText(string letter)
    {
        if (dialogueText != null)
            dialogueText.text += letter;
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
    public async void StartImageAnimation(float interval)
    {
        if (ghostSprites == null || ghostSprites.Length == 0 || ghostImage == null)
            return;

        isAnimating = true;
        int index = 0;

        while (isAnimating)
        {
            ghostImage.sprite = ghostSprites[index];
            index = (index + 1) % ghostSprites.Length;
            await UniTask.Delay(TimeSpan.FromSeconds(interval));
        }
    }

    // 이미지 애니메이션 정지
    public void StopImageAnimation()
    {
        isAnimating = false;
    }

    // 버튼 클릭 시 실행 (TutorialManager에서 이벤트 등록 가능)
    public void OnNextButtonClick()
    {
        OnDialogueClick?.Invoke();
    }
}
