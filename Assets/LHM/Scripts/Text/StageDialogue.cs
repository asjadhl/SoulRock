
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;
    public AudioClip sound; //각 대사별 사운드
}

[System.Serializable]
public class StageDialogue
{
    public DialogueLine[] dialogues;
}

[CreateAssetMenu(fileName = "DialogueData", menuName = "SoulRock/DialogueData")]
public class StageDialogueData : ScriptableObject
{
    public StageDialogue stage1;
    public StageDialogue stage2;
    public StageDialogue stage3;
}