using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallDialogueContents : MonoBehaviour
{
    public List<string> dialogueContents = new List<string>();
    public int currentLineIndex = 0;

    public string GetCurrentDialogueLine()
    {
        if (dialogueContents.Count == 0) return "";
        
        if (currentLineIndex < dialogueContents.Count -1) {
            return dialogueContents[currentLineIndex++];
        }
        return dialogueContents[dialogueContents.Count - 1];
    }
}
