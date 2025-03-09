using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTesterDLines : MonoBehaviour
{
    private DialogueBoxHandler dialogueBoxHandler;
    public List<string> dialogueLines;
    private List<string> introLines, funnyRetort;

    void Start ()
    {
        dialogueBoxHandler = GetComponent<DialogueBoxHandler>();
        
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
    void Update ()
    {
        if (dialogueBoxHandler.lastLineDisplayed) {dialogueLines = funnyRetort;}

        dialogueBoxHandler.dialogueContents = dialogueLines;
    }
}
