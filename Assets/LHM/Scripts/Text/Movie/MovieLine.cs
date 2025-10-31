using UnityEngine;

[System.Serializable]
public class MovieLine
{
    [TextArea(2, 5)]
    public string text;
    public AudioClip sound;
}
