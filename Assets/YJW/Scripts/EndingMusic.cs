using UnityEngine;

public class EndingMusic : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] AudioClip endingClip;

    void Start()
    {
        FXSManager.Instance.PlayClip(0, 3);
        if(BossState.isBoss3Dead == true)
        {
            FXSManager.Instance.PlayClip(0, 4);
        }
    }

    
}
