using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    [SerializeField] private StageDialogueData dialogueData;
    [SerializeField] private DialogueUIManager dialogueUI;
    [SerializeField] private float interval = 3f;

    private Coroutine dialogueRoutine;

  

    void Start()
    {
        if (MainGhostTrainingState.isClicked)
        {
            Debug.Log("트레이닝 룸 진입: 대사 시작");
            StartStageDialogue(1); // 트레이닝용 대사 번호
        }
        if (MainPlayState.isClicked1)
        {
            StartStageDialogue(2); // 스테이지 선택 후 대사
        }
        // Boss1 사망 감지
        if (BossState.isBoss1Dead)
        {
            StartStageDialogue(3); // 보스 처치 후 다음 스테이지 대사
        }
        
        if (MapSelected3.start3)
        {
            StartStageDialogue(4); // 마지막 스테이지 대사
        }
        if (MapSelected3.stop3)
        {
            StartStageDialogue(5); // 마지막 스테이지 대사
        }
    }

    void Update()
    {
        

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
                lines = dialogueData.stage4.dialogues;
                break;
            case 5:
                lines = dialogueData.stage5.dialogues;
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

        dialogueUI.speechBubble.SetActive(false);
        dialogueRoutine = null;
    }
}
