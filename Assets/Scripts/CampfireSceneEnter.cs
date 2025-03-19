using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireSceneEnter : MonoBehaviour
{
    private readonly string tagTarget = "Player";
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    private Player player;

    private PartyManager manager;
    public RandomSurvivorCampfireScript afterDialogue;
    
   
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
            "It's getting pretty late, we should probably find somewhere to stay"
     
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void AfterDialogue() {
        //

        afterDialogue.pickRandomDialogue();
        //npcDialogueHandler.currentLineIndex = 0;

        
        GameStatsManager.Instance._dialogueHandler.isCloseable = false;
        GameStatsManager.Instance._dialogueHandler.OpenDialogueWith(afterDialogue.gameObject);
        GameStatsManager.Instance._dialogueHandler.isCloseable = true;
    }

    private IEnumerator waitToCallNextDialogue() {
        yield return new WaitForSeconds(1);
        
    }
     void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag(tagTarget)) {
            if (player == null) {
                GameObject playerObject = other.gameObject;
                player = other.gameObject.GetComponent<Player>();



            }
            Debug.Log("hitColliderCampfire");
            gameObject.GetComponent<DialogueBoxHandler>().npcProfile = manager.player.Sprite;
            GameStatsManager.Instance._dialogueHandler.isCloseable = false;
            GameStatsManager.Instance._dialogueHandler.OpenDialogueWith(gameObject);
            gameObject.GetComponent<Collider2D>().enabled = false;

        }



    }
}
