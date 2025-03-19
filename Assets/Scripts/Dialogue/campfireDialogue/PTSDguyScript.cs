using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTSDguyScript : MonoBehaviour {
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

        string Feedme = "feed soldier" + gameObject.GetHashCode().ToString();
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
                npcDialogueHandler.dialogueContents.Add($"You have {inventory.getCountofItem("Ration")} rations left");
                npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
                
            } else {
                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();
                npcDialogueHandler.dialogueContents.Add("It seems we are out, chief");
                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            }
            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

        string orNotTag = "do not feed soldier" + gameObject.GetHashCode().ToString();
        Action orNot = () => {
            Debug.Log("Or not callback.");
            statsManager.interactedWithCampfireNPC();
            statsManager.updateBedStatus();
            fedOrNot = false;
            npcDialogueHandler.lastLineDisplayed = false;
            npcDialogueHandler.currentLineIndex += 1;
            npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueContents = new List<string> {
            "The sound... The smell... It's all still there... I can't... I just need something to eat, just a little...",
             $"<link=\"{Feedme}\"><b><color=#d4af37>Feed</color></b></link>.\n...\n<link=\"{orNotTag}\"><b><color=#a40000>Or not...</color></b></link>."
        };

       // npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        if (fedOrNot) {
            npcDialogueHandler.dialogueContents = new List<string> { 
                "T-thank you... For a moment, I forgot... I was back there again..." 
            };
        } else {
            npcDialogueHandler.dialogueContents = new List<string> { 
                "It's fine I didn't deserve it anyway", 
                "Time to get going..." 
            };
        }
    }
}
