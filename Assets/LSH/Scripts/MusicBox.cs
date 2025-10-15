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
    Stage3Boss stage3boss;
    [Header("ShowTimer")]
    [SerializeField] Text text;
    int i = 0;
    //[Header("MusicStartDelay")]
    double nextStartTime = 5f;
    private int delayMusic = 2000;
	bool isChangingSong = false;
    public bool musicStart = false;
    public int clearTimer;
    //BossHP hp;
    private void Awake()
    {
       stage3boss = GameObject.FindWithTag("Stage3Boss").GetComponent<Stage3Boss>();
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
        
    }

    void Update()
    {
		if (musicSource.isPlaying)
		{
			musicStart = true;
			CheckMusicSong();
		}
		_ = ChangeSong();
		if (musicStart)
		{
			text.text = FormatTime(clearTimer);
		}
	}
    //   private async UniTask  ChangeSong()
    //   {
    //	//Debug.LogError((int)CheckRealTime.Instance.inGamerealTime);
    //	//Debug.LogWarning($"다음곡 시작 {nextStartTime+DotBoxGeneratorL.Instance.startDelay}");
    //	if (isChangingSong) return;
    //	if (CheckRealTime.inGamerealTime + 0.1 < nextStartTime || i >= music.Length - 1) return;

    //	isChangingSong = true;

    //	i++;
    //	musicSource.clip = music[i];
    //	await UniTask.Delay(delayMusic);
    //	musicSource.PlayScheduled(nextStartTime);

    //	switch (musicSource.clip.name)
    //	{
    //		case "Stage1":
    //			CircleHit.Instance.bpm = 82;
    //			break;
    //		case "Stage2":
    //			CircleHit.Instance.bpm = 83;
    //			break;
    //		case "Stage3":
    //			CircleHit.Instance.bpm = 117;
    //			break;
    //	}

    //	nextStartTime += musicSource.clip.length;
    //	isChangingSong = false;
    //}
    private async UniTask ChangeSong()
    {
        if (isChangingSong) return; // 이미 노래가 변경되었으면 실행하지 않음

        // 보스 HP가 30 이하인지 확인
        if (stage3boss.isAngry)
        {
            isChangingSong = true; // 노래가 변경되었음을 기록

            // 새로운 노래로 변경
            i++; // 다음 곡으로 이동
            if (i >= music.Length) i = 0; // 인덱스 초과 방지
            // 현재 곡 페이드아웃 (선택 사항)
            await FadeOutCurrentSong();

            // 새로운 곡 재생
            musicSource.clip = music[i];
            await UniTask.Delay(delayMusic); // 약간의 딜레이 추가
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
            case "Stage3":
                CircleHit.Instance.bpm = 117;
                break;
            case "BossAngry2":
                CircleHit.Instance.bpm = 150;
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
	void CheckMusicSong()
	{
		if (musicSource.clip == null) return;

		float remainingTime = musicSource.clip.length - musicSource.time;

		clearTimer = Mathf.Max(0, Mathf.RoundToInt(remainingTime));
	}
	string FormatTime(int seconds)
	{
		int minutes = seconds / 60;
		int secs = seconds % 60;
		return $"{minutes:00}:{secs:00}";
	}
}
