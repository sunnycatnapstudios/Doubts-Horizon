using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HachiwareScript : MonoBehaviour
{
    private DialogueInputHandler dialogueInputHandler;
    private NPCDialogueHandler npcDialogueHandler;
    public Survivor survivor;
    private bool fedOrNot;
    private InteractPrompt prompt;

    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<NPCDialogueHandler>();
        prompt = GetComponent<InteractPrompt>();

        string Feedme = "feed hachi";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
            survivor.Fed = true;
            fedOrNot = true;
            prompt.forceDialogueEnd();




        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

        string orNotTag = "do not feed hachi";
        Action orNot = () => {
            Debug.Log("Or not callback.");
            fedOrNot = false;
            prompt.forceDialogueEnd();



        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueLines = new List<string> {
            "It's dangerous to go alone!",
            $"<link=\"{Feedme}\"><b><#d4af37>Take me</color></b></link>.\n...\n<link=\"{orNotTag}\"><b><#a40000>Or not...</color></b></link>."
        };
        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);


    }
    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        if (fedOrNot) {
            npcDialogueHandler.dialogueLines = new List<string>{"oh ok....", "i guess i see how it is...."};

        }
        npcDialogueHandler.dialogueLines = new List<string> { "oh ok....", "i guess i see how it is...." };
    }
}
