using UnityEngine;
using UnityEngine.UI;

public class NormalMusicBox : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] AudioSource musicSource;
    [Header("Musics")]
    [SerializeField] AudioClip music;
	[Header("Music Bar")]
	[SerializeField] Image musicBar;
	double nextStartTime = 5f;
    public bool MusicFin = false;
	private PlayerHP playerHPSc;
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
		playerHPSc = FindAnyObjectByType<PlayerHP>();
		double delay = 5.5;
        nextStartTime = AudioSettings.dspTime + delay; // 현재 DSP 시간에 지연 시간 추가
         musicSource.clip = music;
         musicSource.PlayScheduled(nextStartTime); // 예약된 시간에 첫 곡 재생
	}
	private void Update()
	{
        if (!playerHPSc.isPlayerDead)
        {
			MusicBar();
			CheckMusicFinished();
		}
		else
		{
			musicSource.Stop();
		}
	}
	public void MusicBar()
	{
		musicBar.fillAmount = (float)CheckRealTime.inGamerealTime / (musicSource.clip.length+6f);
	}

    void CheckMusicFinished()
    {
        if(musicBar.fillAmount >= 1)
        {
            MusicFin = true;
		}
    }
}
