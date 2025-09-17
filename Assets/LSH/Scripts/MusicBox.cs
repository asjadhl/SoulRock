using UnityEngine;

public class MusicBox : MonoBehaviour
{
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
        Debug.LogError(CheckRealTime.Instance.inGamerealTime);
        Debug.LogWarning(nextStartTime);
        if (CheckRealTime.Instance.inGamerealTime + 0.1 >= nextStartTime && i < music.Length -1)
        {
            i++;
            musicSource.clip = music[i];
            musicSource.PlayScheduled(nextStartTime);

            switch(musicSource.clip.name)
            {
                case "Stage1":
                    DotBoxGeneratorL.Instance.bpm = 92;
                    DotBoxGeneratorR.Instance.bpm = 92;
                    break;
                case "Stage2":
                    DotBoxGeneratorL.Instance.bpm = 83;
                    DotBoxGeneratorR.Instance.bpm = 83;
                    break;
                default:
                    DotBoxGeneratorL.Instance.bpm = 117;
                    DotBoxGeneratorR.Instance.bpm = 117;
                    break;
            }

            // 다음 곡 끝나는 시점 갱신
            nextStartTime += musicSource.clip.length;
        }
    }
}
