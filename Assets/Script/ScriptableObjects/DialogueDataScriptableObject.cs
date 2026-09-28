using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue Data")]
public class DialogueDataScriptableObject : ScriptableObject
{
    public string characterName;
    
    [Header("Dialogue Nodes")]
    public List<DialogueNode> dialogueNodes;
}