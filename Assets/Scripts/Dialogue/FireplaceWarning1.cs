using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireplaceWarning1  : MonoBehaviour {
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
        "I can see a fireplace up ahead.",
        "If I'm done here, I should move forward.",
        "Could always use some rest..."
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
