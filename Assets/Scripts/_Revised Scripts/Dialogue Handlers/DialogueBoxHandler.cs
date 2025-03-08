using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxHandler : MonoBehaviour {
    public List<string> dialogueContents = new List<string>();
    public int currentLineIndex = 0;
    [HideInInspector] public Action afterDialogue;
    public Sprite npcProfile;
    public bool lastLineDisplayed = false, hasChoice;

    public string GetCurrentDialogueLine() {
        if (dialogueContents.Count == 0) {
            return "";
        }

        string currentLine = dialogueContents[currentLineIndex];

        if (currentLineIndex < dialogueContents.Count - 1) {
            currentLineIndex++;
        } else {
            lastLineDisplayed = true;
        }

        return currentLine;
    }

    void Start() {
        if (this.CompareTag("NPC")) {

        } else if (this.CompareTag("Interactable")) {

        }
    }

    public void ResetDialogue() {
        currentLineIndex = 0;
        lastLineDisplayed = false;
    }

    public bool CanClose() {
        return lastLineDisplayed;
    }
}
