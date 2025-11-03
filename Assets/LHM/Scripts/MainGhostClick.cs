using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGhostClick : MonoBehaviour
{
    private Camera mainCam;

    Animator ghostAnim;

    [SerializeField] ParticleSystem ghostSurpParticle;


    void Start()
    {
        mainCam = Camera.main;
        ghostAnim = GetComponent<Animator>();
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시 검사
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 클릭된 오브젝트가 이 Ghost라면
                if (hit.transform == transform)
                {
                    Debug.Log("Ghost clicked!");
                    MainPlayState.isClicked1 = true;
                    AllReset();
                    LoadSelectScene().Forget();
                }
            }
        }
    }

    void AllReset()
    {
		BossState.isBoss1Dead = false;
        BossState.isBoss2Dead = false;
        TalkState.isTalking = false;
	   
	}

    public async UniTask LoadSelectScene()
    {
        ghostAnim.SetTrigger("Clicked");
        Vector3 ghostPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        ParticleSystem ghostParticle = Instantiate(ghostSurpParticle, ghostPos, Quaternion.identity);
        ghostParticle.Play();
        await UniTask.Delay(1000);
        SceneManager.LoadScene("StageSelect");
    }
}

