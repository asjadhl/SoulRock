using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
#region ("대사")
public static class TalkState
{
       public static bool isTalking = false;//대사가 재생중인지 확인
}
public class TextManager : MonoBehaviour
{
    [SerializeField] private BossTextData bosstextData;
    [SerializeField] private StageDialogueData dialogueData;

    [SerializeField] private DialogueUIManager dialogueUI;
    [SerializeField] private GameObject BossClosePanel;

    [SerializeField] private RawImage Boss1DataImage;
    [SerializeField] private RawImage Boss2DataImage;
    [SerializeField] private RawImage Boss3DataImage;

    [SerializeField] private Texture Boss1DataImage_KO;
    [SerializeField] private Texture Boss1DataImage_EN;
    [SerializeField] private Texture Boss2DataImage_KO;
    [SerializeField] private Texture Boss2DataImage_EN;
    [SerializeField] private Texture Boss3DataImage_KO;
    [SerializeField] private Texture Boss3DataImage_EN;

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

    #region ("스테이지 진입 씬 대사")
    public async UniTask DelayedDialogueCheckAsync()//스테이지 선택씬에서 대사 출력
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
    public void OnBossImageClick()//스테이지 선택씬에서 대사 출력이후 보스패턴 이미지 패널 활성화시 닫기 버튼으로 씬 전환
    {
        if (!BossState.isBoss1Dead && !BossState.isBoss2Dead)//보스 1 시작전
        {
            OnBossImage(2).Forget();
            SceneLoader.Instance.LoadScene("Stage2");
        }
        if (BossState.isBoss1Dead && !BossState.isBoss2Dead)//보스 2 시작전
        {
            OnBossImage(3).Forget();
            SceneLoader.Instance.LoadScene("Stage3");
        }
        if (BossState.isBoss1Dead && BossState.isBoss2Dead)//보스 3 시작전
        {
            OnBossImage(4).Forget();
            SceneLoader.Instance.LoadScene("LastStage");
        }
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
    private async UniTask firstPlayDialogueAsync(DialogueLine[] lines, int stageNum, CancellationToken token)//대사 출력
    {
        TalkState.isTalking = true;
        int index = 0;
        bool waitingForClick = false;
        bool skipAll = false;
        System.Action onClick = () => waitingForClick = false;
        System.Action onSkipAll = () => skipAll = true;

        dialogueUI.OnDialogueClick += onClick;
        dialogueUI.OnSkipAll += onSkipAll;

        dialogueUI.ShowDialogueUI(true);//대화 ui 활성화
        dialogueUI.StartImageAnimation(stageNum);//스테이지별 이미지 애니메이션

        while (index < lines.Length)
        {
            if (token.IsCancellationRequested || skipAll) break;

            DialogueLine line = lines[index];
            var localized = new LocalizedString(line.localizationTable, line.localizationKey);
            string localizedText = await localized.GetLocalizedStringAsync();//텍스트 불러오기

            dialogueUI.ShowDialogueText(localizedText, line.sound);//텍스트 출력

            waitingForClick = true;
            while (waitingForClick && !token.IsCancellationRequested && !skipAll)//클릭 할때까지 대기
                await UniTask.Yield(PlayerLoopTiming.Update, token);//한문장 출력 대기

            index++;
        }

        dialogueUI.OnDialogueClick -= onClick;
        dialogueUI.OnSkipAll -= onSkipAll;
        dialogueUI.StopImageAnimation();
        dialogueUI.ShowDialogueUI(false);
        dialogueUI.speechBubble.SetActive(false);

        TalkState.isTalking = false;
    }
    #endregion
    #region("보스 사망시 대사")
    private async UniTask BossHandleBossDeathAsync(int bossStage)//보스 사망시 대사 재생
    {
        if (bossStage == 3)
            BossState.isBoss3Dead = true;
        dialogueUI.ShowDialogueUI(true);
        await BossStartStageDialogueAsync(bossStage);
    }

    public async UniTask BossStartStageDialogueAsync(int firstStage)//보스 사망시 대사 시작
    {
        bossCTS?.Cancel();
        bossCTS = new CancellationTokenSource();
        var bossSet = System.Array.Find(bosstextData.bossDialogues, x => x.bossName == $"Boss{firstStage}");//보스 이름과 일치하는 대사 찾기
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
    private async UniTask stagePlayDialogueAsync(BossLine[] lines, int stageNum, CancellationToken token)//보스 대사 출력
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
            if (!string.IsNullOrEmpty(line.localizationTable) && !string.IsNullOrEmpty(line.localizationKey))//영문,한문 대사 처리
            {
                var localized = new LocalizedString(line.localizationTable, line.localizationKey);
                displayText = await localized.GetLocalizedStringAsync();//텍스트 불러오기
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
    #endregion
    #region ("보스 사망시 씬전환")
    public async UniTask BossDialogueCheackAsync()//보스 사망시 스테이지 선택씬 전환
    {

        if (isBossDialogueStarted) return;
        isBossDialogueStarted = true;
        if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
        {
            await PlayDeadParteicle();//보스 사망시 파티클 재생
            Destroy(boss);
            ComboSave.Instance.MaxcomboSaveScr();//최대 콤보 저장
            await UniTask.Delay(2000);
            SceneLoader.Instance.LoadScene("StageSelect");
        }
        if (BossState.isBoss1Dead && BossState.isBoss2Dead && !BossState.isBoss3Dead)
        {
            await PlayDeadParteicle();//보스 사망시 파티클 재생
            Destroy(boss);
            ComboSave.Instance.MaxcomboSaveScr();//최대 콤보 저장
            await UniTask.Delay(2000);
            SceneLoader.Instance.LoadScene("StageSelect");
        }
        if (BossState.isBoss3Dead)
        {
            BossClosePanel.SetActive(true);
            await BossHandleBossDeathAsync(3);
            InputName();  // 이름 입력받아서 순위 띄우기

        }
    }
    public async UniTask ClosePanel()
    {
        BossClosePanel.SetActive(true);

    }
    private async UniTask PlayDeadParteicle()//보스 사망 파티클
    {
        Vector3 bossPos = new Vector3(boss.transform.position.x, boss.transform.position.y + 3f, boss.transform.position.z);
        ParticleSystem deadPartice = Instantiate(bossDeadParticle, bossPos, Quaternion.identity);
        deadPartice.Play();
    }
    #endregion
    #region ("언어 설정 및 엔딩")
    private void InputName()//엔딩시 유저 닉네임 기록용
    {
        Cursor.visible = true;
        inputPanel.SetActive(true);
        inputField.ActivateInputField();
    }

    public void EnterInput()
    {
        EnterPressed().Forget();
    }
    public async UniTask OnBossImage(int stageNum, bool active = true)//영문,한문 전환
    {
        // 현재 설정된 언어 가져오기 (0 = en, 1 = ko)
        int lang = PlayerPrefs.GetInt("LocalKey", 1);

        switch (stageNum)
        {
            case 2:
                Boss1DataImage.gameObject.SetActive(active);
                if (active)
                    Boss1DataImage.texture = (lang == (int)Language.en) ? Boss1DataImage_EN : Boss1DataImage_KO;
                BossButton.gameObject.SetActive(active);
                break;

            case 3:
                Boss2DataImage.gameObject.SetActive(active);
                if (active)
                    Boss2DataImage.texture = (lang == (int)Language.en) ? Boss2DataImage_EN : Boss2DataImage_KO;
                BossButton.gameObject.SetActive(active);
                break;

            case 4:
                Boss3DataImage.gameObject.SetActive(active);
                if (active)
                    Boss3DataImage.texture = (lang == (int)Language.en) ? Boss3DataImage_EN : Boss3DataImage_KO;
                BossButton.gameObject.SetActive(active);
                break;
        }
    }
   
    public async UniTask EnterPressed()//엔딩씬 이름 입력 후 씬 전환
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
    #endregion
    #region ("튜토리얼 대사 출력")
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
            StartTutorialAsync().Forget();
        }
        #region ("튜토리얼 대사 재생")
        private async UniTaskVoid StartTutorialAsync()//튜토리얼 대사 시작
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
        private async UniTask PlayDialogueSetAsync(BossLine[] lineSet, CancellationToken token)//대사 재생
        {
            if (lineSet == null || lineSet.Length == 0)//대사 없으면 종료
            {
                return;
            }
            foreach (var line in lineSet)//대사 한줄씩 재생
            {
                await PlayDialogueLineAsync(line, token);
            }
        }
        private async UniTask PlayDialogueLineAsync(BossLine line, CancellationToken token)//대사 한줄 재생
        {
            TutoUI.ShowDialogueUI(true);
            TutoUI.OnDialogueClick -= OnDialogueClicked;
            TutoUI.OnDialogueClick += OnDialogueClicked;

            await TypeLocalizedDialogueAsync(line.localizationTable, line.localizationKey, line.sound, token);//대사 출력

            waitingForNext = true;
            if (nextButton != null) nextButton.SetActive(true);//Next 버튼 활성화

            await UniTask.WaitUntil(() => waitingForNext == false, cancellationToken: token);//Next 버튼 클릭 대기
            TutoUI.OnDialogueClick -= OnDialogueClicked;
        }
        private async UniTask TypeLocalizedDialogueAsync(string table, string key, AudioClip clip, CancellationToken token)//대사 텍스트 타이핑 효과
        {
            TutoUI.ClearText();
            isTyping = true;

            var localizedString = new LocalizedString(table, key);
            string text = await localizedString.GetLocalizedStringAsync();//텍스트 불러오기

            if (string.IsNullOrEmpty(text))
            {
                text = key;
            }
            for (int i = 0; i < text.Length; i++)
            {
                if (token.IsCancellationRequested) return;

                if (skipRequested)//스킵 요청시 남은 텍스트 한번에 출력
                {
                    TutoUI.AppendText(text.Substring(i));//텍스트 한번에 출력
                    skipRequested = false;
                    break;
                }

                TutoUI.AppendText(text[i].ToString());
                if (clip != null) TutoUI.PlaySound(clip);
                await UniTask.Delay(TimeSpan.FromSeconds(charInterval), cancellationToken: token);//타이핑 효과 대기
            }

            isTyping = false;
        }
        #endregion
        #region ("훈련장 더미 관련")
        private async UniTask OnDummyDeadSequence()//더미 사망시 대사 및 씬 전환
        {
            if (dummyDeadTriggered) return;
            dummyDeadTriggered = true;

            TutoUI.StopImageAnimation();

            if (tutorialData.bossDialogues.Length > 1)
                await PlayDialogueSetAsync(tutorialData.bossDialogues[1].deathLines, tutorialCTS.Token);//더미 사망 후 대사

            await UniTask.Delay(TimeSpan.FromSeconds(1));
            PlayerPrefs.SetInt("IsTutorial", 1);
            SceneLoader.Instance.LoadScene("StageSelect");
        }
        private void OnDialogueClicked()
        {
            if (isTyping)
                skipRequested = true;
            else if (waitingForNext)
                waitingForNext = false;
        }
        private async UniTask HighlightDummyAsync()//더미 반짝이는 하이라이트 효과
        {
            if (dummyRenderer == null) return;

            float timer = 0f;
            while (timer < highlightDuration)
            {
                dummyRenderer.material.color = Color.Lerp(originalColor, highlightColor, Mathf.PingPong(timer * 4f, 1f));//색상 변화
                timer += Time.deltaTime;
                await UniTask.Yield();
            }
            dummyRenderer.material.color = originalColor;//원래 색상 복원
        }
        #endregion
        private void OnDestroy()
        {
            tutorialCTS?.Cancel();
        }
    }
    #endregion
}
#endregion
