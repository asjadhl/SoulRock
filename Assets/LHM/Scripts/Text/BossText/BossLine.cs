using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class BossLine
{
    [Header("Localization Info")]
    public string localizationTable;  // ex: "BossDeath"
    public string localizationKey;    // ex: "Boss1_DeathLine1"

    [Header("Fallback (optional)")]
    [TextArea(2, 5)] public string text;
    public AudioClip sound;
}