using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization;

public class PlayerDieText : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameOverTextUI gameOverUI;

    [Header("Localization Key")]
    [SerializeField] private string tableName = "GameOver";

    private void Start()
    {
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

        // 게임오버 패널이 꺼질 때까지 대기
        while (gameOverUI.gameObject.activeSelf)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        int randomNum = UnityEngine.Random.Range(1, num + 1);
        string entryKey = $"GameOver{randomNum}";

        var localizedString = new LocalizedString(tableName, entryKey);
        string message = await localizedString.GetLocalizedStringAsync();

        Debug.Log($"GameOverText: {entryKey} → {message}");

        await gameOverUI.ShowGameOverText(message);
    }

}
