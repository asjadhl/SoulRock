using Cysharp.Threading.Tasks;
using UnityEngine;

public class MusicBox : MonoBehaviour
{
	/*
     첫곡:  0~ 182초
37 58 ㅃ

72 88 	ㄴ

93 113 ㅃ

136 148 ㄴ

158 187 ㅃ

182 ~ 다음곡
     */
	[Header("AudioSource")]
    [SerializeField] AudioSource musicSource;
    [Header("Musics")]
    [SerializeField] AudioClip[] music;

    int i = 0;
    //[Header("MusicStartDelay")]
    double nextStartTime = 5f;
    private int delayMusic = 2000;
    void Start()
    {
        // 첫 곡 예약
        musicSource.clip = music[i];
		musicSource.PlayScheduled(nextStartTime  + DotBoxGeneratorL.Instance.musicStartDspTime);

        // 다음 시작 시간 예약
        nextStartTime += musicSource.clip.length;
        
    }

    void Update()
    {
        _ = ChangeSong();
    }
    private async UniTask  ChangeSong()
    {
        //Debug.LogError((int)CheckRealTime.Instance.inGamerealTime);
        //Debug.LogWarning($"다음곡 시작 {nextStartTime+DotBoxGeneratorL.Instance.startDelay}");
        if (CheckRealTime.inGamerealTime + 0.1 >= nextStartTime && i < music.Length - 1)
        {
            i++;
            musicSource.clip = music[i];
            await UniTask.Delay(delayMusic);
            musicSource.PlayScheduled(nextStartTime);

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
