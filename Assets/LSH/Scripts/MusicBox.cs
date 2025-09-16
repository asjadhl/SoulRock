using UnityEngine;

public class MusicBox : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] AudioSource musicSource;
    [Header("Musics")]
    [SerializeField] AudioClip[] music;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSource.clip = music[0];
        musicSource.PlayScheduled(DotBoxGeneratorL.Instance.musicStartDspTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
