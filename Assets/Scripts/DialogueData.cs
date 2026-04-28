using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(2, 5)]
    public string text;
    public bool hasChoice;
    public List<DialogueChoice> choices;
}

[Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string nextNodeId;      // For branching dialogue
    public int humanityModifier;    // -1 for AI, +1 for human
    public bool marksIslandComplete;
    public int completedIslandIndex = -1;
}

[Serializable]
public class DialogueNode
{
    public string nodeId;
    public List<DialogueLine> lines;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public List<DialogueNode> nodes;
    public string startNodeId = "start";
    public bool completesIslandOnEnd;
    public int completedIslandIndex = -1;
}