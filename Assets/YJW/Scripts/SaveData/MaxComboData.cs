using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MaxComboData", menuName = "MaxComboData")]
public class MaxComboData : ScriptableObject
{
    public int maxCombo;
    public int maxComboValue
    {
        get { return maxCombo; }
        set
        {
            if (value > maxCombo)
            {
                maxCombo = value;
            }
            else
            {
                return;
            }
        }
    }
}
