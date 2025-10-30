using UnityEngine;
[System.Serializable]
public class BossLine
{
    [TextArea(2, 5)]
    public string text;    
    public AudioClip sound;     
    public string speaker;     
}