using Unity.VisualScripting;
using UnityEngine;

public enum Shape
{
    S,
    H,
    C,
    D
}

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Object/Card Data")]
public class Card : ScriptableObject
{
    public Sprite icon;

    public Shape shape;

}

