using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueTriggerOnBossState : MonoBehaviour
{
    [SerializeField] private TextManager textManager;
    private async Task Start()
    {
        if (textManager == null)
            textManager = FindObjectOfType<TextManager>();

        if (BossState.isBoss1Dead && !BossState.isBoss2Dead)
        {

            await textManager.StartStageDialogueAsync(3);
            await textManager.OnBossImage(3, true);
        }
        else if (!BossState.isBoss1Dead)
        {
            await textManager.StartStageDialogueAsync(2);
            await textManager.OnBossImage(2, true);
        }
        Debug.Log($"[DialogueTrigger] Boss1Dead={BossState.isBoss1Dead}, Boss2Dead={BossState.isBoss2Dead}");

        
    }
}
