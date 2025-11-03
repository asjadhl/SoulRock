// TutorialManager.cs
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("Scriptable Data")]
    [SerializeField] private TutorialTextLine tutorialTextLine;

    [Header("UI")]
    [SerializeField] private TutorialUIManager TutoUI;
    [SerializeField] private GameObject nextButton;

    [Header("Target Dummy")]
    [SerializeField] private GameObject dummyTarget;
    [SerializeField] private Renderer dummyRenderer;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float highlightDuration = 10f;

    [Header("Settings")]
    [SerializeField] private float charInterval = 0.085f;
    [SerializeField] private float imageInterval = 0.5f;

    public bool IsGen = false;
    DummySpawner dummySpawner;
    private CancellationTokenSource tutorialCTS;
    private bool isTyping = false;
    private bool skipRequested = false;
    private bool waitingForNext = false;
    private Color originalColor;
    private void Update()
    {
        if (dummySpawner == null)
            Debug.LogError("dummySpawner가 NULL임!");
        else
            Debug.Log($"dummyHp 현재값: {dummySpawner.dummyHp}");
    }

    private void Start()
    {


        if (dummySpawner == null)
            dummySpawner = FindObjectOfType<DummySpawner>();


        //  시작 시 버튼은 숨김 + 리스너 연결
        if (nextButton != null)
        {
            var btn = nextButton.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnDialogueClicked); // 버튼도 대화 클릭과 동일 처리
            }
        }

        // 하이라이트 원색 저장
        if (dummyRenderer != null)
        {
            if (dummyRenderer.material != null) dummyRenderer.material.color = Color.white;
            originalColor = dummyRenderer.material.color;
        }

        _ = StartTutorialAsync();
       
    }
    private async Task HandleDummyDead()
    {
        Debug.Log("더미 사망 감지됨 → 다음 대사 진행");
        await OnDummyDeadSequence();
    }
    private async UniTask AttackDummyDeadSequence()
    {

        HighlightDummyAsync().Forget();
        await ShowDialogueAsync("자.. 이제 저 빛나는 더미를 맞춰봐... \r\n", null);
        await ShowDialogueAsync("Boo! 딱 2대만 맞춰볼까..", null);
        if (dummySpawner.dummyHp <= 1)
            HandleDummyDead();
        
    }
    private async UniTask OnDummyDeadSequence()
    {
        await ShowDialogueAsync("만약 에임점에 원을 못맞추면... 시간이 지나면서.. 체력이 계속 깎이게 돼... 조심해야해... \r\n", null);
        await ShowDialogueAsync("Boo! 공격을 맞거나 시간이 지나서 체력이 0이 되면 게임오버야...", null);
        await ShowDialogueAsync("그리고... 모든 스테이지는 보스의 공격을 버텨야해...", null);
        await ShowDialogueAsync("...", null);
        await ShowDialogueAsync("그게다야... ", null);
        await ShowDialogueAsync("이제 준비가 다 된 것 같네... 자, 그럼 본격적으로 시작해볼까? ", null);
        TutoUI.StopImageAnimation();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        SceneManager.LoadScene("StageSelect");
    }
    private async UniTask StartTutorialAsync()
    {
        

        tutorialCTS?.Cancel();
        tutorialCTS = new CancellationTokenSource();

     

        // 2) 유령 이미지 애니메이션 시작
        TutoUI.StartImageAnimation();

        // (필요하면) 추가 대사를 더 출력하려면 여기서 루프 돌리면 됨
        for (int i = 0; i < tutorialTextLine.tutorialLines.Length; i++)
        {
            await PlayDialogueLineAsync(tutorialTextLine.tutorialLines[i], tutorialCTS.Token);
            Debug.Log(i);
        }


        await AttackDummyDeadSequence();
        
        
    }
    private async UniTask PlayDialogueLineAsync(TutorialLine line, CancellationToken token)
    {
        TutoUI.ShowDialogueUI(true);

        // 클릭 이벤트 연결 (UI 내부 클릭용)
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
                // 스킵: 남은 글자 즉시 출력
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

    private void OnDialogueClicked()
    {
        if (isTyping)
        {
            // 타이핑 중 → 스킵
            skipRequested = true;
        }
        else if (waitingForNext)
        {
            // 모두 출력됨 → 다음으로 진행
            waitingForNext = false;
        }
    }

    private async UniTask ShowDialogueAsync(string text, AudioClip sound)
    {
        Debug.Log(dummySpawner == null ? "DummySpawner is NULL" : "DummySpawner 연결됨");

        TutoUI.ShowDialogueUI(true);
        skipRequested = false;
        waitingForNext = false;

        // UI 내부 클릭(말풍선 등) 이벤트 연결
        TutoUI.OnDialogueClick -= OnDialogueClicked;
        TutoUI.OnDialogueClick += OnDialogueClicked;

        await TypeDialogueAsync(text, sound, tutorialCTS.Token);
        Debug.Log("Token Cancelled? " + tutorialCTS.Token.IsCancellationRequested);

        waitingForNext = true;
        if (nextButton != null) nextButton.SetActive(true);

        await UniTask.WaitUntil(() => waitingForNext == false, cancellationToken: tutorialCTS.Token);

        TutoUI.OnDialogueClick -= OnDialogueClicked;
    }

    private async UniTask HighlightDummyAsync()
    {
        if (dummyRenderer == null) return;

        float timer = 0f;
        while (timer < highlightDuration)
        {
            dummyRenderer.material.color =
                Color.Lerp(originalColor, highlightColor, Mathf.PingPong(timer * 4f, 1f));
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
