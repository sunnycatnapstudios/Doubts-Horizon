using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static AlertAnim;

public class endingDialogue : MonoBehaviour
{

    private readonly string tagTarget = "Player";
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    private Player player;
    public GameObject nextDialogue;

    // Transition scripts with external functions to call
    public LevelTransition goodTransition;
    public LevelTransition badTransition;

    private AudioTransition audioTransition;
    public fireplace Goodfp;
    public fireplace Badfp;

    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
    }
    [SerializeField] private AudioClips audioClips;
    // Start is called before the first frame update
    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
       
        //levelTransition = GetComponent<LevelTransition>();
        audioTransition = GetComponent<AudioTransition>();
       

        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        npcDialogueHandler.dialogueContents = new List<string> {
            "lets see where youre gonna go"
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void AfterDialogue() {
        GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        //GameObject transition = GameObject.Find("Intro^City1");
        //player.movePoint.transform.position = player.transform.position = transition.transform.position + new Vector3(-13, 9);
        //player.movePoint.transform.position = player.transform.position = new Vector3(-70, -100, 18);

        // Just use our built in transition scripts
        GameStatsManager.Instance.EndIntroSequence();
        
        audioTransition.TriggerAudioTransition();

        if (GameStatsManager.Instance.partyManager.currentPartyMembers.Count > 2) {
            StartCoroutine(Goodfp.StartFireplaceEvent());
            StartCoroutine(goodTransition.PerformLevelTransition());
        } else {
            StartCoroutine(Badfp.StartFireplaceEvent());
            StartCoroutine(badTransition.PerformLevelTransition());
        }
        if (nextDialogue) {
            GameStatsManager.Instance._dialogueHandler.isCloseable = false;
            GameStatsManager.Instance._dialogueHandler.OpenDialogueWith(nextDialogue);
        } else {
            GameStatsManager.Instance._dialogueHandler.isCloseable = true;
        }
    }
}
