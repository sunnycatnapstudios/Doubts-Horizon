using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCamfireDialogue : MonoBehaviour
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
            "Damn, I just checked my inventory (toggle using I) and I am out of food...",
            "Oh wait that looks like some food, over there!",
            "Go grab it, I'm starving",
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void BeforeDialogue() {
        GameStatsManager.Instance._dialogueHandler.dialogueProfile.sprite = manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].Sprite;
        GameStatsManager.Instance._dialogueHandler.dialogueName.text = manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].Name;

        Debug.Log(manager.currentPartyMembers[manager.currentPartyMembers.Count - 1].ToString());

    }

    void AfterDialogue() {
      gameObject.SetActive(false);
    }
}
