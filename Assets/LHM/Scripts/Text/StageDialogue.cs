using UnityEngine;

[System.Serializable]
public class StageDialogue
{
    [TextArea(2, 5)]
    public string[] dialogues;
}

[CreateAssetMenu(fileName = "DialogueData", menuName = "SoulRock/DialogueData")]
public class StageDialogueData : ScriptableObject
{
    public StageDialogue stage1;
    public StageDialogue stage2;
    public StageDialogue stage3;
    public StageDialogue stage4;
}
