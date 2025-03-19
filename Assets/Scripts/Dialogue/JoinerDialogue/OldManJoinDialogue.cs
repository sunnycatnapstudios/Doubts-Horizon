using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManJoinDialogue : MonoBehaviour {
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

        string takeMeTag = "Take me old man";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
            partyManager.AddToParty(survivor);
            Destroy(gameObject);
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);

        npcDialogueHandler.dialogueContents = new List<string> {
            "Hah, thought I'd seen my last sunrise..",
            "This world ain't what it used to be, kid..",
            $"<link=\"{takeMeTag}\"><b><#d4af37>Come along</color></b></link>, maybe we can still make something of it."
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void Update() {
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
    }
}
