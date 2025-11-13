using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGhostClick : MonoBehaviour
{
    private Camera mainCam;

    Animator ghostAnim;

    [SerializeField] ParticleSystem ghostSurpParticle;

    bool isClickedOnce = false;
    public bool isTraining = true;
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

                    if (isClickedOnce)
                        return;

                    isClickedOnce = true;

                    AllReset();
                    LoadSceneToTraining();

                }
            }
        }
    }

    void AllReset()
    {
		BossState.isBoss1Dead = false;
        BossState.isBoss2Dead = false;
        BossState.isBoss3Dead = false;
        TalkState.isTalking = false;
	   
	}


    private void LoadSceneToTraining()
    {
        ghostAnim.SetTrigger("Clicked");
        Vector3 ghostPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        ParticleSystem ghostParticle = Instantiate(ghostSurpParticle, ghostPos, Quaternion.identity);
        ghostParticle.Play();
        Invoke("LoadTheSceneLoaderTutorial", 1f);
    }
    private void LoadSelectedScene()
    {
        ghostAnim.SetTrigger("Clicked");
        Vector3 ghostPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        ParticleSystem ghostParticle = Instantiate(ghostSurpParticle, ghostPos, Quaternion.identity);
        ghostParticle.Play();
        Invoke("LoadTheSceneLoaderSelectScene", 1f);
    }
    private void LoadTheSceneLoaderTutorial()
    {
        SceneLoader.Instance.LoadScene("TutorialTrainingRoom");
    }
    private void LoadTheSceneLoaderSelectScene()
    {
        SceneLoader.Instance.LoadScene("StageSelect");
    }
    [Obsolete]
    private async UniTask LoadSceneToTraning()
    {
        ghostAnim.SetTrigger("Clicked");
        Vector3 ghostPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        ParticleSystem ghostParticle = Instantiate(ghostSurpParticle, ghostPos, Quaternion.identity);
        ghostParticle.Play();
        await UniTask.Delay(1000);
        SceneLoader.Instance.LoadScene("TutorialTrainingRoom");
    }
    [Obsolete]
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

