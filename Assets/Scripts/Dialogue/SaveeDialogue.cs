using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveeDialogue : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;
    [Serializable]
    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
    }

    [SerializeField] private AudioClips audioClips;
    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        string takeMeTag = "Take me savee";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
            partyManager.AddToParty(survivor);
            Destroy(gameObject);
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);

        string orNotTag = "Or not savee";
        Action orNot = () => {
            Debug.Log("Or not callback.");
            Destroy(gameObject);
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueContents = new List<string> {
            "Thank you so much for saving me from that thing!",
            $"May I <link=\"{takeMeTag}\"><b><#d4af37>join you</color></b></link>?\n...\n<link=\"{orNotTag}\"><b><#a40000>Otherwise</color></b></link> I might not make it out here all alone."
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }
    void Update() {
    }
    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
    }
}

