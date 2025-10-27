using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummySpawner : MonoBehaviour
{
    public bool getDummyHit = false;
    Animator animator;
    int dummyHp = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dummyHp <= 0)
        {
            animator.SetTrigger("Dead");
            Debug.LogWarning("더미죽음");
            _ = DummyGen();
        }
    }
    public void getDummyDamage()
    {
        animator.SetTrigger("Hit");
        Debug.LogWarning("더미 맞음");
        dummyHp--;
    }
    private async UniTask DummyGen()
    {
        dummyHp = 3;
        await UniTask.Delay(10000);
        animator.SetTrigger("Stand");
    }

    public void SkipScene()
    {
        SceneManager.LoadScene("StageSelect");
    }
}
