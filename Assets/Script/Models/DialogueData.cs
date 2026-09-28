using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Data", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string characterName;
    
    [Header("Dialogue Nodes")]
    public List<DialogueNode> dialogueNodes = new List<DialogueNode>();
}