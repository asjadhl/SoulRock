using UnityEngine;

public class NormalMusicBox : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] AudioSource musicSource;
    [Header("Musics")]
    [SerializeField] AudioClip music;

    double nextStartTime = 5f;
    private void Awake()
    {
    }
    void Start()
    {
        //      // 첫 곡 예약
        //      musicSource.clip = music[i];
        //musicSource.PlayScheduled(nextStartTime  + CheckRealTime.Instance.startTime);

        //      // 다음 시작 시간 예약
        //      nextStartTime += musicSource.clip.length;
        double delay = 5.0;
        nextStartTime = AudioSettings.dspTime + delay; // 현재 DSP 시간에 지연 시간 추가
        //musicSource.clip = music;
        //musicSource.PlayScheduled(nextStartTime); // 예약된 시간에 첫 곡 재생

        if (FXSManager.Instance != null)
            FXSManager.Instance.PlayClip(0, 1, nextStartTime);
    }
}
