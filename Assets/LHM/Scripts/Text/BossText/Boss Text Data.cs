using UnityEngine;

[CreateAssetMenu(fileName = "BossTextData", menuName = "SoulRock/BossTextData")]
public class BossTextData : ScriptableObject
{
    public BossDialogueSet[] bossDialogues;
}

[System.Serializable]
public class BossDialogueSet
{
    public string bossName;         // 예: "Boss1", "Boss2"
    public BossLine[] deathLines;   // 죽었을 때 대사
}

