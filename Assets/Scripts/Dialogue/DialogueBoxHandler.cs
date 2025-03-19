using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxHandler : MonoBehaviour {
    public List<string> dialogueContents = new List<string>();
    public int currentLineIndex = 0;
    [HideInInspector] public Action afterDialogue, beforeDialogue;
    public Sprite npcProfile;
    public bool lastLineDisplayed = false, hasChoice;

    private AudioClip _sfxTalkingClip; // Should be assigned in a dialog script

    public void SetSfxTalkingClip(AudioClip clip) {
        _sfxTalkingClip = clip;
    }

    public AudioClip SfxTalkingClip {
        get { return _sfxTalkingClip; }
    }

    // Have both to ease compatibility, should remove one
    public string GetNextLine() {
        Debug.Log("GetNextLine is deprecated, use GetCurrentDialogueLine, it is subtlely different.");
        if (currentLineIndex < dialogueContents.Count) {
            return dialogueContents[currentLineIndex++];
        } else {
            return null; // No more lines
        }
    }

    public string GetCurrentDialogueLine() {
        if (dialogueContents.Count == 0) {
            return "";
        }

        if (currentLineIndex >= dialogueContents.Count) {
            return dialogueContents[dialogueContents.Count - 1];
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
