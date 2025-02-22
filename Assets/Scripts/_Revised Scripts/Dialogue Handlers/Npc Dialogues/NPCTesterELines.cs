using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTesterELines : MonoBehaviour
{
    private DialogueBoxHandler dialogueBoxHandler;
    public List<string> dialogueLines;
    private List<string> introLines, positiveAnswer, negativeAnswer;

    void Start ()
    {
        dialogueBoxHandler = GetComponent<DialogueBoxHandler>();
        
        introLines = new List<string>
        {
            "...",
            "...........",
            "......?",
            "....Oh hi?...",
            "So...",
            "How's your day been?"
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
