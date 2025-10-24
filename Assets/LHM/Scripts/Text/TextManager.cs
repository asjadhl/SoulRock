using UnityEngine;
using System.Collections;

public class TextManager : MonoBehaviour
{
    [SerializeField] private StageDialogueData dialogueData;
    [SerializeField] private DialogueUIManager dialogueUI;
    [SerializeField] private float interval = 3f;

    private Coroutine dialogueRoutine;

    // 이전 상태값 저장용
    private bool prevBoss1Dead;
    private bool prevBoss2Dead;
    private bool prevBoss3Dead;

    void Start()
    {
        // 예시: 스테이지 1 대사 시작
        StartStageDialogue(1);

        // 초기 상태 저장
        prevBoss1Dead = BossState.isBoss1Dead;
        prevBoss2Dead = BossState.isBoss2Dead;
        prevBoss3Dead = BossState.isBoss3Dead;
    }

    void Update()
    {
        // Boss1 사망 감지
        if (!prevBoss1Dead && BossState.isBoss1Dead)
        {
            StartStageDialogue(2); // 보스 처치 후 다음 스테이지 대사
            prevBoss1Dead = true;
        }

        // Boss2 사망 감지
        if (!prevBoss2Dead && BossState.isBoss2Dead)
        {
            StartStageDialogue(3);
            prevBoss2Dead = true;
        }

        // Boss3 사망 감지
        if (!prevBoss3Dead && BossState.isBoss3Dead)
        {
            StartStageDialogue(4);
            prevBoss3Dead = true;
        }
    }

    public void StartStageDialogue(int stageNum)
    {
        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        DialogueLine[] lines = null;

        switch (stageNum)
        {
            case 1:
                lines = dialogueData.stage1.dialogues;
                break;
            case 2:
                lines = dialogueData.stage2.dialogues;
                break;
            case 3:
                lines = dialogueData.stage3.dialogues;
                break;
            case 4:
                lines = dialogueData.stage3.dialogues;
                break;
        }

        if (lines != null)
            dialogueRoutine = StartCoroutine(PlayDialogue(lines, stageNum));
    }
    
    private IEnumerator PlayDialogue(DialogueLine[] lines, int stageNum)
    {
        dialogueUI.ShowDialogueUI(true);
        dialogueUI.StartImageAnimation(stageNum);

        foreach (var line in lines)
        {
            dialogueUI.ShowDialogueText(line.text, line.sound);
            yield return new WaitForSeconds(interval);
        }

        dialogueUI.StopImageAnimation();
        dialogueUI.ShowDialogueUI(false);
        dialogueRoutine = null;
    }
}
