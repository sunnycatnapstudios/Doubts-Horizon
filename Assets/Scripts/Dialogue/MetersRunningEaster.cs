using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetersRunningEaster : MonoBehaviour {
    private readonly string tagTarget = "Player";
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    private Player player;
    public GameObject nextDialogue;

    private PartyManager manager;
   
    [Serializable]
    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
    }

    [SerializeField] private AudioClips audioClips;

    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        manager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        npcDialogueHandler.dialogueContents = new List<string> {
            "Looks like this used to be a really fast car!",
            "It definitely has seen better days though...",
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void AfterDialogue() {
    }
}
