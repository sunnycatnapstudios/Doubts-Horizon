using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFFDialogue : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;

    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();

        string takeMeTag = "Take me bff";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
            //_PartyManager _partyManager = GameStatsManager.Instance._partyManager;
            partyManager.AddToParty(survivor);
            Destroy(gameObject);
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);

        npcDialogueHandler.dialogueContents = new List<string> {
            "Oh good, you're okay too.",
            "That was some crazy storm..",
            $"<link=\"{takeMeTag}\"><b><#d4af37>Let's go</color></b></link> and look for the others."
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }
    void Update() {
    }
    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
    }
}

