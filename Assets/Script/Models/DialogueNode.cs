using System.Collections.Generic;
using UnityEngine;

public class DialogueNode
{
    public string characterName;
    [TextArea(3, 10)]
    public string dialogueText;
    public List<DialogueChoice> choices;
}