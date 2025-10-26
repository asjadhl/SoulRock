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
       
        // Boss1 사망 감지
        if (BossState.isBoss1Dead)
        {
            StartStageDialogue(2); // 보스 처치 후 다음 스테이지 대사

        }

        // Boss2 사망 감지
        if (BossState.isBoss2Dead)
        {
            StartStageDialogue(3);

        }

        // Boss3 사망 감지
        if (BossState.isBoss3Dead)
        {
            StartStageDialogue(4);

        }
        if (MainGhostTrainingState.isClicked)
        {
            Debug.Log("트레이닝 룸 진입: 대사 시작");
            StartStageDialogue(1); // 트레이닝용 대사 번호
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BossState.isBoss1Dead = true;
            SceneManager.LoadScene("StageSelect");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
           // MainGhostTrainingState.isClicked = true;
            SceneManager.LoadScene("TraingRoom");
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

        dialogueUI.speechBubble.SetActive(false);
        dialogueRoutine = null;
    }
}
