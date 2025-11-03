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
        
    }

    public async UniTask ShowGameOverText(string message)
    {
       
        if (dialogueText == null) return;


        dialogueText.text = "";
        if (!gameObject.activeSelf) return; // 패널이 켜져 있을 때만 텍스트 시작
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
       
    }

    private void OnDestroy()
    {
        typingCTS?.Cancel();
    }
}
