using UnityEngine;

public class MusicBox : MonoBehaviour
{
    //첫번째 노래 딜레이 5초  (5초)
    //두번째 노래 딜레이 +5초 (10초)
    //세번째 동일
    [Header("AudioSource")]
    [SerializeField] AudioSource musicSource;
    [Header("Musics")]
    [SerializeField] AudioClip[] music;

    int i = 0;
    double nextStartTime;

    void Start()
    {

        // 첫 곡 예약
        musicSource.clip = music[i];
        musicSource.PlayScheduled(nextStartTime + DotBoxGeneratorL.Instance.startDelay);

        // 다음 시작 시간 예약
        nextStartTime += musicSource.clip.length;
        
    }

    void Update()
    {
        ChangeSong();
    }
    void ChangeSong()
    {
        Debug.LogError((int)CheckRealTime.Instance.inGamerealTime);
        Debug.LogWarning($"다음곡 시작 {nextStartTime+DotBoxGeneratorL.Instance.startDelay}");
        if (CheckRealTime.Instance.inGamerealTime + 0.1 >= nextStartTime && i < music.Length - 1)
        {
            i++;
            musicSource.clip = music[i];
            musicSource.PlayScheduled(nextStartTime + DotBoxGeneratorL.Instance.startDelay);

            switch (musicSource.clip.name)
            {
                case "Stage1":
                    DotBoxGeneratorL.Instance.bpm = 82;
                    DotBoxGeneratorR.Instance.bpm = 82;
                    break;
                case "Stage2":
                    DotBoxGeneratorL.Instance.bpm = 83;
                    DotBoxGeneratorR.Instance.bpm = 83;
                    break;
                case "Stage3":
                    DotBoxGeneratorL.Instance.bpm = 117;
                    DotBoxGeneratorR.Instance.bpm = 117;
                    break;
                default:
                    Debug.Log("No Song");
                    break;
            }

            // 다음 곡 끝나는 시점 갱신
            nextStartTime += musicSource.clip.length;
        }
    }
}
