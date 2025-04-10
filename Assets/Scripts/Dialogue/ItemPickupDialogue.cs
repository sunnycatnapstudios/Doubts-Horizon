using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class ItemPickupDialogue : MonoBehaviour
{

    public List<string> startDialogue;
    public List<string> afterDialogue;
    private Inventory inventory;
    public Item heldItem;
    DialogueBoxHandler dialogueBoxHandler;
    private bool hasItem = true;
    private DialogueBoxHandler npcDialogueHandler;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        npcDialogueHandler.dialogueContents = startDialogue;
        npcDialogueHandler.dialogueContents.Add($"You found a {heldItem.GetName()}");

        npcDialogueHandler.afterDialogue = AfterDialogue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AfterDialogue() {
        

        if (hasItem&& heldItem!= null) {
            inventory.addItem(heldItem);
            
        }
        npcDialogueHandler.dialogueContents = afterDialogue;
       
    }
}
