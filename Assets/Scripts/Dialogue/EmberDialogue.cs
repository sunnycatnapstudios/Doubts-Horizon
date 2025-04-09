using Random = UnityEngine.Random;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmberDialogue : MonoBehaviour {
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
        if (audioClips.sfxTalkingBlip == null && survivor != null) {
            audioClips.sfxTalkingBlip = survivor.GetTalkingSfx();
        }
        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        npcDialogueHandler.beforeDialogue = BeforeDialogue;
    }

    void BeforeDialogue() {
        Debug.Log("Ember BeforeDialogue");
        var dialogueOptions = new List<string> {
            "Just because I'm a fire doesn't mean I can't live in the woods y'know.",
            "I'm actually a pretty chill dude B)",
            "Only I can prevent forest fires!",
        };
        npcDialogueHandler.dialogueContents = new List<string> {
            dialogueOptions[Random.Range(0, dialogueOptions.Count)],
        };
        npcDialogueHandler.beforeDialogue = BeforeDialogue;
    }
}
