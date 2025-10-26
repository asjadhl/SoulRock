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
        ChangeSong();
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
    void ChangeSong()
    {
        if (isChangingSong) return; // 이미 노래가 변경되었으면 실행하지 않음

        if ((int)CheckRealTime.inGamerealTime == 15)
        {
			Debug.LogError("노래변경");
            isChangingSong = true; // 노래가 변경되었음을 기록

            // 새로운 노래로 변경
            i++; // 다음 곡으로 이동
            //if (i > music.Length) i = 0; // 인덱스 초과 방지
            // 현재 곡 페이드아웃 (선택 사항)
            //await FadeOutCurrentSong();

            // 새로운 곡 재생
             musicSource.clip = music[i];
           
            //await UniTask.Delay(delayMusic); // 약간의 딜레이 추가
            musicSource.Play();
            //FXSManager.Instance.PlayClip(0, music[i]);
        }
        switch (musicSource.clip.name)
            //switch (FXSManager.Instance.MusicSource.clip.name)
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
        //float startVolume = FXSManager.Instance.MusicVolume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
          //  FXSManager.Instance.MusicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            //FXSManager.Instance.MusicVolume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            await UniTask.Yield();
        }

       musicSource.Stop(); 
       // FXSManager.Instance.MusicSource.Stop();
       musicSource.volume = startVolume;
      //  FXSManager.Instance.MusicVolume = startVolume;
        //FXSManager.Instance.MusicSource.volume = startVolume;
    }
    //void CheckMusicTime()
    //{
    //    Debug.LogWarning(music)
    //}
}