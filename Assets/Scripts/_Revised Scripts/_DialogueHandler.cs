using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class _DialogueHandler : MonoBehaviour
{
    private float detectionRadius = 1.2f;
    private GameObject player, currentNPC, dialogueBox, currentInteractPrompt, currentSmallDialogueBox, dialogueOptions;
    public GameObject interactPromptPrefab, smallDialogueBox;
    public LayerMask NPCLayer;

    private TextMeshProUGUI dialogueText, dialogueName, smallDialogueText;
    private Image dialogueProfile, continueArrow;
    public Animator dialogueAnimator, darkScreenAnimator;
    private bool isDialogueActive = false;

    private DialogueBoxHandler dialogueBoxHandler;
    private TypeWriter typeWriter;

    private List<string> positiveAnswer, negativeAnswer;
    private List<string> currentDialogue;


    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        dialogueBox = GameObject.FindWithTag("Dialogue Box");

        dialogueProfile = GameObject.FindWithTag("Character Profile").GetComponent<Image>();
        dialogueText = GameObject.FindWithTag("Dialogue Text").GetComponentInChildren<TextMeshProUGUI>();
        dialogueName = GameObject.FindWithTag("Name Card").GetComponentInChildren<TextMeshProUGUI>();
        dialogueAnimator = dialogueBox.GetComponent<Animator>();

        darkScreenAnimator = GameObject.FindWithTag("Dark Screen").GetComponent<Animator>();
        typeWriter = dialogueBox.GetComponentInChildren<TypeWriter>();

        dialogueOptions = GameObject.FindWithTag("Dialogue Options");
        // dialogueOptions.SetActive(false);

        positiveAnswer = new List<string> 
        { 
            "Oh that's nice, hope it stays cheerful!!!",
            "...",
            "See ya later then..."
        };

        negativeAnswer = new List<string>
        { 
            ".....Oh, sorry it's not going that great",
            "Hoping it gets better then...",
            "I'll leave you to it then......",
            ".....",
            "Sorry......"
        };
    }

    void Start()
    {
        continueArrow = GameObject.FindWithTag("Continue Arrow").GetComponent<Image>();
    }

    void Update()
    {
        Collider2D npcCollider = Physics2D.OverlapCircle(player.transform.position, detectionRadius, NPCLayer);
        GameObject newNPC = npcCollider ? npcCollider.gameObject : null;

        if (newNPC == null)
        {
            if (currentInteractPrompt != null)
            {
                Destroy(currentInteractPrompt);
                currentInteractPrompt = null;
            }
            CloseDialogueBox();
            currentNPC = null;
            return;
        }
        if (currentNPC != newNPC)
        {
            if (currentInteractPrompt != null)
            {
                Destroy(currentInteractPrompt);
                currentInteractPrompt = null;
            }
        }

        // Assign new NPC
        currentNPC = newNPC;
        dialogueBoxHandler = currentNPC.GetComponent<DialogueBoxHandler>();
        dialogueProfile.sprite = dialogueBoxHandler.npcProfile;

        Color color = continueArrow.color;
        color.a = typeWriter.isTyping ? 0 : 1f;
        continueArrow.color = color;

        // If there's no prompt, instantiate one
        if (currentInteractPrompt == null)
        {
            currentInteractPrompt = Instantiate(
                interactPromptPrefab,
                Camera.main.WorldToScreenPoint(currentNPC.transform.position + Vector3.up * 1.5f),
                Quaternion.identity,
                GameObject.FindGameObjectWithTag("Overworld UI").transform
            );
            currentInteractPrompt.transform.SetSiblingIndex(0);
        }
        else
        {
            currentInteractPrompt.transform.position = Camera.main.WorldToScreenPoint(currentNPC.transform.position + Vector3.up * 1.5f);
        }

        // Handle interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentNPC.CompareTag("Interactable"))
            {
                if (currentSmallDialogueBox == null)
                {
                    currentSmallDialogueBox = Instantiate(
                        smallDialogueBox,
                        Camera.main.WorldToScreenPoint(currentNPC.transform.position + Vector3.up * 1.7f),
                        Quaternion.identity,
                        GameObject.FindGameObjectWithTag("Overworld UI").transform);
                        
                    currentSmallDialogueBox.transform.SetParent(currentInteractPrompt.transform);

                    smallDialogueText = currentSmallDialogueBox.GetComponentInChildren<TextMeshProUGUI>();
                    dialogueBoxHandler.currentLineIndex = 0;
                }
                smallDialogueText.text = dialogueBoxHandler.GetCurrentDialogueLine();
            }
            else if (currentNPC.CompareTag("NPC"))
            {
                OpenDialogueBox();
            }
        }
    }

    void OpenDialogueBox()
    {
        if (isDialogueActive) {
            UpdateDialogueBox();
            return;
        }

        isDialogueActive = true;
        dialogueBoxHandler.currentLineIndex = 0;
        dialogueBoxHandler.lastLineDisplayed = false;

        dialogueName.text = currentNPC.name;
        dialogueAnimator.Play("Dialogue Appear");
        darkScreenAnimator.Play("Darken Screen");

        // if (dialogueBoxHandler.hasChoice) {ShowChoice();}
        typeWriter.StartTypewriter(dialogueBoxHandler.GetCurrentDialogueLine());
    }
    void UpdateDialogueBox()
    {
        if (typeWriter.isTyping)
        {
            typeWriter.skipTyping = true;
            return;
        }
        else if (dialogueBoxHandler.CanClose())
        {
            CloseDialogueBox();
            return;
        }

        // if (dialogueBoxHandler.hasChoice) {ShowChoice();}
        typeWriter.StartTypewriter(dialogueBoxHandler.GetCurrentDialogueLine());
    }
    void CloseDialogueBox()
    {
        if (!isDialogueActive) {return;}

        isDialogueActive = false;
        darkScreenAnimator.Play("Lighten Screen");
        dialogueAnimator.Play("Dialogue Dissapear");
    }
    
    // void ShowChoice()
    // {
    //     dialogueOptions.SetActive(true);
    // }
    // public void OnChoiceMade(string choice)
    // {
    //     if (choice == "A")
    //     {
    //         currentDialogue = positiveAnswer;
    //     }
    //     else if (choice == "B")
    //     {
    //         currentDialogue = negativeAnswer;
    //     }

    //     dialogueBoxHandler.dialogueContents = currentDialogue;
    //     dialogueBoxHandler.currentLineIndex = 0;
    //     dialogueBoxHandler.lastLineDisplayed = false;
    // }
}
