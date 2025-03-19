using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireSceneEnter : MonoBehaviour
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
            "It's getting pretty late, we should probably find somewhere to stay"
     
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void AfterDialogue() {
        afterDialogue.pickRandomDialogue();
        
        GameStatsManager.Instance._dialogueHandler.isCloseable = false;
        GameStatsManager.Instance._dialogueHandler.OpenDialogueWith(afterDialogue.gameObject);
    }
}
