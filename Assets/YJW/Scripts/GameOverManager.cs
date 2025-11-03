using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;
	string currentSceneName;
    [SerializeField] private GameOverTextUI gameOverTextUI;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject GameOverText;

    public AudioClip gameOverSound;
    
    private bool isTriggered = false;
    private void Awake()
    {
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		currentSceneName = SceneManager.GetActiveScene().name;
	}
    void Start()
    {
      
    }
    public async UniTask TriggerGameOver()
    {
        if (isTriggered) return;
        isTriggered = true;
		if (gameOverPanel != null)
			gameOverPanel.SetActive(true);
		await UniTask.Delay(5000);
		if (GameOverText != null)
			GameOverText.SetActive(false);
		//audioSource.Stop();
		//audioSource.PlayOneShot(gameOverSound);
		if (gameOverSound != null)
		{
			int sL = (int)(gameOverSound.length * 1000);
			await UniTask.Delay(sL);
		}
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

	public void RetryButton()
	{
		Retry().Forget();
	}
	public void MainButton()
	{
		GoMain().Forget();
	}
    private async UniTask Retry()
    {
		if (SceneLoader.Instance != null)
			await SceneLoader.Instance.LoadScene("New Scene");
	}

	private async UniTask GoMain()
    {
		if (SceneLoader.Instance != null)
			await SceneLoader.Instance.LoadScene("Main");
	}
	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		currentSceneName = scene.name;

		if (gameOverPanel == null)
			gameOverPanel = GameObject.Find("GameOver"); // 씬에 있는 GameOverPanel 이름으로
		if (GameOverText == null)
			GameOverText = GameObject.Find("GameOverText"); // 씬에 있는 Text 오브젝트 이름으로
		if (gameOverTextUI == null)
			gameOverTextUI = FindAnyObjectByType<GameOverTextUI>();
	}
}
