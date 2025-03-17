using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamselDialogue : MonoBehaviour {
    private DialogueBoxHandler NPCDialogueHandler;
    public List<string> dialogueLines;
    private List<string> introLines, funnyRetort;

    public Survivor Survivor;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
    }

    [SerializeField] private AudioClips audioClips;


    void Start() {
        NPCDialogueHandler = GetComponent<DialogueBoxHandler>();
        NPCDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        introLines = new List<string> {
            "Thank you so much for saving me from that beast!",
            "My hero <3"
        };
        dialogueLines = introLines;
        NPCDialogueHandler.dialogueContents = dialogueLines;
        NPCDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void Update() {
    }

    void AfterDialogue() {
        Debug.Log("got hook");
        PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
        partyManager.AddToParty(Survivor);
        Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        Destroy(gameObject);
        GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
    }
}
