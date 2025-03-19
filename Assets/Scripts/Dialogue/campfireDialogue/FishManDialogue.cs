using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManDialogue : MonoBehaviour {
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

        string Feedme = "feed fishman" + gameObject.GetHashCode().ToString();
        Action takeMe = () => {
            Debug.Log("Blub blub, thanks!");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

            if (inventory.hasItemByName("Ration")) {
                survivor.Fed = true;
                fedOrNot = true;
                inventory.removeItemByName("Ration");
                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();
                npcDialogueHandler.dialogueContents.Clear();
                npcDialogueHandler.dialogueContents.Add($"Blub blub! You have {inventory.getCountofItem("Ration")} rations left.");
                
            } else {
                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();
                npcDialogueHandler.dialogueContents.Clear();
                npcDialogueHandler.dialogueContents.Add("Blub... You don't even have any for yourself.");
            }
            
            npcDialogueHandler.lastLineDisplayed = false;
            npcDialogueHandler.currentLineIndex += 1;
            npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

        string orNotTag = "do not feed fishman" + gameObject.GetHashCode().ToString();
        Action orNot = () => {
            Debug.Log("Blub... guess I'll stay hungry.");
            statsManager.interactedWithCampfireNPC();
            statsManager.updateBedStatus();
            fedOrNot = false;
            npcDialogueHandler.dialogueContents.Clear();
            npcDialogueHandler.lastLineDisplayed = false;
            npcDialogueHandler.currentLineIndex += 1;
            npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueContents = new List<string> {
            "Blub blub... Got any rations, traveler?",
            $"<link=\"{Feedme}\"><b><color=#d4af37>Feed</color></b></link>.\n...\n<link=\"{orNotTag}\"><b><color=#a40000>Or not...</color></b></link>."
        };
        //npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        npcDialogueHandler.dialogueContents.Clear();
        
        if (fedOrNot) {
            npcDialogueHandler.dialogueContents.Add("Blub! That was tasty. You ever need a lock picked, I'm your fish.");
        } else {
            npcDialogueHandler.dialogueContents.Add("Blub... No food for me...");
            npcDialogueHandler.dialogueContents.Add("There's no next time...");
        }
    }
}
