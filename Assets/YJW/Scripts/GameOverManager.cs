using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
public class GameOverManager : MonoBehaviour
{
    string currentSceneName;
    [SerializeField] private GameOverTextUI gameOverTextUI;
    [SerializeField] private GameObject gameOverPanel;

    private bool isTriggered = false;
    private async void Awake()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }
    public async UniTask TriggerGameOver()
    {
        if (isTriggered) return;
        isTriggered = true;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }


        await UniTask.Delay(4000);

        if (gameOverTextUI != null)
        {
            string[] messages =
            {
                "Boo.. 포기하지마.. 잭슨..",
                "Boo.. 기다릴게.. 다시 도전해줘..",
                "다시 한번 마음을 가다듬어봐..."
            };

            int randomIndex = Random.Range(0, messages.Length);
            await gameOverTextUI.ShowGameOverText(messages[randomIndex]);
        }
    }
    public void Retry()
    {
        SceneManager.LoadScene(currentSceneName);
    }

    public void GoMain()
    {
        SceneManager.LoadScene("Main");
    }
}
