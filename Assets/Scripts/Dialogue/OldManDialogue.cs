using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManDialogue : MonoBehaviour {
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
            GameStatsManager.Instance._dialogueHandler.OpenDialogueWith(GameObject.Find("Explosion"));
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);
        npcDialogueHandler.dialogueContents = new List<string> {
            $"EYY I am an old man"
        };

        npcDialogueHandler.afterDialogue = new Action(() => {
            Debug.Log("Completed Old Man dialogue.");
        });
    }
}
