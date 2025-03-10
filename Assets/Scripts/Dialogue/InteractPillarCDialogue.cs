﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPillarCDialogue : MonoBehaviour {
    private DialogueBoxHandler NPCDialogueHandler;
    private InteractPrompt InteractPrompt;
    public List<string> dialogueLines;
    private List<string> introLines, funnyRetort;

    void Awake() {
        NPCDialogueHandler = GetComponent<DialogueBoxHandler>();
        InteractPrompt = GetComponent<InteractPrompt>();

        introLines = new List<string>
        {
            "Testng 1, 2\nTesting 1, 2...",
            "Yep, this seems to be working",
            "...",
            "Hello Player,\nWelcome to the Demo World",
            "Feel free to explore our selection of mechanics"
        };
        funnyRetort = new List<string>
        {
            "What?\nNever seen a talking pile of rocks before?"
        };
        dialogueLines = introLines;
    }
    void Update() {
        if (InteractPrompt.dialogueFinished) { dialogueLines = funnyRetort; }

        NPCDialogueHandler.dialogueContents = dialogueLines;
    }
}
