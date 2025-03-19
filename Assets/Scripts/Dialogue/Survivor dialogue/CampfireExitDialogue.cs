using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireExitDialogue : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    private Player player;
    public Vector3 location;
    
    private PartyManager manager;
    [SerializeField] private Sprite highlightSprite;
    [SerializeField] private Sprite normalSprite;

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

        string takeMeTag = "endingDialogue";
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            Transform transform = GameObject.FindGameObjectWithTag("Player").transform;
            Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

            //if (inventory.hasItemByName("Ration")) {
            //    inventory.removeItemByName("Ration");
            //    npcDialogueHandler.dialogueContents.Add("You sleep full and satiated");
            //    npcDialogueHandler.lastLineDisplayed = false;
            //    npcDialogueHandler.currentLineIndex += 1;

            //} else {
            //    npcDialogueHandler.dialogueContents.Add("you go to sleep on an empty stomach");
            //    npcDialogueHandler.lastLineDisplayed = false;
            //    npcDialogueHandler.currentLineIndex += 1;
            //}
            player.movePoint.transform.position = location;
            player.transform.position = location;
            kickUnfed();
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();

            



        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);
       



        npcDialogueHandler.dialogueContents = new List<string> {
            "All people not fed will not survive to the next morning.",
            "If you do not leave a ration for yourself it will negatively affect your health.",
            "Are you sure you're ready to leave?",
            $"<link=\"{takeMeTag}\"><b><#d4af37>Click</color></b></link> if youre ready to go back or press E to Stay"
        };

        npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void Update() {
    }

    

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
    }
    private void kickUnfed() {
        GameObject[] followers = GameObject.FindGameObjectsWithTag("Followers");
        //followers = GameObject.FindGameObjectWithTag("Feed1").GetComponent<fireplace>().getFollowers();

        foreach (GameObject follower in followers) {
            follower.GetComponent<SpriteRenderer>().enabled = true;

        }
        List<Survivor> iterator = new List<Survivor>(manager.currentPartyMembers);
        foreach (Survivor survivor in iterator) {
            if (survivor.UnKickable) {
                continue;
            }
            if (survivor.Fed) {

                survivor.Fed = false;

            } else {
                manager.RemoveFromParty(survivor);
                Debug.Log($"Kicked {survivor.GetName()} from party");

            }
        }
    }


    public void highlightBedSprite() {
        GetComponentInParent<SpriteRenderer>().sprite = highlightSprite;


    }


    public void deselectBedSprite() {
        GetComponentInParent<SpriteRenderer>().sprite = normalSprite;
    }
}
