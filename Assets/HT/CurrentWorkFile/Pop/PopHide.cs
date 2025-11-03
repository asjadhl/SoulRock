using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class PopHide : MonoBehaviour
{

    System.Lazy<PopSystem> popSystem = new System.Lazy<PopSystem>(() =>
    {
        PopSystem system = GameObject.FindAnyObjectByType<PopSystem>();
        if (system == null)
            Debug.LogError("PopSystem not found in the scene!");
        return system;
    });
    System.Lazy<Animator> animator;
   private RuntimeAnimatorController _originalruntimeAnimatorController;
    System.Lazy<CancellationTokenSource> cts = new Lazy<CancellationTokenSource>(() =>
    {
        return new CancellationTokenSource();
    });

    System.Lazy<Button> button;

    public void Open()
    {
       
        animator.Value.runtimeAnimatorController = popSystem.Value.runtimeAnimatorController;
        gameObject.SetActive(true);
        animator.Value.Play("Show", 0, 0f);
        
       
    }

    public void Awake()
    {
        
        animator = new System.Lazy<Animator>(() => 
        {
           var animator = GetComponent<Animator>();
            _originalruntimeAnimatorController = animator.runtimeAnimatorController;
            return animator;

        });
        button = new(() => { 
        return GetComponent<Button>();
        }
        );
    }

    public void Close()
    {
        
      
        HideAfterAnimation(animator.Value,gameObject,cts.Value.Token).Forget();
        
    }
    private void OnDisable()
    {
        if (cts.Value != null)
        {
            cts.Value.Cancel();
            cts.Value.Dispose();
            cts = new Lazy<CancellationTokenSource>(() =>
            {
                return new CancellationTokenSource();
            });
        }
    }
    private void OnDestroy()
    {
        if (cts.Value != null)
        {
            cts.Value.Cancel();
            cts.Value.Dispose();
        }
    }
    private async UniTaskVoid HideAfterAnimation(Animator animator, GameObject panel, CancellationToken token)
    {

        try
        {
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Selected"),
               cancellationToken: token);
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1,cancellationToken: token);
            button.Value.enabled = false;
            animator.runtimeAnimatorController = popSystem.Value.runtimeAnimatorController;
            animator.Play("Close", 0, 0f);
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Close"),
                                    cancellationToken: token);
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f,
                                    cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Close animation canceled — forcing hide.");
        }
        finally
        {
            if (panel != null)
                panel.SetActive(false);


        }
    }
 
}
