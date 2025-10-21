using System.Security.Cryptography;
using UnityEngine;


public class GBAttack : MonoBehaviour
{
    AudioSource musicBox;
    int musicIndex;
    [SerializeField] Transform[] soundGhostPos;
    bool isAttack = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        musicBox = GameObject.FindWithTag("MusicBox").GetComponent<AudioSource>();
        musicIndex = 0;
	}

    // Update is called once per frame
    void Update()
    {
        if(!isAttack)
        {
            SoundAttack();
		}
    }

    void  SoundAttack()
    {
		isAttack = true;
        musicBox.panStereo = 0f;
        musicIndex = Random.Range(-1, 2);
        switch(musicIndex)
        {
            case -1:
                musicBox.panStereo = -1f;
                SoundAttackVector(0);
                break;
            case 0:
                musicBox.panStereo = 0f;
				SoundAttackVector(1);
				break;
            case 1:
                musicBox.panStereo = 1f;
				SoundAttackVector(2);
				break;
		}
        musicBox.panStereo = 1f;
		isAttack = false;
	}
    void SoundAttackVector(int patternNum)
    {
         switch(patternNum)
        {
            case 0:
                transform.position = soundGhostPos[patternNum].position;
				break;
            case 1:
				transform.position = soundGhostPos[patternNum].position;
				break;
            case 2:
				transform.position = soundGhostPos[patternNum].position;
				break;
        }
    }
}
