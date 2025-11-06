using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;
	string currentSceneName;
    [SerializeField] private GameOverTextUI gameOverTextUI;
    [SerializeField] public GameObject gameOverPanel;
    [SerializeField] GameObject GameOverText;
	[SerializeField] private string tableNmae = "GameOver";
    public AudioClip gameOverSound;
	private bool isTriggered = false;
	private bool isRetrying = false; 
	private void Awake()
    {
		 
		currentSceneName = SceneManager.GetActiveScene().name;
	}
    private void Start()
    {
		gameOverPanel.SetActive(false);
	}
    public async UniTask TriggerGameOver(CancellationToken token = default)
    {
		if (this == null) return;

		if (isTriggered) return;
		isTriggered = true;
		if (gameOverPanel != null)
			gameOverPanel.SetActive(true);
		await UniTask.Delay(5000, cancellationToken: token);
		if (this == null || token.IsCancellationRequested) return;

		if(gameOverTextUI != null)
		{
			int randomNum = UnityEngine.Random.Range(1, 4);
			string entryKey = $"GameOver{randomNum}";
			var localizedString = new LocalizedString(tableNmae, entryKey);
			string message = await localizedString.GetLocalizedStringAsync();
			await gameOverTextUI.ShowGameOverText(message);
		}




		if (gameOverSound != null)
		{
			int sL = (int)(gameOverSound.length * 1000);
			await UniTask.Delay(sL, cancellationToken: token); 
		}
		await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
	}

	public void RetryButton()
	{
		if (!isRetrying)
		{
			RetryAsync();
		}
	}
	public void MainButton()
	{
		GoMain();
	}
	private void RetryAsync()
	{
		if (isRetrying) return; 
		isRetrying = true;
		try
		{
			if (SceneLoader.Instance != null && !string.IsNullOrEmpty(currentSceneName))
			{
				SceneLoader.Instance.LoadScene(currentSceneName);
			}
		}
		finally
		{
			isRetrying = false;
			isTriggered = false; 
		}
	}

	// private void Retry()
	// {
	//     if (SceneLoader.Instance != null && !string.IsNullOrEmpty(currentSceneName))
	//         SceneLoader.Instance.LoadScene(currentSceneName).Forget();
	// }

	private GameObject FindObjectIncludingInactive(string name)
	{
		Scene activeScene = SceneManager.GetActiveScene();
		GameObject[] rootObjects = activeScene.GetRootGameObjects();

		foreach (GameObject root in rootObjects)
		{
			if (root.name == name) return root;

			Transform[] children = root.GetComponentsInChildren<Transform>(true);

			foreach (Transform child in children)
			{
				if (child.name == name) return child.gameObject;
			}
		}
		return null;
	}
	private void GoMain()
	{
        if (isRetrying) return; 
        isRetrying = true;
        try
        {
            if (SceneLoader.Instance != null && !string.IsNullOrEmpty("Main"))
			{ 
                SceneLoader.Instance.LoadScene("Main");
            }
        }
        finally
        {
            isRetrying = false;
            isTriggered = false; 
        }
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

		gameOverPanel = FindObjectIncludingInactive("GameOverOB");
		GameOverText = GameObject.Find("GameOverText");
		gameOverTextUI = FindAnyObjectByType<GameOverTextUI>();

		if (gameOverPanel != null)
		{
			gameOverPanel.SetActive(false);
		}
		GameObject retryObject = FindObjectIncludingInactive("Retry");

		if (retryObject != null)
		{
			Button retryButton = retryObject.GetComponent<Button>();

			if (retryButton != null)
			{
				retryButton.onClick.RemoveAllListeners();
				retryButton.onClick.AddListener(RetryButton);
			}
		}
        GameObject mainObject = FindObjectIncludingInactive("Main");

        if (mainObject != null)
        {
            Button mainButton = mainObject.GetComponent<Button>();

            if (mainButton != null)
            {
                mainButton.onClick.RemoveAllListeners();
                mainButton.onClick.AddListener(MainButton);
            }
        }

    }
}
