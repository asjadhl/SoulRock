using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ChatState
{

}
public class BossTextManager: MonoBehaviour
{
    [SerializeField] private BossTextData bosstextData;
    [SerializeField] private BossTextUI bossUI;
    [SerializeField] private float interval = 3f;

    private CancellationTokenSource bossCTS;

    private async UniTask Start()
    {
        bossUI.ShowDialogueUI(false);
        bossUI.ShowDialogueUI2(false);

        // 보스 상태 감시 루프
        await CheckBossDeathLoopAsync();

        //_ = DelayedDialogueCheckAsync();
    }
    private async UniTask CheckBossDeathLoopAsync()
    {
        while (true)
        {
            if (BossState.isBoss1Dead)
            {
                await HandleBossDeathAsync(1, 3);
            }
            else if (BossState.isBoss2Dead)
            {
                await HandleBossDeathAsync(4, 7);
            }
            else if (BossState.isBoss3Dead)
            {
                await HandleBossDeathAsync(8, 8);
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
    private async UniTask HandleBossDeathAsync(int startStage, int endStage)
    {
        if (startStage == 1)
            BossState.isBoss1Dead = true;
        else if (startStage == 4)
            BossState.isBoss2Dead = true;
        else if (startStage == 8)
            BossState.isBoss3Dead = true;

        Debug.Log($"보스 사망 즉시 갱신됨: Boss1={BossState.isBoss1Dead}, Boss2={BossState.isBoss2Dead}, Boss3={BossState.isBoss3Dead}");
        bossUI.ShowDialogueUI(true);
        for (int i = startStage; i <= endStage; i++)
        {
            await StartStageDialogueAsync(i);
        }

        bossUI.ShowDialogueUI(false);
        bossUI.ShowDialogueUI2(false);

        await UniTask.Delay(1000);
        SceneManager.LoadScene("StageSelect");
    }

    public async UniTask DelayedDialogueCheckAsync()
    {
       
        await StartStageDialogueAsync(1);

        await StartStageDialogueAsync(2);

        await StartStageDialogueAsync(3);

        SceneManager.LoadScene("Main");
    }



    public async UniTask StartStageDialogueAsync(int stageNum)
    {
        // 기존 대화 취소
        bossCTS?.Cancel();
        bossCTS = new CancellationTokenSource();

        BossLine[] lines = stageNum switch
        {
            1 => bosstextData.act1.Bossdialogues,
            2 => bosstextData.act2.Bossdialogues,
            3 => bosstextData.act3.Bossdialogues,
            4 => bosstextData.act4.Bossdialogues,
            5 => bosstextData.act5.Bossdialogues,
            6 => bosstextData.act6.Bossdialogues,
            7 => bosstextData.act7.Bossdialogues,
            8 => bosstextData.act8.Bossdialogues,
            _ => null
        };

        if (lines != null)
            await PlayDialogueAsync(lines, stageNum, bossCTS.Token);
    }

    private async UniTask PlayDialogueAsync(BossLine[] lines, int stageNum, CancellationToken token)
    {
        int index = 0;
        bool waitingForClick = false;

        System.Action onClick = () => waitingForClick = false;
        bossUI.OnDialogueClick += onClick;

        bossUI.ShowDialogueUI(true);
        bossUI.StartImageAnimation(stageNum);

        while (index < lines.Length)
        {
            if (token.IsCancellationRequested) break;

            BossLine line = lines[index];
            bossUI.ShowDialogueBySpeaker(line);

            waitingForClick = true;

           
            float elapsed = 0f;
            while (waitingForClick && !token.IsCancellationRequested)
            {
                elapsed += Time.deltaTime;
                if (elapsed >= 3f) 
                    waitingForClick = false;

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            index++;
        }

        bossUI.OnDialogueClick -= onClick;
        bossUI.StopImageAnimation();
        bossUI.ShowDialogueUI(false);
        bossUI.speechBubble.SetActive(false);
    }



}
