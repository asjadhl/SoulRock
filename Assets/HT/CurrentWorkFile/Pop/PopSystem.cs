using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PopSystem : MonoBehaviour
{
    private Lazy<CancellationTokenSource> cts = new(() => new CancellationTokenSource());

    // ✅ Maps prefab hash → panel instance
    private Dictionary<string, GameObject> panelMap = new();

    private Lazy<Transform> m_canvas = new(() =>
    {
        var canvasObj = GameObject.FindAnyObjectByType<Canvas>();
        if (canvasObj == null)
        {
            Debug.LogError("No Canvas Found in Scene!");
            return null;
        }
        return canvasObj.transform;
    });

    public RuntimeAnimatorController runtimeAnimatorController;

    // ✅ Show popup
    public void PopUp(GameObject prefab,string key)
    {
        if (m_canvas.Value == null) return;

     

        // ✅ Panel already exists → just reactivate it
        if (panelMap.TryGetValue(key, out GameObject existingPanel))
        {
            existingPanel.SetActive(true);
            Animator anim = existingPanel.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("Show", 0, 0f);
            }
            var canvasgroup1 = existingPanel.GetComponent<CanvasGroup>();
            canvasgroup1.interactable = true;
            return;
        }

     
        GameObject panel = Instantiate(prefab, m_canvas.Value);
        panel.SetActive(true);
        var canvasgroup = panel.AddComponent<CanvasGroup>();
        canvasgroup.interactable = true;
        Animator animator = panel.GetComponent<Animator>();
        if (animator == null)
            animator = panel.AddComponent<Animator>();

        animator.runtimeAnimatorController = runtimeAnimatorController;
        animator.Play("Show", 0, 0f);

        // ✅ Remember panel by hash
        panelMap.Add(key, panel);
   
    }

    // ✅ Close popup (animate + disable, not destroy)
    public void PopDown(string key)
    {
        

        if (panelMap.TryGetValue(key, out GameObject panel))
        {
          var canvasgroup =  panel.GetComponent<CanvasGroup>();
            canvasgroup.interactable = false;
            
            Animator animator = panel.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("Close", 0, 0f);
                HideAfterAnimation(animator, panel, cts.Value.Token).Forget();
            }
            else
            {
                // Fallback if no animator → just hide
                panel.SetActive(false);
            }
        }
        
    }

    private async UniTaskVoid HideAfterAnimation(Animator animator, GameObject panel, CancellationToken token)
    {
        Debug.Log("???");
        try
        {
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

    private void OnDestroy()
    {
        if (cts.IsValueCreated)
        {
            cts.Value.Cancel();
            cts.Value.Dispose();
        }
    }
}
