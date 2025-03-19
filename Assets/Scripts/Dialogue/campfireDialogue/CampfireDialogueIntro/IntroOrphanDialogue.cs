using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroOrphanDialogue : MonoBehaviour
{
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

        string Feedme = "IntroFeedOrphan";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

            if (inventory.hasItemByName("Ration")) {
                survivor.Fed = true;
                fedOrNot = true;
                inventory.removeItemByName("Ration");
                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();
                npcDialogueHandler.dialogueContents.Add($"You have {inventory.getCountofItem("Ration")} rations left");
                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                npcDialogueHandler.afterDialogue = new Action(AfterDialogue);

            } else {

                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();

                npcDialogueHandler.dialogueContents.Add($"You dont even have any for yourself");
                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            }
            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

        string orNotTag = "IntroAbandonOrphan";
        Action orNot = () => {
            Debug.Log("Or not callback.");
            fedOrNot = false;
            npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            statsManager.interactedWithCampfireNPC();
            statsManager.updateBedStatus();
            npcDialogueHandler.dialogueContents.Add("But why not?");
            npcDialogueHandler.dialogueContents.Add("Please...?");
            npcDialogueHandler.lastLineDisplayed = false;
            npcDialogueHandler.currentLineIndex += 1;
            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueContents = new List<string> {
            "I... I haven't eaten in so long... Please, do you have anything?",
            $"<link=\"{Feedme}\"><b><#d4af37>Feed</color></b></link>.\n...\n<link=\"{orNotTag}\"><b><#a40000>Or not...</color></b></link>."
        };
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        if (fedOrNot) {
            npcDialogueHandler.dialogueContents = new List<string>
                { "Thank you so much I dont think I would have made it!", "Times are so rough out here though,","What is there to do next?" };
        } else {
            npcDialogueHandler.dialogueContents = new List<string> { "Fine", "I didnt need any anyway","You should have left me in that fire" };
        }
    }
}
