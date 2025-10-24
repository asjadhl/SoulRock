using UnityEngine;

public class EndingMusic : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] AudioClip endingClip;

    void Start()
    {
        if (FXSManager.Instance == null)
            Debug.Log("NULL!!");
        FXSManager.Instance.PlayClip(0, 4);
        if (BossState.isBoss3Dead == true)
        {
            FXSManager.Instance.PlayClip(0, 4);
        }
    }

    
}
