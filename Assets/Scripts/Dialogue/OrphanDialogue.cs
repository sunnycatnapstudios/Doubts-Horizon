using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrphanDialogue : MonoBehaviour {
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

        string takeMeTag = "Take orphan";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
            partyManager.AddToParty(survivor);
            Destroy(gameObject);
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);

        npcDialogueHandler.dialogueContents = new List<string> {
            "Mommy?!",
            "... *sobs*",
            "I can't find my mommy.",
            $"Can you <link=\"{takeMeTag}\"><b><#d4af37>help me</color></b></link>"
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void Update() {
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
    }
}
