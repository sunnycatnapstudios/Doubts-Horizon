using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocksmithUnlockDialogue : MonoBehaviour {
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

        npcDialogueHandler.dialogueContents = new List<string> {
            "", // Secret first line which is getting skipped for some reason
            "I've got this, blub!",
        };
    }
}
