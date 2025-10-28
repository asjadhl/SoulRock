using System.Collections;
using UnityEngine;
public class TextManager : MonoBehaviour
{
    [SerializeField] private StageDialogueData dialogueData;
    [SerializeField] private DialogueUIManager dialogueUI;
    [SerializeField] private float interval = 3f;

    private Coroutine dialogueRoutine;

  

    void Start()
    {
        StartCoroutine(DelayedDialogueCheck());
    }


    void Update()
    {
        

    }

    private IEnumerator DelayedDialogueCheck()
    {
        yield return new WaitForSeconds(0.1f);

        if (MainGhostTrainingState.isClicked)
            StartStageDialogue(1);
        if (MainPlayState.isClicked1)
            StartStageDialogue(2);
        if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
            StartStageDialogue(3);
        if (MapSelected3.start3)
            StartStageDialogue(4);
        if (MapSelected3.stop3)
            StartStageDialogue(5);
    }

    public void StartStageDialogue(int stageNum)
    {
        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        DialogueLine[] lines = null;

        switch (stageNum)
        {
            case 1: lines = dialogueData.stage1.dialogues; break;
            case 2: lines = dialogueData.stage2.dialogues; break;
            case 3: lines = dialogueData.stage3.dialogues; break;
            case 4: lines = dialogueData.stage4.dialogues; break;
            case 5: lines = dialogueData.stage5.dialogues; break;
        }

        if (lines != null)
            dialogueRoutine = StartCoroutine(PlayDialogue(lines, stageNum));
    }

    private IEnumerator PlayDialogue(DialogueLine[] lines, int stageNum)
    {
        // 대화 시작 시 True 설정
        switch (stageNum)
        {
            case 1: DialogueLineTrueORFalse.TutorialTrue = true; break;
            case 2: DialogueLineTrueORFalse.stage1True = true; break;
            case 3: DialogueLineTrueORFalse.stage2True = true; break;
            case 4: DialogueLineTrueORFalse.stage3_1True = true; break;
            case 5: DialogueLineTrueORFalse.stage3_2True = true; break;
        }

        int index = 0;
        bool waitingForClick = false;

        // 클릭 이벤트 등록
        System.Action onClick = () => waitingForClick = false;
        dialogueUI.OnDialogueClick += onClick;

        dialogueUI.ShowDialogueUI(true);
        dialogueUI.StartImageAnimation(stageNum);

        // 반복문으로 한 줄씩 출력
        while (index < lines.Length)
        {
            DialogueLine line = lines[index];
            dialogueUI.ShowDialogueText(line.text, line.sound);

            waitingForClick = true;
            while (waitingForClick)
                yield return null;

            index++;
        }

        // 이벤트 해제 및 종료 처리
        dialogueUI.OnDialogueClick -= onClick;
        dialogueUI.StopImageAnimation();
        dialogueUI.ShowDialogueUI(false);
        dialogueUI.speechBubble.SetActive(false);
        dialogueRoutine = null;

        // 대화 종료 시 False 설정
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
