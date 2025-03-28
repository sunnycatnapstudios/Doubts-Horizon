using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireplaceWarning2  : MonoBehaviour {
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
        "We could use a second break now",
         "I see another fireplace up ahead",
        "Hopefully I have collected enough supplies...",
            };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void AfterDialogue() {
        if (nextDialogue) {
            GameStatsManager.Instance._dialogueHandler.isCloseable = false;
            GameStatsManager.Instance._dialogueHandler.OpenDialogueWith(nextDialogue);
        } else {
            GameStatsManager.Instance._dialogueHandler.isCloseable = true;
        }
    }
}
