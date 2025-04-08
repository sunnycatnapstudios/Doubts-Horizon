using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManJoinDialogue : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;
    string takeMeTag = "Take me fish bro";

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
    }

    [SerializeField] private AudioClips audioClips;

    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        Action takeMe = () => {
            Debug.Log("Blub! I'm in, blub!");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
            partyManager.AddToParty(survivor);
            Destroy(gameObject);
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);

//         npcDialogueHandler.dialogueContents = new List<string> {
//             "Blub... Thought I was lost at sea, blub...",
//             "That storm was rough, but I'm a survivor, blub!",
//             "Need a locksmith? Blub! I'm your guy!",
//             $"<link=\"{takeMeTag}\"><b><#d4af37>Blub! Let's go</color></b></link>, before the tide changes."
//         };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
        npcDialogueHandler.beforeDialogue = BeforeDialogue;
    }

    void BeforeDialogue() {
        Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        if (inventory.hasItemByName("Fishbowl")) {
            npcDialogueHandler.dialogueContents = new List<string> {
                "Blub! That bowl",
                "It is exactly what I need blub!",
                "Let me use it and I will forever be in your debt blub.",
                $"Whaddya say, blub?\n<link=\"{takeMeTag}\"><b><#d4af37>Give</color></b></link>",
            };
        } else {
            npcDialogueHandler.dialogueContents = new List<string> {
                "Blub... I used to be a locksmith y'know, blub...",
                "Now I can't even blubbing leave the riverside or else I will drown-",
                "you know what I mean blub.",
                "What can you do though.. I'm a survivor, blub!",
            };
        }
        Debug.Log("fish beforedialogue1");
    }

    void AfterDialogue() {
        Debug.Log("Blub... Completed dialogue.");
    }
}
