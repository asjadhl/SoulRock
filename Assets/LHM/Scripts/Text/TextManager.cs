using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class TalkState
{
       public static bool isTalking = false;
}
public class TextManager : MonoBehaviour
{
    [SerializeField] private BossTextData bosstextData;
    [SerializeField] private StageDialogueData dialogueData;
    [SerializeField] private DialogueUIManager dialogueUI;
    [SerializeField] private float interval = 3f;

    private CancellationTokenSource dialogueCTS;
    private CancellationTokenSource bossCTS;
    private void Start()
    {
        //if (dialogueUI != null)
        //{
        //    dialogueUI.ShowDialogueUI(true);  
        //}
    }
    public async UniTask DelayedDialogueCheckAsync()
    {
        if (!BossState.isBoss1Dead && !BossState.isBoss2Dead)
        {
            await StartStageDialogueAsync(2);
        }
        if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
        {
            await StartStageDialogueAsync(3);
        }
    }

        
    public async UniTask StartStageDialogueAsync(int stageNum)
    {
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
            await firstPlayDialogueAsync(lines, stageNum, dialogueCTS.Token);
    }
    //private async UniTask BossHandleBossDeathAsync(int firstStage, int endStage)
    //{
    //    if (firstStage == 1)
    //        BossState.isBoss1Dead = true;
    //    else if (firstStage == 4)
    //        BossState.isBoss2Dead = true;
    //    else if (firstStage == 8)
    //        BossState.isBoss3Dead = true;

    //    dialogueUI.ShowDialogueUI(true);
    //    for (int i = firstStage; i <= endStage; i++)
    //    {
    //        await BossStartStageDialogueAsync(i);
    //    }

    //    dialogueUI.ShowDialogueUI(false);
    //}
    //public async UniTask BossStartStageDialogueAsync(int firstStage)
    //{
    //    bossCTS?.Cancel();
    //    bossCTS = new CancellationTokenSource();

    //    BossLine[] lines = firstStage switch
    //    {
    //        1 => bosstextData.act1.Bossdialogues,
    //        2 => bosstextData.act2.Bossdialogues,
    //        3 => bosstextData.act3.Bossdialogues,
    //        4 => bosstextData.act4.Bossdialogues,
    //        5 => bosstextData.act5.Bossdialogues,
    //        6 => bosstextData.act6.Bossdialogues,
    //        7 => bosstextData.act7.Bossdialogues,
    //        8 => bosstextData.act8.Bossdialogues,
    //        _ => null
    //    };
    //    if (lines != null)
    //        await stagePlayDialogueAsync(lines, firstStage, bossCTS.Token);
    //}
    private async UniTask firstPlayDialogueAsync(DialogueLine[] lines, int stageNum, CancellationToken token)
    {
        TalkState.isTalking = true;   
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

        TalkState.isTalking = false;
    }
    //private async UniTask stagePlayDialogueAsync(BossLine[] lines, int stageNum, CancellationToken token)
    //{
    //    int index = 0;
    //    bool waitingForClick = false;

    //    System.Action onClick = () => waitingForClick = false;
    //    dialogueUI.OnDialogueClick += onClick;

    //    dialogueUI.ShowDialogueUI(true);
    //    dialogueUI.StartImageAnimation(stageNum);

    //    while (index < lines.Length)
    //    {
    //        if (token.IsCancellationRequested) break;

    //        BossLine line = lines[index];
    //        dialogueUI.ShowDialogueText(line.text, line.sound);

    //        waitingForClick = true;
    //        while (waitingForClick && !token.IsCancellationRequested)
    //            await UniTask.Yield(PlayerLoopTiming.Update, token);

    //        index++;
    //    }

    //    dialogueUI.OnDialogueClick -= onClick;
    //    dialogueUI.StopImageAnimation();
    //    dialogueUI.ShowDialogueUI(false);
    //    dialogueUI.speechBubble.SetActive(false);


    //}
}
