using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newGuyScript : MonoBehaviour
{
    private DialogueInputHandler dialogueInputHandler;
    private NPCDialogueHandler npcDialogueHandler;
    public Survivor survivor;
    private bool fedOrNot;
    private InteractPrompt prompt;
    private Inventory inventory;


    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<NPCDialogueHandler>();
        prompt = GetComponent<InteractPrompt>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        string Feedme = "feed newguy";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

            if (inventory.hasItemByName("Ration")) {
                survivor.Fed = true;
                fedOrNot = true;
                inventory.removeItemByName("Ration");
                npcDialogueHandler.dialogueLines.Add($"You have {inventory.getCountofItem("Ration")} rations left");
            } else {
                npcDialogueHandler.dialogueLines.Add($"You dont even have any for yourself");
            }

            prompt.forceDialogueEnd();





        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

        string orNotTag = "do not feed newguy";
        Action orNot = () => {
            Debug.Log("Or not callback.");
            fedOrNot = false;
            prompt.forceDialogueEnd();



        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueLines = new List<string> {
            "please, if you have any to share",
            $"<link=\"{Feedme}\"><b><#d4af37>Feed</color></b></link>.\n...\n<link=\"{orNotTag}\"><b><#a40000>Or not...</color></b></link>."
        };
        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);


    }
    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        if (fedOrNot) {
            npcDialogueHandler.dialogueLines = new List<string> { "i love beef jerky" };

        } else {
            npcDialogueHandler.dialogueLines = new List<string> { "i guess this is it hubh", "well" };
        }
    }
}
