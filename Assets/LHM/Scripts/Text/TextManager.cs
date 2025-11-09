using Cysharp.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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
    [SerializeField] private RawImage Boss1DataImage;
    [SerializeField] private RawImage Boss2DataImage;
    [SerializeField] private RawImage Boss3DataImage;
    [SerializeField] public GameObject BossButton;
    public static bool BossButtonClicked = false;

    private CancellationTokenSource dialogueCTS;
    private CancellationTokenSource bossCTS;
    bool skipAll = false;
    [SerializeField] ParticleSystem bossDeadParticle;
    [SerializeField] GameObject boss;
    private bool isBossDialogueStarted = false;

    [SerializeField] TMP_InputField inputField;
    [SerializeField] GameObject inputPanel;


    private void Update()
    {

    }

    public async UniTask BossDialogueCheackAsync()
    {

        if (isBossDialogueStarted) return;
        isBossDialogueStarted = true;
        Debug.Log("BossDialogueCheackAsync() 호출됨");
        if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
        {
            Debug.Log("BossDialogueCheackAsync() 호출됨");
            //BossClosePanel.SetActive(true);
            //await BossHandleBossDeathAsync(1);
            await PlayDeadParteicle();
            Destroy(boss);
            ComboSave.Instance.MaxcomboSaveScr();
            await UniTask.Delay(2000);
            SceneLoader.Instance.LoadScene("StageSelect");
        }
        if (BossState.isBoss1Dead && BossState.isBoss2Dead && !BossState.isBoss3Dead)
        {
            Debug.Log("Boss2DialogueCheackAsync() 호출됨");
            //BossClosePanel.SetActive(true);
            //await BossHandleBossDeathAsync(2);
            await PlayDeadParteicle();
            Destroy(boss);
            ComboSave.Instance.MaxcomboSaveScr();
            await UniTask.Delay(2000);
            SceneLoader.Instance.LoadScene("StageSelect");
        }
        if (BossState.isBoss3Dead)
        {
            Debug.Log("Boss2DialogueCheackAsync() 호출됨");
            BossClosePanel.SetActive(true);
            await BossHandleBossDeathAsync(3);
            InputName();

        }
        Debug.Log("BossDialogueCheackAsync 실행됨");

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
    private async UniTask BossHandleBossDeathAsync(int bossStage)
    {
        Debug.Log("BossHandleBossDeathAsync() 호출됨");
        if (bossStage == 1)
            BossState.isBoss1Dead = true;
        else if (bossStage == 2)
            BossState.isBoss2Dead = true;
        else if (bossStage == 3)
            BossState.isBoss3Dead = true;
        dialogueUI.ShowDialogueUI(true);
        await BossStartStageDialogueAsync(bossStage);
    }

    public async UniTask BossStartStageDialogueAsync(int firstStage)
    {
        bossCTS?.Cancel();
        bossCTS = new CancellationTokenSource();

        var bossSet = System.Array.Find(bosstextData.bossDialogues, x => x.bossName == $"Boss{firstStage}");
        if (bossSet == null)
        {
            return;
        }

        BossLine[] lines = bossSet.deathLines;
        if (lines != null)
        {
            await stagePlayDialogueAsync(lines, firstStage, bossCTS.Token);
        }
    }

    private async UniTask firstPlayDialogueAsync(DialogueLine[] lines, int stageNum, CancellationToken token)
    {
        TalkState.isTalking = true;
        int index = 0;
        bool waitingForClick = false;
        bool skipAll = false;
        System.Action onClick = () => waitingForClick = false;
        System.Action onSkipAll = () => skipAll = true;

        dialogueUI.OnDialogueClick += onClick;
        dialogueUI.OnSkipAll += onSkipAll;

        dialogueUI.ShowDialogueUI(true);
        dialogueUI.StartImageAnimation(stageNum);

        while (index < lines.Length)
        {
            if (token.IsCancellationRequested || skipAll) break;

            DialogueLine line = lines[index];
            var localized = new LocalizedString(line.localizationTable, line.localizationKey);
            string localizedText = await localized.GetLocalizedStringAsync();

            dialogueUI.ShowDialogueText(localizedText, line.sound);

            waitingForClick = true;
            while (waitingForClick && !token.IsCancellationRequested && !skipAll)
                await UniTask.Yield(PlayerLoopTiming.Update, token);

            index++;
        }

        dialogueUI.OnDialogueClick -= onClick;
        dialogueUI.OnSkipAll -= onSkipAll;
        dialogueUI.StopImageAnimation();
        dialogueUI.ShowDialogueUI(false);
        dialogueUI.speechBubble.SetActive(false);

        TalkState.isTalking = false;
    }
    private async UniTask stagePlayDialogueAsync(BossLine[] lines, int stageNum, CancellationToken token)
    {
        Debug.Log(lines);
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

            string displayText = line.text;
            if (!string.IsNullOrEmpty(line.localizationTable) && !string.IsNullOrEmpty(line.localizationKey))
            {
                var localized = new LocalizedString(line.localizationTable, line.localizationKey);
                displayText = await localized.GetLocalizedStringAsync();
            }

            dialogueUI.ShowDialogueText(displayText, line.sound);

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

    private void InputName()
    {
        Cursor.visible = true;
        inputPanel.SetActive(true);
        inputField.ActivateInputField();
    }

    public void EnterInput()
    {
        EnterPressed().Forget();
    }
    public async UniTask OnBossImage(int stageNum, bool active)
    {
        switch (stageNum)
        {
            case 2:
                Boss1DataImage.gameObject.SetActive(active);
                BossButton.gameObject.SetActive(active);
                break;
            case 3:
                Boss2DataImage.gameObject.SetActive(active);
                BossButton.gameObject.SetActive(active);
                break;
            case 4:
                Boss3DataImage.gameObject.SetActive(active);
                BossButton.gameObject.SetActive(active);
                break;
        }
    }

    public void OnBossImageClick()
    {
        OnBossImage(2, false).Forget();
        OnBossImage(3, false).Forget();


        OnBossImage(4, false).Forget();
        BossButtonClicked = true;
    }



    public async UniTask EnterPressed()
    {
        if (string.IsNullOrEmpty(inputField.text))
            return;

        Datamanager.instance.curPlayerData.playerName = inputField.text;
        Datamanager.instance.curPlayerData.combo = ComboSave.Instance.maxComboData.maxComboValue;
        Datamanager.instance.SaveToJson();

        ComboSave.Instance.maxComboData.maxCombo = 0;

        inputPanel.SetActive(false);
        await PlayDeadParteicle();
        Destroy(boss);

        await UniTask.Delay(1500);

        SceneLoader.Instance.LoadScene("Ending");
    }

}
