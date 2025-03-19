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

       
        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        npcDialogueHandler.dialogueContents = new List<string> {
            "Hey over there!",
            "That looks like some food!",
            "Im starving"

        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }



    void AfterDialogue() {
      gameObject.SetActive(false);
    }
}
