using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("Music Bar")]
	[SerializeField] Image musicBar;
	int i = 0;
    //[Header("MusicStartDelay")]
    double nextStartTime = 5f;
    private int delayMusic = 1000;
	bool isChangingSong = false;
    public bool musicStart = false;
    BossHP hp;
    private void Awake()
    {
        hp = GameObject.FindWithTag("Stage3Boss").GetComponent<BossHP>();
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
        musicSource.clip = music[i];
        musicSource.PlayScheduled(nextStartTime); // 예약된 시간에 첫 곡 재생
		//if (FXSManager.Instance != null)
		//    FXSManager.Instance.PlayClip(0, 2, nextStartTime);
		//else
		//    gameObject.SetActive(false);

	}

    void Update()
    {
        //if(FXSManager.Instance.MusicSource.isPlaying)
        //{
        //   musicStart = true;
        //}

        if (musicSource.isPlaying)
        {
            musicStart = true;
        }
		MusicBar();
		ChangeSong();
	}

    public void MusicBar()
    {
		musicBar.fillAmount = (int)CheckRealTime.inGamerealTime / (int)musicSource.clip.length;
	}
    void ChangeSong()
    {
        if (isChangingSong) return; // 이미 노래가 변경되었으면 실행하지 않음

        if ((int)CheckRealTime.inGamerealTime == 40)
        {
            isChangingSong = true; // 노래가 변경되었음을 기록

            // 새로운 노래로 변경
            i++; // 다음 곡으로 이동
            // 새로운 곡 재생
             musicSource.clip = music[i];
            musicSource.Play();
        }
        switch (musicSource.clip.name)
        {
            case "Stage1":
                CircleHit.Instance.bpm = 82;
                break;
            case "Stage2":
                CircleHit.Instance.bpm = 83;
                break;
            case "SkeletonBoss":
                CircleHit.Instance.bpm = 92;
                break;
            case "SkeletonBossAngry":
                CircleHit.Instance.bpm = 102;
                break;
        }
    }

    private async UniTask FadeOutCurrentSong()
    {
        float fadeDuration = 1f;
         float startVolume = musicSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            await UniTask.Yield();
        }

       musicSource.Stop(); 
       musicSource.volume = startVolume;
    }
}