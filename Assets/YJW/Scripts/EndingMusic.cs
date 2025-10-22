using UnityEngine;

public class EndingMusic : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] AudioClip endingClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(BossState.isBoss3Dead == true)
        {
            audioSource.clip = endingClip;
            audioSource.Play();
        }
    }

    
}
