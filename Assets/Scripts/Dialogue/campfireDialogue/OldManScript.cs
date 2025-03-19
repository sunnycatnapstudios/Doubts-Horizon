using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManScript : MonoBehaviour {
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

        string feedMe = "feed oldguy" + gameObject.GetHashCode().ToString();
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

            if (inventory.hasItemByName("Ration")) {
                survivor.Fed = true;
                fedOrNot = true;
                inventory.removeItemByName("Ration");
                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();
                
                npcDialogueHandler.dialogueContents.Add($"You have {inventory.getCountofItem("Ration")} rations left.");
            } else {
                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();
                npcDialogueHandler.dialogueContents.Add("Too bad we are out...");
            }

            npcDialogueHandler.lastLineDisplayed = false;
            npcDialogueHandler.currentLineIndex += 1;
            npcDialogueHandler.afterDialogue = AfterDialogue;
            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(feedMe, takeMe);

        string orNotTag = "do not feed oldguy" + gameObject.GetHashCode().ToString();
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
            "Please, I haven't eaten in days...",
             $"<link=\"{feedMe}\"><b><color=#d4af37>Feed</color></b></link>.\n...\n<link=\"{orNotTag}\"><b><color=#a40000>Or not...</color></b></link>."
        };
        //npcDialogueHandler.afterDialogue = AfterDialogue;
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        npcDialogueHandler.dialogueContents = fedOrNot
            ? new List<string> { "Ah, thank you. This reminds me of the war days..." }
            : new List<string> { "I see... so this is how it ends...", "Farewell, then." };
    }
}
