using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinerDialogue : MonoBehaviour {
    private NPCDialogueHandler npcDialogueHandler;
    private InteractPrompt InteractPrompt;
    public Survivor survivor;

    void Start() {
        npcDialogueHandler = GetComponent<NPCDialogueHandler>();
        InteractPrompt = GetComponent<InteractPrompt>();

        npcDialogueHandler.dialogueLines = new List<string> {
            "It's dangerous to go alone!",
            "Take me."
        };
        npcDialogueHandler.afterDialogue = new NPCDialogueHandler.AfterDialogueCall(AfterDialogue);
    }
    void Update() {
    }
    void AfterDialogue() {
        Debug.Log("Completed dialogue");
        PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
        partyManager.AddToParty(survivor);
        Destroy(gameObject);
    }
}

