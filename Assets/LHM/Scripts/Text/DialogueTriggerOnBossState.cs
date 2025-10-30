using UnityEngine;

public class DialogueTriggerOnBossState : MonoBehaviour
{
    [SerializeField] private TextManager textManager;
    private void Start()
    {
        if (textManager == null)
            textManager = FindObjectOfType<TextManager>();

        if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
        {
          
            textManager.StartStageDialogueAsync(3).Forget();
        }
        else if (!BossState.isBoss1Dead)
        {
            textManager.StartStageDialogueAsync(2).Forget();
        }
        Debug.Log($"[DialogueTrigger] Boss1Dead={BossState.isBoss1Dead}, Boss2Dead={BossState.isBoss2Dead}");

        //// 보스2 클리어 → StageSelect 복귀 시
        //else if (BossState.isBoss2Dead && !BossState.isBoss3Dead)
        //{
        //    Debug.Log("보스2 클리어: 보스3 설명 대사 출력");
        //    textManager.StartStageDialogueAsync(4).Forget();
        //}

        //// 보스3 클리어 → 엔딩 설명
        //else if (BossState.isBoss3Dead)
        //{
        //    Debug.Log("보스3 클리어: 엔딩 대사 출력");
        //    textManager.StartStageDialogueAsync(5).Forget();
        //}
    }
}
