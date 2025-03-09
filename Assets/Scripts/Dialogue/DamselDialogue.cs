using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamselDialogue : MonoBehaviour {
    private DialogueBoxHandler NPCDialogueHandler;
    private InteractPrompt InteractPrompt;
    public List<string> dialogueLines;
    private List<string> introLines, funnyRetort;

    public Survivor Survivor;

    void Start() {
        NPCDialogueHandler = GetComponent<DialogueBoxHandler>();
        InteractPrompt = GetComponent<InteractPrompt>();

        introLines = new List<string>
        {
            "Thank you so much for saving me from that beast!",
            "My hero <3"
        };
        dialogueLines = introLines;
        NPCDialogueHandler.dialogueContents = dialogueLines;
        NPCDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }
    void Update() {
    }
    void AfterDialogue() {
        Debug.Log("got hook");
        PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
        partyManager.AddToParty(Survivor);
        Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();





        Destroy(gameObject);
    }
}
