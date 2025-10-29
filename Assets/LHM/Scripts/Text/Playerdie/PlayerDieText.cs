using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class PlayerDieText : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameOverTextUI gameOverUI;

    private CancellationTokenSource dieCTS;

    private void Start()
    {
        dieCTS = new CancellationTokenSource();
        if (gameOverUI != null)
        {
            var textObj = gameOverUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textObj != null) textObj.gameObject.SetActive(false);
        }
    }

    public async UniTask OnDeadSequence(int num)
    {
        if (gameOverUI == null)
        {
            Debug.LogError("GameOverTextUI reference missing!");
            return;
        }

        int randomNum = UnityEngine.Random.Range(1, num + 1);
        Debug.Log("PlayerDieText Random Num: " + randomNum);

        string message = randomNum switch
        {
            1 => "Boo.. 포기하지마.. 잭슨..",
            2 => "Boo.. 기다릴게.. 다시 도전해줘..",
            _ => "다시 한번 마음을 가다듬어봐..."
        };

        await gameOverUI.ShowGameOverText(message);
    }

    private void OnDestroy()
    {
        dieCTS?.Cancel();
    }
}
