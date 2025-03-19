using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBffDialogue : MonoBehaviour
{
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;
    private bool fedOrNot;
    private Inventory inventory;
    private GameStatsManager statsManager;
    public GameObject loreDialogueCollider; // INSPECTOR


    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        statsManager = GameStatsManager.Instance;

        string Feedme = "IntroFeedHachi";
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

                npcDialogueHandler.dialogueContents.Add($"Its just over there");
                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                //npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            }
            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

        string orNotTag = "introAbandonHachi";
        Action orNot = () => {
            Debug.Log("Or not callback.");
            fedOrNot = false;
            npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            statsManager.interactedWithCampfireNPC();
            statsManager.updateBedStatus();
            npcDialogueHandler.dialogueContents.Add("do you want to get rid of me that bad?");
            npcDialogueHandler.dialogueContents.Add("i guess i see how it is....");
            npcDialogueHandler.lastLineDisplayed = false;
            npcDialogueHandler.currentLineIndex += 1;
            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueContents = new List<string> {
            "hey, If you dont mind splitting that ration,","Im really weak right now",
            $"<link=\"{Feedme}\"><b><#d4af37>Feed</color></b></link>.\n...\n<link=\"{orNotTag}\"><b><#a40000>Or not...</color></b></link>."
        };
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        if (fedOrNot) {
            npcDialogueHandler.dialogueContents = new List<string>
                { "Thanks!", "I knew I could trust you!" };
        } else {
            npcDialogueHandler.dialogueContents = new List<string> { "well", "im sure i can manage" };
        }
        loreDialogueCollider.SetActive(true);
    }
}
