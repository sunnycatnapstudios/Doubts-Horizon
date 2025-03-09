using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HachiwareScript : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;
    private bool fedOrNot;
    private InteractPrompt prompt;
    private Inventory inventory;


    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        prompt = GetComponent<InteractPrompt>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        string Feedme = "feed hachi";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

            if (inventory.hasItemByName("Ration")) {
                survivor.Fed = true;
                fedOrNot = true;
                inventory.removeItemByName("Ration");
                npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
                prompt.forceFinishDialogue();
                npcDialogueHandler.dialogueContents.Add($"You have {inventory.getCountofItem("Ration")} rations left");
                prompt.forceDialogueEnd();
            } else {
                npcDialogueHandler.dialogueContents.Add($"You dont even have any for yourself");
                npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
                prompt.forceDialogueEnd();
            }

            //prompt.forceDialogueEnd();
        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

        string orNotTag = "do not feed hachi";
        Action orNot = () => {
            Debug.Log("Or not callback.");
            fedOrNot = false;
            npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            prompt.forceDialogueEnd();
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueContents = new List<string> {
            "Im very hungry please feed me",
            $"<link=\"{Feedme}\"><b><#d4af37>Feed</color></b></link>.\n...\n<link=\"{orNotTag}\"><b><#a40000>Or not...</color></b></link>."
        };
        //npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        if (fedOrNot) {
            npcDialogueHandler.dialogueContents = new List<string>
                { "thank you for saving me mister", "im forever in your debt!" };
        } else {
            npcDialogueHandler.dialogueContents = new List<string> { "oh ok....", "i guess i see how it is...." };
        }
    }
}
