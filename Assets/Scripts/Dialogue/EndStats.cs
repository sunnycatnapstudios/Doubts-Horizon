using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndStats : MonoBehaviour {
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
            "stats"
        };

        npcDialogueHandler.beforeDialogue = new Action(() => {
            GameStatsManager.Instance._dialogueHandler.dialogueName.text = "Me";
            npcDialogueHandler.dialogueContents = new List<string> {
                $"Good job speedrunner you took {Time.realtimeSinceStartup} seconds.",
            };
        });

        npcDialogueHandler.afterDialogue = new Action(() => {
            AudioManager.Instance.RestartToDefault();
            SceneManager.LoadScene("Title", LoadSceneMode.Single);
        });
    }
}
