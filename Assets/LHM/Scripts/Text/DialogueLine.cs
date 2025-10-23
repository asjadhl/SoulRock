using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;          // 대사
    public AudioClip sound;      // 대사별 사운드
}
