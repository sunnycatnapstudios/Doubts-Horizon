using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCampfireExitDialogue : MonoBehaviour
{
    private readonly string tagTarget = "Player";
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    private Player player;

    private PartyManager manager;
    public RandomSurvivorCampfireScript afterDialogue;


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

        npcDialogueHandler.beforeDialogue = new Action(BeforeDialogue);
        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        npcDialogueHandler.dialogueContents = new List<string> {
            "It's getting late, we can camp out here for the night",
            "I can't believed the building collapsed",
            "those poor other people, we were the only ones that made it out of the group :(",

        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void BeforeDialogue() {
        GameStatsManager.Instance._dialogueHandler.dialogueProfile.sprite = manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].Sprite;

        Debug.Log(manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].ToString());
        string name = manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].Name;
        npcDialogueHandler.dialogueContents.Add($"I cant believe we saw {name} di...*sobs*");

    }

    void AfterDialogue() {
        gameObject.SetActive(false);
    }
}
