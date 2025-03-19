using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteranDialogue : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
    }

    [SerializeField] private AudioClips audioClips;

    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        string takeMeTag = "Take me vet";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            Player player = playerObj.GetComponent<Player>();
            PartyManager partyManager = player.GetComponent<PartyManager>();
            partyManager.AddToParty(survivor);
            Destroy(gameObject);
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);
        string orNotTag = "Or not vet";
        Action orNot = () => {
            Debug.Log("Or not callback.");
            Destroy(gameObject);
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);
        npcDialogueHandler.dialogueContents = new List<string> {
            "What are you kids doing out here all alone? It's dangerous!",
            "Careful! There's on of 'em monsters ahead.",
            "Look at them soulless eyes...",
            "You can use your sling shot (RMB) to stun it, but it won't stay down for long.",
            "And sometimes, if you beat them they drop loot!",
            "just remember to check by pressing (I)",
            $"I could help you along your way.. all I ask is you share some of your supplies.",
            $"What do you say? Are you <link=\"{takeMeTag}\"><b><#d4af37>with me</color></b></link>? Or are you against that thing all <link=\"{orNotTag}\"><b><#a40000>by yourself</color></b></link>?"
        };

        npcDialogueHandler.afterDialogue = new Action(() => {
            Debug.Log("Completed Veteran dialogue.");
        });
    }
}
