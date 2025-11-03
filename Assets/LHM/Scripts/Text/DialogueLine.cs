using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker;               // 화자 (예: Boo)
    public string localizationTable;     // Localization 테이블 이름 (예: "Tutorial_Dialogue")
    public string localizationKey;       // Localization 키 (예: "Step1_Start")
    public AudioClip sound;              // 대사별 사운드 (선택)
}
public class DialogueLineTrueORFalse
{
    static public bool TutorialTrue = false;
    static public bool stage1True = false;
    static public bool stage2True = false;
    static public bool stage3_1True = false;
    static public bool stage3_2True = false;
}
