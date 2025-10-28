using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private StageDialogueData dialogueData;
    [SerializeField] private DialogueUIManager dialogueUI;
    [SerializeField] private float interval = 3f;

    private CancellationTokenSource dialogueCTS;

    private void Start()
    {
        _ = DelayedDialogueCheckAsync();
    }

    public async UniTaskVoid DelayedDialogueCheckAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

        if (MainGhostTrainingState.isClicked)
            StartStageDialogueAsync(1).Forget();
        if (MainPlayState.isClicked1)
            StartStageDialogueAsync(2).Forget();
        if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
            StartStageDialogueAsync(3).Forget();
        if (MapSelected3.start3)
            StartStageDialogueAsync(4).Forget();
        if (MapSelected3.stop3)
            StartStageDialogueAsync(5).Forget();
    }

    public async UniTaskVoid StartStageDialogueAsync(int stageNum)
    {
        // 기존 대화 취소
        dialogueCTS?.Cancel();
        dialogueCTS = new CancellationTokenSource();

        DialogueLine[] lines = stageNum switch
        {
            1 => dialogueData.stage1.dialogues,
            2 => dialogueData.stage2.dialogues,
            3 => dialogueData.stage3.dialogues,
            4 => dialogueData.stage4.dialogues,
            5 => dialogueData.stage5.dialogues,
            _ => null
        };

        if (lines != null)
            await PlayDialogueAsync(lines, stageNum, dialogueCTS.Token);
    }

    private async UniTask PlayDialogueAsync(DialogueLine[] lines, int stageNum, CancellationToken token)
    {
        // 대화 시작 플래그 설정
        switch (stageNum)
        {
            case 1: DialogueLineTrueORFalse.TutorialTrue = true; break; //트레이닝 룸 대사
            case 2: DialogueLineTrueORFalse.stage1True = true; break; //광대 설명
            case 3: DialogueLineTrueORFalse.stage2True = true; break; //해골 설명
            case 4: DialogueLineTrueORFalse.stage3_1True = true; break; //박살내기 전
            case 5: DialogueLineTrueORFalse.stage3_2True = true; break; //박살낸 후
        }

        int index = 0;
        bool waitingForClick = false;

        System.Action onClick = () => waitingForClick = false;
        dialogueUI.OnDialogueClick += onClick;

        dialogueUI.ShowDialogueUI(true);
        dialogueUI.StartImageAnimation(stageNum);

        while (index < lines.Length)
        {
            if (token.IsCancellationRequested) break;

            DialogueLine line = lines[index];
            dialogueUI.ShowDialogueText(line.text, line.sound);

            waitingForClick = true;
            while (waitingForClick && !token.IsCancellationRequested)
                await UniTask.Yield(PlayerLoopTiming.Update, token);

            index++;
        }

        dialogueUI.OnDialogueClick -= onClick;
        dialogueUI.StopImageAnimation();
        dialogueUI.ShowDialogueUI(false);
        dialogueUI.speechBubble.SetActive(false);

        // 대화 종료 플래그 해제
        switch (stageNum)
        {
            case 1: DialogueLineTrueORFalse.TutorialTrue = false; break;
            case 2: DialogueLineTrueORFalse.stage1True = false; break;
            case 3: DialogueLineTrueORFalse.stage2True = false; break;
            case 4: DialogueLineTrueORFalse.stage3_1True = false; break;
            case 5: DialogueLineTrueORFalse.stage3_2True = false; break;
        }
    }
}
