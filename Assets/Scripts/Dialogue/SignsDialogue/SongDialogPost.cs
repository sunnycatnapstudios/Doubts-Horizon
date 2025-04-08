using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongDialogPost : MonoBehaviour
{
public int signDialogID;    // To pick which one to play

    private DialogueBoxHandler NPCDialogueHandler;
    private List<string> _dialogueLines;
    //public GameObject nextDialogue;     // Do not assign

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
    }

    [SerializeField] private AudioClips audioClips;


    void Start() {
        NPCDialogueHandler = GetComponent<DialogueBoxHandler>();
        NPCDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        switch (signDialogID) {
            case 1:
                _dialogueLines = new List<string> {
                    "At least that's what I think it says"
                };
                break;
            case 2:
                _dialogueLines = new List<string> {
                    "The words trail off there..."
                };
                break;
            case 3:
                _dialogueLines = new List<string> {
                    "*Something* must of torn off the end of this sign. " +
                    "I have a hunch to who the culprit may be...\n>looks behind you ..."
                };
                break;
            case 4:
                _dialogueLines = new List<string> {
                    "Guess we just have to follow the trail"
                };
                break;
            case 5:
                _dialogueLines = new List<string> {
                    "What a weird message...",
                    "Actually this sign seems rather new... at least I can read this one"
                };
                break;
            case 6:
                _dialogueLines = new List<string> {
                    "What a even weirder message...",
                    "Could there be more people living around here?"
                };
                break;
            case 7:
                _dialogueLines = new List<string> {
                    "I can see .. .. fire inside? What is going on in there?",
                };
                break;
            case 8:
                _dialogueLines = new List<string> {
                    "s... nary? Some of the letters are unreadable...",
                    "Wait are these scratch marks!?",
                    "...",
                    "Gotta be careful..."
                };
                break;
            case 9:
                _dialogueLines = new List<string> {
                    "..."
                };
                break;
            default:
                Debug.LogError("Unknown sign dialog ID");
                break;
        }


        NPCDialogueHandler.dialogueContents = _dialogueLines;
        NPCDialogueHandler.afterDialogue = AfterDialogue;
    }

    void AfterDialogue() {
        GameStatsManager.Instance._dialogueHandler.isCloseable = true;
        NPCDialogueHandler.afterDialogue = AfterDialogue;
    }
}
