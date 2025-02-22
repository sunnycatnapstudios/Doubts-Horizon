using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxHandler : MonoBehaviour
{
    public List<string> dialogueContents = new List<string>();
    public int currentLineIndex = 0;
    public Sprite npcProfile;
    public bool lastLineDisplayed = false, hasChoice;

    void Start()
    {
        if (this.CompareTag("NPC"))
        {

        }
        else if (this.CompareTag("Interactable"))
        {
            
        }
    }

    public string GetCurrentDialogueLine()
    {
        if (dialogueContents.Count == 0) return "";
        
        string currentLine = dialogueContents[currentLineIndex];

        if (currentLineIndex < dialogueContents.Count - 1) {currentLineIndex++;}
        else {lastLineDisplayed = true;}

        return currentLine;
    }
    public bool CanClose() {return lastLineDisplayed;}
}
