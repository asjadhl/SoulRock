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
	private bool isRetrying = false; // <<< 추가: 재시도 중복 호출 방지 플래그
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
			Debug.Log($"[GameOver] {entryKey} -> {message}");
			await gameOverTextUI.ShowGameOverText(message);
		}




		if (gameOverSound != null)
		{
			int sL = (int)(gameOverSound.length * 1000);
			// 2. Delay에 토큰 전달
			await UniTask.Delay(sL, cancellationToken: token); // <<< token 전달
		}
		// 3. Delay에 토큰 전달
		await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token); // <<< token 전달
	}

	public void RetryButton()
	{
		if (!isRetrying)
		{
			RetryAsync().Forget();
		}
	}
	public void MainButton()
	{
		GoMain().Forget();
	}

	// Retry 함수를 비동기로 변경하고 currentSceneName을 사용하도록 합니다.
	private async UniTaskVoid RetryAsync()
	{
		if (isRetrying) return; // 이중 체크
		isRetrying = true;
		try
		{
			if (SceneLoader.Instance != null && !string.IsNullOrEmpty(currentSceneName))
			{
				// 씬 로드가 완료될 때까지 기다림
				await SceneLoader.Instance.LoadScene(currentSceneName);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError($"씬 로드 실패: {ex.Message}");
		}
		finally
		{
			// 씬 로드가 실패했든 성공했든, 플래그와 게임 오버 상태를 리셋
			isRetrying = false;
			isTriggered = false; // 게임 오버 상태도 리셋
		}
	}

	// 이전 코드를 유지할 경우:
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
	private async UniTask GoMain()
	{
        if (isRetrying) return; // 이중 체크
        isRetrying = true;
        try
        {
            if (SceneLoader.Instance != null && !string.IsNullOrEmpty("Main"))
            {
                // 씬 로드가 완료될 때까지 기다림
                await SceneLoader.Instance.LoadScene("Main");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"씬 로드 실패: {ex.Message}");
        }
        finally
        {
            // 씬 로드가 실패했든 성공했든, 플래그와 게임 오버 상태를 리셋
            isRetrying = false;
            isTriggered = false; // 게임 오버 상태도 리셋
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

	// 씬 로드 시마다 참조를 갱신합니다.
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// 현재 로드된 씬 이름으로 currentSceneName을 갱신합니다. (재시도 시 사용)
		currentSceneName = scene.name;

		// 씬이 로드될 때마다 오브젝트를 다시 찾습니다. (널 체크는 필요 없습니다. 무조건 갱신합니다.)
		// 중요한 것은 씬 내에 해당 이름의 오브젝트가 정확히 하나만 존재해야 합니다.
		gameOverPanel = FindObjectIncludingInactive("GameOverOB");
		GameOverText = GameObject.Find("GameOverText");
		gameOverTextUI = FindAnyObjectByType<GameOverTextUI>();

		// 게임 오버 패널은 일반적으로 로딩 직후에는 비활성화 상태여야 합니다.
		if (gameOverPanel != null)
		{
			// 이 패널이 씬에 존재한다면, 다음 게임 오버 시퀀스를 위해 비활성화 상태로 둡니다.
			gameOverPanel.SetActive(false);
		}
		GameObject retryObject = FindObjectIncludingInactive("Retry");

		if (retryObject != null)
		{
			Button retryButton = retryObject.GetComponent<Button>();

			if (retryButton != null)
			{
				// 이전에 연결된 리스너를 모두 제거합니다. (중복 호출 방지)
				retryButton.onClick.RemoveAllListeners();

				// RetryButton() 함수를 버튼 이벤트에 동적으로 연결합니다.
				retryButton.onClick.AddListener(RetryButton);
				// 참고: RetryButton() 함수는 이미 isRetrying 체크 로직을 포함하고 있습니다.
			}
		}
        GameObject mainObject = FindObjectIncludingInactive("Main");

        if (mainObject != null)
        {
            Button mainButton = mainObject.GetComponent<Button>();

            if (mainButton != null)
            {
                // 이전에 연결된 리스너를 모두 제거합니다. (중복 호출 방지)
                mainButton.onClick.RemoveAllListeners();

                // RetryButton() 함수를 버튼 이벤트에 동적으로 연결합니다.
                mainButton.onClick.AddListener(MainButton);
                // 참고: RetryButton() 함수는 이미 isRetrying 체크 로직을 포함하고 있습니다.
            }
        }

    }
}
