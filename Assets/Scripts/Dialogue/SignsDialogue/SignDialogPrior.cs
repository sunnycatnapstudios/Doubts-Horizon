using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignDialogPrior : MonoBehaviour
{
    public int signDialogID;    // To pick which one to play

    private DialogueBoxHandler NPCDialogueHandler;
    private List<string> _dialogueLines;
    public GameObject nextDialog;     // Assign with me sprite

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
                    ">N_w lea_in_ __ub_ __r_zo_. Po_u_at_on _0__1",
                };
                break;
            case 2:
                _dialogueLines = new List<string> {
                    ">DO __T CRO_S, __NT__S AH_A_aaa.....",
                };
                break;
            case 3:
                _dialogueLines = new List<string> {
                    ">2km - lea_ing d___b_ _or__on\n>1.8k_ - hi_hw__ ___",
                };
                break;
            case 4:
                _dialogueLines = new List<string> {
                    ">1.4km - h__e_p vil_ig_",
                };
                break;
            case 5:
                _dialogueLines = new List<string> {
                    ">BENATOR BRIDGE CROSSING AHEAD\n(but why not come up instead)",
                };
                break;
            case 6:
                _dialogueLines = new List<string> {
                    ">BENATOR BRIDGE CROSSING AHEAD\n(and come see me while you're at it~)",
                };
                break;
            case 7:
                _dialogueLines = new List<string> {
                    ">WELCOME TO COMMODITYING COMMODITIES FOR BUYING AND BUYING.\nMAKE SURE TO COME IN HEALTHY AND COME OUT WEALTHY",
                };
                break;
            case 8:
                _dialogueLines = new List<string> {
                    ">s_/nt/_ty",
                };
                break;
            case 9:
                _dialogueLines = new List<string> {
                    ">welcome to our santuary. population 36",
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
        Debug.Log(nextDialog +"Exist or not?");
        if (nextDialog) {
            GameStatsManager.Instance._dialogueHandler.isCloseable = false;
            GameStatsManager.Instance._dialogueHandler.OpenDialogueWith(nextDialog);
        } else {
            GameStatsManager.Instance._dialogueHandler.isCloseable = true;

        }
    }
}
