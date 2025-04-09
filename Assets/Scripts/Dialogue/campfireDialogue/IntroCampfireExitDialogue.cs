using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCampfireExitDialogue : MonoBehaviour {
    private readonly string tagTarget = "Player";
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    private Player player;

    private PartyManager manager;
    public RandomSurvivorCampfireScript afterDialogue;


    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        manager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

        npcDialogueHandler.beforeDialogue = new Action(BeforeDialogue);

        npcDialogueHandler.dialogueContents = new List<string> {
            "It's getting late, we can camp out here for the night",
            "I can't believed the building collapsed",
            "The elders, they used to speak of a sanctuary far north, where any could find salviation",
            "but those poor other people cant anymore, we were the only ones that made it out of the group :(",
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void BeforeDialogue() {
        GameStatsManager.Instance._dialogueHandler.dialogueProfile.sprite =
            manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].Sprite;
        GameStatsManager.Instance._dialogueHandler.dialogueName.text =
            manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].name;

        Debug.Log(manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].ToString());
        string name = manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].Name == "Best Friend"
            ? "little Orphan Olivia"
            : "Best Friend Fred";
        npcDialogueHandler.dialogueContents.Add($"I cant believe we saw {name} di...*sobs*");
        AudioClip talkingSfx = manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].GetTalkingSfx();
        npcDialogueHandler.SetSfxTalkingClip(talkingSfx);
    }

    void AfterDialogue() {
        gameObject.SetActive(false);
    }
}
