using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrphanCampfireScript : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;
    private bool fedOrNot;
    private Inventory inventory;
    private GameStatsManager statsManager;

    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        statsManager = GameStatsManager.Instance;

        string Feedme = "feed orphan";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

            if (inventory.hasItemByName("Ration")) {
                survivor.Fed = true;
                fedOrNot = true;
                inventory.removeItemByName("Ration");
                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();

                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                npcDialogueHandler.afterDialogue = AfterDialogue;  
                npcDialogueHandler.dialogueContents.Add($"You have {inventory.getCountofItem("Ration")} rations left.");
                
            } else {
                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();

                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                npcDialogueHandler.afterDialogue = AfterDialogue;  

                npcDialogueHandler.dialogueContents.Add("Oh no, we are out!!");
            }

            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

        string orNotTag = "do not feed orphan";
        Action orNot = () => {
            Debug.Log("Or not callback.");
            statsManager.interactedWithCampfireNPC();
            statsManager.updateBedStatus();
            fedOrNot = false;

            npcDialogueHandler.lastLineDisplayed = false;
            npcDialogueHandler.currentLineIndex += 1;
            npcDialogueHandler.afterDialogue = AfterDialogue; 
            
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueContents = new List<string> {
            "I... I haven't eaten in so long... Please, do you have anything?",
            $"<link=\"{Feedme}\"><b><color=#d4af37>Feed</color></b></link>",
            $"<link=\"{orNotTag}\"><b><color=#a40000>Or not...</color></b></link>"
        };

        npcDialogueHandler.afterDialogue = AfterDialogue;
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        if (fedOrNot) {
            npcDialogueHandler.dialogueContents = new List<string> { "T-thank you... I was so scared I'd never eat again..." };
        } else {
            npcDialogueHandler.dialogueContents = new List<string> { "Oh... okay.", "I guess I'll keep looking..." };
        }
    }
}
