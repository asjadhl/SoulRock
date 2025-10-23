using System.Collections;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [Header("대사 데이터")]
    [SerializeField] private StageDialogueData dialogueData;
    [SerializeField] private DialogueUIManager dialogueUI;

    [Header("대사 간 간격 (초)")]
    [SerializeField] private float interval = 3f;

    private Coroutine dialogueRoutine;
    void Start()
    {
        // 예시: 1스테이지 대사 시작
        StartStageDialogue(1);
    }
    public void StartStageDialogue(int stageNum)
    {
        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        DialogueLine[] lines = null; // string[] → DialogueLine[]

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
            dialogueRoutine = StartCoroutine(PlayDialogue(lines, stageNum)); // 스테이지 번호 전달
    }


    private IEnumerator PlayDialogue(DialogueLine[] lines, int stageNum)
    {
        dialogueUI.ShowDialogueUI(true);               // 대화창 켜기
        dialogueUI.StartImageAnimation(stageNum);      // 스테이지별 이미지 애니메이션 시작

        foreach (var line in lines)
        {
            dialogueUI.ShowDialogueText(line.text, line.sound);
            yield return new WaitForSeconds(interval);
        }

        dialogueUI.StopImageAnimation();               // 대화 종료 시 애니메이션 정지
        dialogueUI.ShowDialogueUI(false);              // 대화창 끄기
        dialogueRoutine = null;
    }


}
