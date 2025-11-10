using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;

public class TutorialManager : MonoBehaviour
{
    [Header("Scriptable Data")]
    [SerializeField] private BossTextData tutorialData;

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
        originalColor = Color.white;

        StartTutorialAsync().Forget();
    }

    private async UniTaskVoid StartTutorialAsync()
    {
        tutorialCTS?.Cancel();
        tutorialCTS = new CancellationTokenSource();

        TutoUI.StartImageAnimation();

        if (tutorialData.bossDialogues.Length > 0)
            await PlayDialogueSetAsync(tutorialData.bossDialogues[0].deathLines, tutorialCTS.Token);

        HighlightDummyAsync().Forget();

        await UniTask.WaitUntil(() => dummySpawner != null && dummySpawner.dummyHp <= 1);



        await OnDummyDeadSequence();
    }

    private async UniTask OnDummyDeadSequence()
    {
        if (dummyDeadTriggered) return;
        dummyDeadTriggered = true;

        TutoUI.StopImageAnimation();

        if (tutorialData.bossDialogues.Length > 1)
            await PlayDialogueSetAsync(tutorialData.bossDialogues[1].deathLines, tutorialCTS.Token);

        await UniTask.Delay(TimeSpan.FromSeconds(1));
        PlayerPrefs.SetInt("IsTutorial",1); //Declare No More Tutorial till the game is Application Quit
        SceneLoader.Instance.LoadScene("StageSelect");
    }

    private async UniTask PlayDialogueSetAsync(BossLine[] lineSet, CancellationToken token)
    {
        if (lineSet == null || lineSet.Length == 0)
        {
            return;
        }

        foreach (var line in lineSet)
        {
            await PlayDialogueLineAsync(line, token);
        }
    }

    private async UniTask PlayDialogueLineAsync(BossLine line, CancellationToken token)
    {
        TutoUI.ShowDialogueUI(true);
        TutoUI.OnDialogueClick -= OnDialogueClicked;
        TutoUI.OnDialogueClick += OnDialogueClicked;

        await TypeLocalizedDialogueAsync(line.localizationTable, line.localizationKey, line.sound, token);

        waitingForNext = true;
        if (nextButton != null) nextButton.SetActive(true);

        await UniTask.WaitUntil(() => waitingForNext == false, cancellationToken: token);
        TutoUI.OnDialogueClick -= OnDialogueClicked;
    }

    private async UniTask TypeLocalizedDialogueAsync(string table, string key, AudioClip clip, CancellationToken token)
    {
        TutoUI.ClearText();
        isTyping = true;

        var localizedString = new LocalizedString(table, key);
        string text = await localizedString.GetLocalizedStringAsync();

        if (string.IsNullOrEmpty(text))
        {
            text = key; 
        }

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

    // 튜토리얼 전용 단일 문자열 호출 (보스라인 외)
    private async UniTask ShowLocalizedDialogueAsync(string table, string key, AudioClip clip)
    {
        TutoUI.ShowDialogueUI(true);
        skipRequested = false;
        waitingForNext = false;

        TutoUI.OnDialogueClick -= OnDialogueClicked;
        TutoUI.OnDialogueClick += OnDialogueClicked;

        await TypeLocalizedDialogueAsync(table, key, clip, tutorialCTS.Token);

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
