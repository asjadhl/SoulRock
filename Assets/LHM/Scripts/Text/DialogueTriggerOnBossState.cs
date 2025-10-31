using Cysharp.Threading.Tasks;
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

        
    }
}
