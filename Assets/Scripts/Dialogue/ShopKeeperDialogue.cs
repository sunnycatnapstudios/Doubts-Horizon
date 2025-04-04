using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeperDialogue : MonoBehaviour
{
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    public Survivor survivor;
    private bool fedOrNot;
    private Inventory inventory;
    private GameStatsManager statsManager;
    public Item Potion;
    public Item Knife;
    public Item Ration;
    string orNotTag;
    string Feedme;
    string sacrificeHP;
    int timesSacrificed=0;

    void Start() {
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        statsManager = GameStatsManager.Instance;

         Feedme = "buy Potion" + gameObject.GetHashCode().ToString();
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            PartyManager partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();

            if (inventory.hasItemByName("Ration")) {
               
                inventory.removeItemByName("Ration");
               
                inventory.addItem(Potion);
                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                npcDialogueHandler.afterDialogue = AfterDialogue;
                npcDialogueHandler.dialogueContents.Add("Thank you for your endorsement");
                npcDialogueHandler.dialogueContents.Add($"You have {inventory.getCountofItem("Ration")} rations left.");

            } else {
               

                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                npcDialogueHandler.afterDialogue = AfterDialogue;

                npcDialogueHandler.dialogueContents.Add("You have no rations. What are you trying to pull?");
            }

            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(Feedme, takeMe);

         orNotTag = "buy Knife" + gameObject.GetHashCode().ToString();
        Action orNot = () => {
            if (inventory.hasItemByName("Ration")) {

                inventory.removeItemByName("Ration");
                inventory.addItem(Knife);


                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                npcDialogueHandler.afterDialogue = AfterDialogue;
                npcDialogueHandler.dialogueContents.Add("Be careful. It's sharp");
                npcDialogueHandler.dialogueContents.Add($"You have {inventory.getCountofItem("Ration")} rations left.");

            } else {


                npcDialogueHandler.lastLineDisplayed = false;
                npcDialogueHandler.currentLineIndex += 1;
                npcDialogueHandler.afterDialogue = AfterDialogue;

                npcDialogueHandler.dialogueContents.Add("You have no rations. What are you trying to pull?");
            }

            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(orNotTag, orNot);


         sacrificeHP = "Sell Life" + gameObject.GetHashCode().ToString();
        Action sellLife = () => {
            Debug.Log("Or not callback.");
           

            npcDialogueHandler.lastLineDisplayed = false;
            npcDialogueHandler.currentLineIndex += 1;
            npcDialogueHandler.afterDialogue = AfterDialogue;
            npcDialogueHandler.dialogueContents.Add("Let me have a look");
            if (checkHP()) {
                if (timesSacrificed > 3) {
                    npcDialogueHandler.dialogueContents.Add("Greed is not an admirable quality");
                } else {
                    takeHealthThreshhold();
                    inventory.addItem(Ration);
                    npcDialogueHandler.dialogueContents.Add("Thank you for the vitality");
                }

                npcDialogueHandler.dialogueContents.Add($"You have {inventory.getCountofItem("Ration")} rations left.");
            } else {
                npcDialogueHandler.dialogueContents.Add("You cannot give what you don't have.");

            }
            



            GameStatsManager.Instance._dialogueHandler.UpdateDialogueBox();
        };
        dialogueInputHandler.AddDialogueChoice(sacrificeHP, sellLife);

        npcDialogueHandler.dialogueContents = new List<string> {
            "Welcome to my shop,",
            "Does anything catch your eye?",
             $"<link=\"{Feedme}\"><b><color=#d4af37>Bandages</color></b></link>           <link=\"{orNotTag}\"><b><color=#a40000>knife</color></b></link>         <link=\"{sacrificeHP}\"><b><color=#a40000>Sacrifice...</color></b></link>"
        };
        npcDialogueHandler.beforeDialogue = BeforeDialogue;
        

        //npcDialogueHandler.afterDialogue = AfterDialogue;
    }
    void BeforeDialogue() {
        Debug.Log("am i even bing called");
        npcDialogueHandler.dialogueContents = new List<string> {
            "Welcome to my shop,",
            "Does anything catch your eye?",
             $"<link=\"{Feedme}\"><b><color=#d4af37>Bandages</color></b></link>     <link=\"{orNotTag}\"><b><color=#a40000>knife</color></b></link>     (1 Ration)\n        <link=\"{sacrificeHP}\"><b><color=#a40000>Sacrifice...</color></b></link>(Your Lifeforce)"
        };
    }
    public bool checkHP() {
        float threshhold = 0.3f;
        foreach (Survivor member in GameStatsManager.Instance.partyManager.currentPartyMembers) {
            if (member.currentHealth < member.maxHealth * threshhold) {
                return false;
            }
        }
        return true;
    }
    public void takeHealthThreshhold() {
        float threshhold = 0.3f;
        Debug.Log("HIAIIII");
        foreach (Survivor member in GameStatsManager.Instance.partyManager.currentPartyMembers) {
           
                member.currentHealth -=(int)( member.maxHealth * threshhold);
                Debug.Log(member.currentHealth.ToString()+member.name);
            if (member.currentHealth < 1) {
                member.currentHealth = 1;
            }
               
            

        }
    }

    void AfterDialogue() {
        npcDialogueHandler.dialogueContents = new List<string> {
            "Welcome to my shop,",
            "Does anything catch your eye?",
             $"<link=\"{Feedme}\"><b><color=#d4af37>Bandages</color></b></link>     <link=\"{orNotTag}\"><b><color=#a40000>knife</color></b></link>     (1 Ration)\n        <link=\"{sacrificeHP}\"><b><color=#a40000>Sacrifice...</color></b></link>(Your Lifeforce)"
        };

    }
}
