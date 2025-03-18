using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSurvivorCampfireScript : MonoBehaviour
{
    // Start is called before the first frame update
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    private Player player;
    public Vector3 movement;
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
        npcDialogueHandler.dialogueContents = new List<string> {
           "..."

        };

        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        
    }

    void AfterDialogue() {
        player.movePoint.transform.position = player.movePoint.transform.position +movement;
        player.transform.position +=movement;
    }


    public void pickRandomDialogue() {

        int min = 0;
        if (manager.currentPartyMembers.Count > 1) {
            min = 1;
        }

        int rand = UnityEngine.Random.Range(min,manager.currentPartyMembers.Count);

        string dialogue = manager.currentPartyMembers[rand].enteringFireDialogue;
        Debug.Log(dialogue);
        gameObject.GetComponent<DialogueBoxHandler>().npcProfile = manager.currentPartyMembers[rand].Sprite;

        npcDialogueHandler.currentLineIndex = 0;

        npcDialogueHandler.dialogueContents = new List<string> {
           
           dialogue

        };
       
        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);








    }

    
}
