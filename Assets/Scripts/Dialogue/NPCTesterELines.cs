﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTesterELines : MonoBehaviour
{
    private DialogueBoxHandler dialogueBoxHandler;
    public _DialogueInputHandler _dialogueInputHandler;
    public List<string> dialogueLines;
    private List<string> introLines, positiveAnswer, negativeAnswer;

    void Start ()
    {
        dialogueBoxHandler = GetComponent<DialogueBoxHandler>();
        _dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<_DialogueInputHandler>();
        
        introLines = new List<string>
        {
            "...{pause}\n...........",
            "......?",
            "....Oh hi?...",
            "So...",
            "How's your{pause} day been?",
            "Sorry{pause}, I should've told you{pause}\nI need your help on something",
            "Will you help me?",
            "{dialoguePrompt:Will you help me?|Sure I can help|I don't think we can help you here}",
            "Thanks for the response"
        };
        positiveAnswer = new List<string>
        {
            "Oh that's nice, hope it stays cheerful!!!",
            "...",
            "See ya later then..."
        };
        negativeAnswer = new List<string>
        {
            ".....Oh, sorry it's not going that great",
            "Hoping it gets better then...",
            "I'll leave you to it then......",
            ".....",
            "Sorry......"
        };
        dialogueLines = introLines;

    }
    void Update ()
    {
        // if (dialogueBoxHandler.lastLineDisplayed) {dialogueLines = funnyRetort;}

        dialogueBoxHandler.dialogueContents = dialogueLines;
    }
}
