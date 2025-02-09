﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueHandler : MonoBehaviour
{
    public List<string> dialogueLines;
    private int currentLineIndex = 0; 
    [HideInInspector] public delegate void AfterDialogueCall();
    [HideInInspector] public AfterDialogueCall afterDialogue;

    public string GetNextLine()
    {
        if (currentLineIndex < dialogueLines.Count)
        {
            return dialogueLines[currentLineIndex++];
        }
        else
        {
            return null;  // No more lines
        }
    }

    public void ResetDialogue()
    {
        currentLineIndex = 0;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
