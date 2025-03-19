using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionIntro : MonoBehaviour {
    private readonly string tagTarget = "Player";
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    private Player player;
    public GameObject nextDialogue;

    private PartyManager manager;
   
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
            "BOOOOOOOOOOOOOooOOOOOOOOOOOOOOoooOOOOOOOOOOOOoooOOOOOOOOOOOoooOOOo...{pause}OoooooooooooOOoooooooooOoooooooooooOoooooooooooooOoooooOoooOoooOoOOOOOOOOOOOOOOOM!"
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void AfterDialogue() {
        GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        GameObject transition = GameObject.Find("Intro^City1");
        player.movePoint.transform.position = player.transform.position = transition.transform.position + new Vector3(-13, 9);
        //player.movePoint.transform.position = player.transform.position = new Vector3(-70, -100, 18);

        if (nextDialogue) {
            GameStatsManager.Instance._dialogueHandler.isCloseable = false;
            GameStatsManager.Instance._dialogueHandler.OpenDialogueWith(nextDialogue);
        } else {
            GameStatsManager.Instance._dialogueHandler.isCloseable = true;
        }
    }
}
