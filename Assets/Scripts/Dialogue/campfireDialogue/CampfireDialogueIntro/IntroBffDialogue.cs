using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBffDialogue : MonoBehaviour
{
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;
    private bool fedOrNot;
    private Inventory inventory;
    private GameStatsManager statsManager;
    public GameObject loreDialogueCollider; // INSPECTOR
    string Feedme = "IntroFeedHachi";

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
    }

    [SerializeField] private AudioClips audioClips;

    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        statsManager = GameStatsManager.Instance;
        if (audioClips.sfxTalkingBlip == null && survivor != null) {
            audioClips.sfxTalkingBlip = survivor.GetTalkingSfx();
        }
        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

            if (inventory.hasItemByName("Ration")) {
                survivor.Fed = true;
                fedOrNot = true;
                inventory.removeItemByName("Ration");
                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();
                npcDialogueHandler.dialogueContents.Add($"You have {inventory.getCountofItem("Ration")} rations left");
                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                npcDialogueHandler.afterDialogue = new Action(AfterDialogue);

            } else {

                statsManager.interactedWithCampfireNPC();
                statsManager.updateBedStatus();

                npcDialogueHandler.dialogueContents.Add($"Its just over there");
                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                //npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            }
            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

        npcDialogueHandler.dialogueContents = new List<string> {
            "hey, If you dont mind splitting that ration,","Im really weak right now",
            $"<link=\"{Feedme}\"><b><#d4af37>Feed</color></b></link>"
        };

        npcDialogueHandler.beforeDialogue = BeforeDialogue;
    }

    void BeforeDialogue() {
        if (inventory.hasItemByName("Ration")) {
            npcDialogueHandler.dialogueContents = new List<string> {
                "hey, If you dont mind splitting that ration,","Im really weak right now",
                $"<link=\"{Feedme}\"><b><#d4af37>Feed</color></b></link>"
            };
        } else if (!fedOrNot) {
            npcDialogueHandler.dialogueContents = new List<string> {
                "Big juicy chicken leg right over there!",
            };
        }

        npcDialogueHandler.beforeDialogue = BeforeDialogue;
    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        npcDialogueHandler.dialogueContents = new List<string> {
            "Thank you so much! I don't think I would have made it!", "Times are so tough..",
            "What is there to do next?",
        };
        loreDialogueCollider.SetActive(true);
    }
}
