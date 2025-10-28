using UnityEngine;

public class DialogueTriggerOnBossState : MonoBehaviour
{
    [SerializeField] private TextManager dialogueAutoPlayer;

    void Start()
    {
        // 스테이지1 보스가 죽었으면 해당 대사 실행
        if (BossState.isBoss1Dead)
        {
            _ = dialogueAutoPlayer.StartStageDialogueAsync(2); // 예: 다음 스테이지의 대사 실행
        }

        // 필요하면 다른 보스도 조건 추가
        if (BossState.isBoss2Dead)
        {
            _ = dialogueAutoPlayer.StartStageDialogueAsync(3);
        }
        if (MainGhostTrainingState.isClicked)
        {
            Debug.Log("트레이닝 룸 진입: 대사 시작");
            _ = dialogueAutoPlayer.StartStageDialogueAsync(1); // 트레이닝용 대사 번호
        }
    }
}
