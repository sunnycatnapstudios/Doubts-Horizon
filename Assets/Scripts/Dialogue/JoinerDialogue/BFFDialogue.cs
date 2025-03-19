using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFFDialogue : MonoBehaviour {
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

        string takeMeTag = "Take me bff";
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
    "Oh good, I thought you were gone",
    "That was some crazy storm..",
    $"<link=\"{takeMeTag}\"><b><#d4af37>Let's go</color></b></link> and look for the others."
};

        npcDialogueHandler.afterDialogue = new Action(() => {
            Debug.Log("Completed BFF dialogue.");
        });
    }
}
