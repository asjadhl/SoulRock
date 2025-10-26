using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectGhostMove : MonoBehaviour
{
	public float moveSpeed = 4f;
	[SerializeField] Transform playerPos;
	public float chaseStartDistance = 10f;
	public float disableDistance = 1f;
    [SerializeField] BoxCollider clownCol;
    [SerializeField] BoxCollider skull;
    bool lastStageOn = false;
    void Awake()
    {
        if (playerPos == null)
            playerPos = GameObject.FindWithTag("Player")?.transform; //물음표 붙이면 null이면 null로 반환 개꿀임.
    }

    // Update is called once per frame
    private void Update()
    {
        // 테스트용 입력
        if (Input.GetKeyDown(KeyCode.R))
            BossState.isBoss1Dead = true;
        if (Input.GetKeyDown(KeyCode.T))
            BossState.isBoss2Dead = true;

        // 두 보스가 모두 죽었을 때 한 번만 실행
        if (BossState.isBoss1Dead && BossState.isBoss2Dead && !lastStageOn)
        {
            lastStageOn = true;
            MoveScene().Forget();
        }
    }
    private async UniTask GhostMove(float durationSeconds)
    {
        float elapsed = 0f;

        while (elapsed < durationSeconds)
        {
            if (playerPos == null) break;

            float distance = Vector3.Distance(transform.position, playerPos.position);

            if (distance > disableDistance)
            {
                Vector3 dir = (playerPos.position - transform.position).normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;
            }
            else if (distance <= disableDistance)
            {
                transform.SetParent(playerPos.transform, true);
                Debug.Log("Ghost reached player");
                break;
            }

            elapsed += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update); //다음 Update 루프 타이밍 때 다시 실행
        }
    }


    private async UniTaskVoid MoveScene()
    {
        Debug.Log("28초 동안 Ghost 이동 시작");
        await UniTask.Delay(5000);
        clownCol.GetComponent<BoxCollider>().enabled = true;
        skull.GetComponent<BoxCollider>().enabled = true;
        await GhostMove(5f); 
        Debug.Log("28초 후 LastStage로 이동");

        await UniTask.Delay(28000);
        SceneManager.LoadScene("LastStage");
    }
}
