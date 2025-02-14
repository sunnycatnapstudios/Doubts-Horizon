using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinerDialogue : MonoBehaviour
{
    private NPCDialogueHandler NPCDialogueHandler;
    private InteractPrompt InteractPrompt;
    public List<string> dialogueLines;
    private List<string> introLines, funnyRetort;
    public Survivor survivor;

    void Start ()
    {
        NPCDialogueHandler = GetComponent<NPCDialogueHandler>();
        InteractPrompt = GetComponent<InteractPrompt>();
        
        introLines = new List<string>
        {
            "It's dangerous to go alone!",
            "Take me."
        };
        dialogueLines = introLines;
        NPCDialogueHandler.dialogueLines = dialogueLines;
        NPCDialogueHandler.afterDialogue = new NPCDialogueHandler.AfterDialogueCall(AfterDialogue);
    }
    void Update ()
    {
    }
    void AfterDialogue() {
        Debug.Log("got hook");
        PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
        partyManager.AddToParty(survivor);
        

        
        
        Destroy(gameObject);
    }
}

