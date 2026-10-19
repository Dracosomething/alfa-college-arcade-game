using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue Data")]
public class DialogueDataScriptableObject : ScriptableObject
{
    public string CharacterName;
    
    [Header("Dialogue Nodes")]
    public List<DialogueNode> DialogueNodes;
}