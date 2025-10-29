using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummySpawner : MonoBehaviour
{
    public event Action OnDummyDead;
    public bool isDummyDead = false;    
    private bool deathHandled = false;
    public bool getDummyHit = false;
    Animator animator;
    public int dummyHp = 3;
    TutorialManager tutorialManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!deathHandled && dummyHp <= 0)
        {
            deathHandled = true;
            isDummyDead = true;
            animator.SetTrigger("Dead");
            OnDummyDead?.Invoke();
            Dummyreset();
        }
    }
    public void getDummyDamage()
    {
        if (deathHandled) return;
        animator.SetTrigger("Hit");
        Debug.LogWarning("더미 맞음");
        dummyHp--;
    }
    private async UniTask DummyGen()
    {
        await UniTask.Delay(3000);  // 3초 뒤 부활
        dummyHp = 3;
        isDummyDead = false;
        deathHandled = false;
        animator.SetTrigger("Stand");
    }
    private async UniTask Dummyreset()
    {
        await UniTask.Delay(3000);  // 1초 뒤 부활
        _ = DummyGen();
    }
    public void SkipScene()
    {
        SceneManager.LoadScene("StageSelect");
    }
}
