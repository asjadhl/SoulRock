using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class CloneMove : MonoBehaviour
{
    public float moveSpeed = 3.8f;

    private ParticleManager particleManager;
    private bool isInitialized = false;
    
    private void Awake()
    {
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
    }
    private void Start()
    {
        isInitialized = true;
    }
    private void Update()
    {
        UpdateBossRun();
    }

    private void UpdateBossRun()
    {
        transform.position += moveSpeed * Time.fixedDeltaTime * -transform.forward;
    }
    public void OnHit()
    {
        if (!isInitialized) return; //시작 폭발 방지임ㅋㅋ
        Vector3 effectPos = transform.position + new Vector3(0, 1f, 2f);
        particleManager.PlayRealGhostEffect(effectPos);
        gameObject.SetActive(false);
    }
}
