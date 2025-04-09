using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oldmanEndingDialogue : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;
    private GameStatsManager statsManager;
    public AudioClip sfxTalkingClip;

    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        statsManager = GameStatsManager.Instance;

        if (sfxTalkingClip == null) {
            sfxTalkingClip = survivor.GetTalkingSfx();
        }
        npcDialogueHandler.SetSfxTalkingClip(sfxTalkingClip);

        statsManager.interactedWithCampfireNPC();
        statsManager.updateBedStatus();

        npcDialogueHandler.dialogueContents = new List<string> {
            "This was the most scared and the most I have been in my old life.",
            "Good job in leading us to safety."
        };

        npcDialogueHandler.afterDialogue = AfterDialogue;

        GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
    }

    void AfterDialogue() {
        Debug.Log("Completed good ending orphan dialogue.");
    }
}

