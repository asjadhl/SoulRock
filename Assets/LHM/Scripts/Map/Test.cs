using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
 
 
public class Test : MonoBehaviour
{
    public GameObject ga;
    public CancellationTokenSource cts;
    public void Start()
    {

        cts = new();
    }
    public void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(gameObject);
            Tes(cts.Token).Forget();
        }
    }

    public async UniTaskVoid Tes(CancellationToken t)
    {
        await UniTask.WaitForSeconds(3f, cancellationToken: t);
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        cts.Cancel();
    }

}
