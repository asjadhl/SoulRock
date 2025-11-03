using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동에 필요

public class GhostTrainingLoader : MonoBehaviour
{
    private Camera mainCam;

    Animator ghostAnim;

    [SerializeField] ParticleSystem ghostSurpParticle;

    void Start()
    {
        ghostAnim = GetComponent<Animator>();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    Debug.Log("오브젝트 클릭 감지됨");
                    MainGhostTrainingState.isClicked = true;
                    LoadTrainingScene().Forget();
                }
            }
        }
    }

    public async UniTask LoadTrainingScene()
    {
        ghostAnim.SetTrigger("Clicked");
        Vector3 ghostPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        ParticleSystem ghostParticle = Instantiate(ghostSurpParticle, ghostPos, Quaternion.identity);
        ghostParticle.Play();
        await UniTask.Delay(1000);
        SceneManager.LoadScene("TraingRoom");
    }

}
