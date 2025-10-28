using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;          // 대사
    public AudioClip sound;      // 대사별 사운드
}
public class DialogueLineTrueORFalse
{
    static public bool TutorialTrue = false;
    static public bool stage1True = false;
    static public bool stage2True = false;
    static public bool stage3_1True = false;
    static public bool stage3_2True = false;
}
