using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("Scriptable Data")]
    [SerializeField] private TutorialTextData tutorialData;  // 모든 대사 통합
    [Header("UI")]
    [SerializeField] private TutorialUIManager TutoUI;
    [SerializeField] private GameObject nextButton;

    [Header("Dummy")]
    [SerializeField] private GameObject dummyTarget;
    [SerializeField] private Renderer dummyRenderer;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float highlightDuration = 10f;

    [Header("Settings")]
    [SerializeField] private float charInterval = 0.085f;

    private DummySpawner dummySpawner;
    private CancellationTokenSource tutorialCTS;
    private bool isTyping = false;
    private bool skipRequested = false;
    private bool waitingForNext = false;
    private bool dummyDeadTriggered = false;
    private Color originalColor;

    private void Start()
    {
        if (dummySpawner == null)
            dummySpawner = FindObjectOfType<DummySpawner>();

        if (nextButton != null)
        {
            var btn = nextButton.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnDialogueClicked);
            }
        }

        if (dummyRenderer != null)
            originalColor = dummyRenderer.material.color;

        StartTutorialAsync().Forget();
    }

    private async UniTaskVoid StartTutorialAsync()
    {
        tutorialCTS?.Cancel();
        tutorialCTS = new CancellationTokenSource();

        TutoUI.StartImageAnimation();

        // 1️tutline1 출력
        await PlayDialogueSetAsync(tutorialData.tutline1, tutorialCTS.Token);

        // 2️더미 관련 시퀀스 시작
        HighlightDummyAsync().Forget();
        await ShowDialogueAsync("더미를 맞춰보자! \n ( Hit the pile twice! )", null);

        // 3️더미 처치 대기
        await UniTask.WaitUntil(() => dummySpawner != null && dummySpawner.dummyHp <= 1);

        // 4️더미 죽으면 다음 세트 출력
        await OnDummyDeadSequence();
    }

    private async UniTask OnDummyDeadSequence()
    {
        if (dummyDeadTriggered) return;
        dummyDeadTriggered = true;

        TutoUI.StopImageAnimation();

        // tutline2 출력
        await PlayDialogueSetAsync(tutorialData.tutline2, tutorialCTS.Token);

        await UniTask.Delay(TimeSpan.FromSeconds(1));
        SceneManager.LoadScene("StageSelect");
    }

    private async UniTask PlayDialogueSetAsync(TutorialTextLine lineSet, CancellationToken token)
    {
        if (lineSet == null || lineSet.tutorialLines == null)
        {
            Debug.LogWarning("TutorialTextLine 세트가 비어있습니다!");
            return;
        }

        foreach (var line in lineSet.tutorialLines)
        {
            await PlayDialogueLineAsync(line, token);
        }
    }

    private async UniTask PlayDialogueLineAsync(TutorialLine line, CancellationToken token)
    {
        TutoUI.ShowDialogueUI(true);
        TutoUI.OnDialogueClick -= OnDialogueClicked;
        TutoUI.OnDialogueClick += OnDialogueClicked;

        await TypeDialogueAsync(line.text, line.sound, token);

        waitingForNext = true;
        if (nextButton != null) nextButton.SetActive(true);

        await UniTask.WaitUntil(() => waitingForNext == false, cancellationToken: token);
        TutoUI.OnDialogueClick -= OnDialogueClicked;
    }

    private async UniTask TypeDialogueAsync(string text, AudioClip clip, CancellationToken token)
    {
        TutoUI.ClearText();
        isTyping = true;

        for (int i = 0; i < text.Length; i++)
        {
            if (token.IsCancellationRequested) return;

            if (skipRequested)
            {
                TutoUI.AppendText(text.Substring(i));
                skipRequested = false;
                break;
            }

            TutoUI.AppendText(text[i].ToString());
            if (clip != null) TutoUI.PlaySound(clip);
            await UniTask.Delay(TimeSpan.FromSeconds(charInterval), cancellationToken: token);
        }

        isTyping = false;
    }

    private async UniTask ShowDialogueAsync(string text, AudioClip sound)
    {
        TutoUI.ShowDialogueUI(true);
        skipRequested = false;
        waitingForNext = false;

        TutoUI.OnDialogueClick -= OnDialogueClicked;
        TutoUI.OnDialogueClick += OnDialogueClicked;

        await TypeDialogueAsync(text, sound, tutorialCTS.Token);

        waitingForNext = true;
        if (nextButton != null) nextButton.SetActive(true);

        await UniTask.WaitUntil(() => waitingForNext == false, cancellationToken: tutorialCTS.Token);

        TutoUI.OnDialogueClick -= OnDialogueClicked;
    }

    private void OnDialogueClicked()
    {
        if (isTyping)
            skipRequested = true;
        else if (waitingForNext)
            waitingForNext = false;
    }

    private async UniTask HighlightDummyAsync()
    {
        if (dummyRenderer == null) return;

        float timer = 0f;
        while (timer < highlightDuration)
        {
            dummyRenderer.material.color = Color.Lerp(originalColor, highlightColor, Mathf.PingPong(timer * 4f, 1f));
            timer += Time.deltaTime;
            await UniTask.Yield();
        }
        dummyRenderer.material.color = originalColor;
    }

    private void OnDestroy()
    {
        tutorialCTS?.Cancel();
    }
}
