using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishmanEndingDialogue : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;
    private GameStatsManager statsManager;
    public AudioClip sfxTalkingClip;

    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        statsManager = GameStatsManager.Instance;

        if (sfxTalkingClip == null && survivor != null) {
            sfxTalkingClip = survivor.GetTalkingSfx();
        }
        npcDialogueHandler.SetSfxTalkingClip(sfxTalkingClip);

        statsManager.interactedWithCampfireNPC();
        statsManager.updateBedStatus();

        npcDialogueHandler.dialogueContents = new List<string> {
            "BLUB.. I knew you were a leader once I met you!",
            "Cannot wait to develop - BLUB! - this place."
        };

        npcDialogueHandler.afterDialogue = AfterDialogue;

        GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
    }

    void AfterDialogue() {
        Debug.Log("Completed good ending orphan dialogue.");
    }
}

