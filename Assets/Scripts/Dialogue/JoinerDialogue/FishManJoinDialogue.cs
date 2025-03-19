using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManJoinDialogue : MonoBehaviour {
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

        string takeMeTag = "Take me fish bro";
        Action takeMe = () => {
            Debug.Log("Blub! I'm in, blub!");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
            partyManager.AddToParty(survivor);
            Destroy(gameObject);
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);

        npcDialogueHandler.dialogueContents = new List<string> {
            "Blub... Thought I was lost at sea, blub...",
            "That storm was rough, but I'm a survivor, blub!",
            "Need a locksmith? Blub! I'm your guy!",
            $"<link=\"{takeMeTag}\"><b><#d4af37>Blub! Let's go</color></b></link>, before the tide changes."
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void Update() {
    }

    void AfterDialogue() {
        Debug.Log("Blub... Completed dialogue.");
    }
}
