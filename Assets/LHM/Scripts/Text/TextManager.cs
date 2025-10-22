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
        StartStageDialogue(2);
    }
    public void StartStageDialogue(int stageNum)
    {
        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        string[] lines = null;

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
            dialogueRoutine = StartCoroutine(PlayDialogue(lines));
    }

    private IEnumerator PlayDialogue(string[] lines)
    {
        dialogueUI.ShowDialogueUI(true); // 켜기

        foreach (var line in lines)
        {
            dialogueUI.ShowDialogueText(line);
            yield return new WaitForSeconds(interval);
        }

        dialogueUI.ShowDialogueUI(false); // 끄기
        dialogueRoutine = null;
    }
}
