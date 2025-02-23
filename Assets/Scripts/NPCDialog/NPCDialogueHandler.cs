using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueHandler : MonoBehaviour {
    public List<string> dialogueLines;
    private int currentLineIndex = 0; 
    [HideInInspector] public Action afterDialogue;

    public string GetNextLine() {
        if (currentLineIndex < dialogueLines.Count) {
            return dialogueLines[currentLineIndex++];
        } else {
            return null;  // No more lines
        }
    }

    public void ResetDialogue() {
        currentLineIndex = 0;
    }

    void Awake() {
    }
    
    void Start() {
    }

    void Update() {
    }
}
