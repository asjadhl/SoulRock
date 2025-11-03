using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;

public static class TalkState
{
       public static bool isTalking = false;
}
public class TextManager : MonoBehaviour
{
    [SerializeField] private BossTextData bosstextData;
    [SerializeField] private StageDialogueData dialogueData;//여기 stage1에 대사 입력하시면되요
    [SerializeField] private DialogueUIManager dialogueUI;
    [SerializeField] private GameObject BossClosePanel;

    private CancellationTokenSource dialogueCTS;
    private CancellationTokenSource bossCTS;

    [SerializeField] ParticleSystem bossDeadParticle;
    [SerializeField] GameObject boss;

    private void Start()
    {
        //MovieDialogueAsync().Forget();
        //if (dialogueUI != null)
        //{
        //    dialogueUI.ShowDialogueUI(true);  
        //}
        
    }
    public async UniTask BossDialogueCheackAsync()
    {
        if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
        {
            BossClosePanel.SetActive(true);
            await BossHandleBossDeathAsync(1, 3);
            await PlayDeadParteicle();
            Destroy(boss);
            await UniTask.Delay(2000);
            SceneManager.LoadScene("StageSelect");
        }
        if (BossState.isBoss1Dead && BossState.isBoss2Dead)
        {
            await BossHandleBossDeathAsync(4, 7);
            await PlayDeadParteicle();
            Destroy(boss);
            await UniTask.Delay(2000);
            SceneManager.LoadScene("StageSelect");
        }
        if (BossState.isBoss3Dead)
        {
            await BossHandleBossDeathAsync(8, 8);
            await PlayDeadParteicle();
            Destroy(boss);
            await UniTask.Delay(2000);
            SceneManager.LoadScene("Main");
        }
    }
    public async UniTask DelayedDialogueCheckAsync()//보스 1,2
    {
        if (!BossState.isBoss1Dead && !BossState.isBoss2Dead)
        {
            await StartStageDialogueAsync(1);
        }
        if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
        {
            await StartStageDialogueAsync(4);
        }
        if (!BossState.isBoss3Dead)
        {
            await StartStageDialogueAsync(8);
        }
    }
    public async UniTask ClosePanel()
    {
        BossClosePanel.SetActive(true);
        
    }
    public async UniTask MovieDialogueAsync()//<- 이거 movie씬 대사 처리
    {
        dialogueCTS?.Cancel();
        dialogueCTS = new CancellationTokenSource();//여기까지 기존 대사 취소및 초기화
        DialogueLine[] lines = dialogueData.stage1.dialogues;// 그 데이터 stage1에 있는 대사 불러옴
        if (lines != null)
            await firstPlayDialogueAsync(lines, 0, dialogueCTS.Token);//출력
    }

    public async UniTask StartStageDialogueAsync(int stageNum)
    {
        dialogueCTS?.Cancel();
        dialogueCTS = new CancellationTokenSource();

        DialogueLine[] lines = stageNum switch//스테이지별 대사들
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
    private async UniTask BossHandleBossDeathAsync(int firstStage, int endStage)
    {
        if (firstStage == 1)
            BossState.isBoss1Dead = true;
        else if (firstStage == 4)
            BossState.isBoss2Dead = true;
        else if (firstStage == 8)
            BossState.isBoss3Dead = true;

        dialogueUI.ShowDialogueUI(true);
        for (int i = firstStage; i <= endStage; i++)
        {
            await BossStartStageDialogueAsync(i);
        }

        
    }
    public async UniTask BossStartStageDialogueAsync(int firstStage)
    {
        bossCTS?.Cancel();
        bossCTS = new CancellationTokenSource();

        BossLine[] lines = firstStage switch
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
            await stagePlayDialogueAsync(lines, firstStage, bossCTS.Token);
    }
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
            var localized = new LocalizedString(line.localizationTable, line.localizationKey);
            string localizedText = await localized.GetLocalizedStringAsync();

            dialogueUI.ShowDialogueText(localizedText, line.sound);

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
    private async UniTask stagePlayDialogueAsync(BossLine[] lines, int stageNum, CancellationToken token)
    {
        int index = 0;
        bool waitingForClick = false;

        System.Action onClick = () => waitingForClick = false;
        dialogueUI.OnDialogueClick += onClick;

        dialogueUI.ShowDialogueUI(true);
        dialogueUI.BossImageAnimation(stageNum);

        while (index < lines.Length)
        {
            if (token.IsCancellationRequested) break;

            BossLine line = lines[index];
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


    }

    private async UniTask PlayDeadParteicle()
    {
        Vector3 bossPos = new Vector3(boss.transform.position.x, boss.transform.position.y + 3f, boss.transform.position.z);
        ParticleSystem deadPartice = Instantiate(bossDeadParticle, bossPos, Quaternion.identity);
        deadPartice.Play();
    }
}
