using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class GameOverManager : MonoBehaviour
{
    string currentSceneName;
    [SerializeField] private GameOverTextUI gameOverTextUI;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject GameOverText;

    public MusicBox musicBox;
    public AudioClip gameOverSound;
    
    private bool isTriggered = false;
    private async void Awake()
    {

        currentSceneName = SceneManager.GetActiveScene().name;
    }
    void Start()
    {
      
    }
    public async UniTask TriggerGameOver()
    {
        if (isTriggered) return;
        isTriggered = true;
        gameOverPanel.SetActive(true);

        await UniTask.Delay(5000);
        GameOverText.SetActive(false);
        //audioSource.Stop();
        //audioSource.PlayOneShot(gameOverSound);
        int sL = (int)gameOverSound.length * 1000;
        await UniTask.Delay(sL);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        if (gameOverTextUI != null)
        {
            string[] messages =
            {
                "Boo.. 포기하지마.. 잭슨..",
                "Boo.. 기다릴게.. 다시 도전해줘..",
                "다시 한번 마음을 가다듬어봐..."
            };

            int randomIndex = UnityEngine.Random.Range(0, messages.Length);
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
