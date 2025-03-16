using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinerDialogue : MonoBehaviour {
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

        if (dialogueInputHandler == null) return;

        string takeMeTag = "Take me";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
            partyManager.AddToParty(survivor);
            Destroy(gameObject);
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);

        string orNotTag = "Or not";
        Action orNot = () => {
            Debug.Log("Or not callback.");
            Destroy(gameObject);
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);

        npcDialogueHandler.dialogueContents = new List<string> {
            "It's dangerous to go alone!",
            $"<link=\"{takeMeTag}\"><b><#d4af37>Take me</color></b></link>.\n...\n<link=\"{orNotTag}\"><b><#a40000>Or not...</color></b></link>."
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }
    void Update() {
    }
    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
    }
}

