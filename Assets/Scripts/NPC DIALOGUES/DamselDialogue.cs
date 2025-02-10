﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamselDialogue : MonoBehaviour
{
    private NPCDialogueHandler NPCDialogueHandler;
    private InteractPrompt InteractPrompt;
    public List<string> dialogueLines;
    private List<string> introLines, funnyRetort;

    void Start ()
    {
        NPCDialogueHandler = GetComponent<NPCDialogueHandler>();
        InteractPrompt = GetComponent<InteractPrompt>();
        
        introLines = new List<string>
        {
            "Thank you so much for saving me from that beast!",
            "My hero <3"
        };
        dialogueLines = introLines;
        NPCDialogueHandler.dialogueLines = dialogueLines;
        NPCDialogueHandler.afterDialogue = new NPCDialogueHandler.AfterDialogueCall(AfterDialogue);
    }
    void Update ()
    {
    }
    void AfterDialogue() {
        Debug.Log("got hook");
        PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
        partyManager.AddToParty("MemberB");
        Destroy(gameObject);
    }
}
