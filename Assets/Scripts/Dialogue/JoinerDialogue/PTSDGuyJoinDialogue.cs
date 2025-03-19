using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTSDGuyJoinDialogue : MonoBehaviour {
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

        string takeMeTag = "Take me PTSD guy";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
            partyManager.AddToParty(survivor);
            Destroy(gameObject);
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);

        npcDialogueHandler.dialogueContents = new List<string> {
            "You don't know what it's like... the things I've seen...",
            "I can't sleep without hearing the screams...",
            $"<link=\"{takeMeTag}\"><b><color=#d4af37>Come with me</color></b></link>. Maybe... maybe I can find some peace."
        };

        npcDialogueHandler.afterDialogue = AfterDialogue;
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
    }
}
    