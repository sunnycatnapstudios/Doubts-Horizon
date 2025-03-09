using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newGuyScript : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;
    private bool fedOrNot;
    private Inventory inventory;


    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        string Feedme = "feed newguy";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

            if (inventory.hasItemByName("Ration")) {
                survivor.Fed = true;
                fedOrNot = true;
                inventory.removeItemByName("Ration");
                npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
                npcDialogueHandler.dialogueContents.Add($"You have {inventory.getCountofItem("Ration")} rations left");
            } else {
                npcDialogueHandler.dialogueContents.Add($"You dont even have any for yourself");
                npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            }
            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

        string orNotTag = "do not feed newguy";
        Action orNot = () => {
            Debug.Log("Or not callback.");
            fedOrNot = false;
            npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueContents = new List<string> {
            "please, if you have any to share",
            $"<link=\"{Feedme}\"><b><#d4af37>Feed</color></b></link>.\n...\n<link=\"{orNotTag}\"><b><#a40000>Or not...</color></b></link>."
        };
        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        if (fedOrNot) {
            npcDialogueHandler.dialogueContents = new List<string> { "i love beef jerky" };
        } else {
            npcDialogueHandler.dialogueContents = new List<string> { "i guess this is it hubh", "well" };
        }
    }
}
