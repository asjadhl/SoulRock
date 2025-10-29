using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDieText : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TutorialUIManager DieUi;
    [SerializeField] private GameObject nextButton;

    [Header("Settings")]
    [SerializeField] private float charInterval = 0.085f;
    [SerializeField] private float imageInterval = 0.5f;

    public bool IsGen = false;
    private CancellationTokenSource PlayerDieCTS;
    private bool isTyping = false;
    private bool skipRequested = false;
    private bool waitingForNext = false;
    private Color originalColor;

    private void Start()
    {
        PlayerDieCTS = new CancellationTokenSource();
        OnDeadSequence(3);
    }

    private async UniTaskVoid OnDeadSequence(int num)
    {
        //int num에 받은 숫자중 1~num 사이의 랜덤 숫자 선택
        int randomNum = UnityEngine.Random.Range(1, num);
        Debug.Log("PlayerDieText Random Num: " + randomNum);
        switch (randomNum)
        {
            case 1:
                await ShowDialogueAsync("Boo.. 포기하지마.. 잭슨..", null);
                break;
            case 2:
                await ShowDialogueAsync("Boo.. 기다릴게.. 다시 도전해줘..", null);
                break;
            case 3:
                await ShowDialogueAsync("다시 한번 마음을 가다듬어봐...", null);
                break;
        }
    }
   
    private async UniTask TypeDialogueAsync(string text, AudioClip clip, CancellationToken token)
    {
        DieUi.ClearText();
        isTyping = true;

        for (int i = 0; i < text.Length; i++)
        {
            if (token.IsCancellationRequested) return;

            if (skipRequested)
            {
                // 스킵: 남은 글자 즉시 출력
                DieUi.AppendText(text.Substring(i));
                skipRequested = false;
                break;
            }

            DieUi.AppendText(text[i].ToString());
            if (clip != null) DieUi.PlaySound(clip);
            await UniTask.Delay(TimeSpan.FromSeconds(charInterval), cancellationToken: token);
        }

        isTyping = false;
    }
    private async UniTask ShowDialogueAsync(string text, AudioClip sound)
    {


        DieUi.ShowDialogueUI(true);
        skipRequested = false;
        waitingForNext = false;

        // UI 내부 클릭(말풍선 등) 이벤트 연결
        DieUi.OnDialogueClick -= OnDialogueClicked;
        DieUi.OnDialogueClick += OnDialogueClicked;

        await TypeDialogueAsync(text, sound, PlayerDieCTS.Token);
        Debug.Log("Token Cancelled? " + PlayerDieCTS.Token.IsCancellationRequested);

        waitingForNext = true;
        if (nextButton != null) nextButton.SetActive(true);

        await UniTask.WaitUntil(() => waitingForNext == false, cancellationToken: PlayerDieCTS.Token);

        DieUi.OnDialogueClick -= OnDialogueClicked;
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
    private void OnDestroy()
    {
        PlayerDieCTS?.Cancel();
    }
}
