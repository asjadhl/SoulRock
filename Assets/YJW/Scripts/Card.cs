using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Object/Card Data")]
public class Card : ScriptableObject
{
    public Sprite icon;
    public int num;
    public int shpae; // 1은 s, 2는 d, 3은 h, 4는 c

}

