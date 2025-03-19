using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrphanDialogue : MonoBehaviour {
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

        string takeMeTag = "Take orphan";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            Player player = playerObj.GetComponent<Player>();
            PartyManager partyManager = player.GetComponent<PartyManager>();
            partyManager.AddToParty(survivor);
            Destroy(gameObject);
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
            GameObject transition = GameObject.Find("Intro^City1");
            player.movePoint.transform.position = player.transform.position = transition.transform.position + new Vector3(-13, 10);
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);

        npcDialogueHandler.dialogueContents = new List<string> {
            $"WAAH! <link=\"{takeMeTag}\"><b><#d4af37>Get it off!</color></b></link>"
//             "OWWW it hurts!!",
        };

        npcDialogueHandler.afterDialogue = new Action(() => {
            Debug.Log("Completed Orphan Dialogue.");
        });
    }

}
