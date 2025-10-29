using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class GameOverTextUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Typing Effect")]
    [SerializeField] private AudioSource typingAudio;
    [SerializeField] private AudioClip typingClip;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    private CancellationTokenSource typingCTS;

    private void Awake()
    {
        // 시작 시 완전히 비활성화
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);
    }

    public async UniTask ShowGameOverText(string message)
    {
       
        if (dialogueText == null) return;


        dialogueText.text = "";
        dialogueText.gameObject.SetActive(true);

        typingCTS?.Cancel();
        typingCTS = new CancellationTokenSource();

        foreach (char c in message)
        {
            if (typingCTS.IsCancellationRequested) break;

            dialogueText.text += c;

            if (typingAudio != null && typingClip != null)
            {
                typingAudio.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y);
                typingAudio.PlayOneShot(typingClip);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(typingSpeed), cancellationToken: typingCTS.Token);
        }

        // 2초 유지 후 자동 숨김
        await UniTask.Delay(2000);
        dialogueText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        typingCTS?.Cancel();
    }
}
