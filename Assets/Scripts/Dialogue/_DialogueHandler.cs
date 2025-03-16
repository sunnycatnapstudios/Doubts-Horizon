using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class _DialogueHandler : MonoBehaviour {
    private float detectionRadius = 1.2f;
    public GameObject currentNPC, newNPC;

    private GameObject player,
        dialogueBox,
        currentInteractPrompt,
        currentSmallDialogueBox,
        dialogueOptions,
        continueArrow;

    public GameObject interactPromptPrefab, smallDialogueBox;
    public LayerMask NPCLayer;

    private TextMeshProUGUI dialogueText, dialogueName, smallDialogueText;
    private Image dialogueProfile;
    private Animator dialogueAnimator, darkScreenAnimator;
    private bool isDialogueActive = false;

    private DialogueBoxHandler dialogueBoxHandler;
    private _DialogueInputHandler _dialogueInputHandler;
    private TypeWriter typeWriter;

    private List<string> positiveAnswer, negativeAnswer;
    private List<string> currentDialogue;


    void Awake() {
        GameObject UICanvas = GameObject.FindWithTag("UI Canvas");

        player = GameObject.FindWithTag("Player");
        dialogueBox = UICanvas.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.CompareTag("Dialogue Box"))?.gameObject;

        // dialogueProfile = UICanvas.FindWithTag("Character Profile").GetComponent<Image>();
        dialogueProfile = dialogueBox.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.CompareTag("Character Profile"))?.gameObject.GetComponent<Image>();

        // dialogueText = UICanvas.FindWithTag("Dialogue Text").GetComponentInChildren<TextMeshProUGUI>();
        dialogueText = dialogueBox.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.CompareTag("Dialogue Text"))?.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        // dialogueName = UICanvas.FindWithTag("Name Card").GetComponentInChildren<TextMeshProUGUI>();
        dialogueName = dialogueBox.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.CompareTag("Name Card"))?.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        dialogueAnimator = dialogueBox.GetComponent<Animator>();


        // darkScreenAnimator = UICanvas.FindWithTag("Dark Screen").GetComponent<Animator>();
        darkScreenAnimator = UICanvas.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.CompareTag("Dark Screen"))?.gameObject.GetComponent<Animator>();

        typeWriter = dialogueBox.GetComponentInChildren<TypeWriter>();

        // dialogueOptions = UICanvas.FindWithTag("Dialogue Options");
        dialogueOptions = dialogueBox.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.CompareTag("Dialogue Options"))?.gameObject;

        _dialogueInputHandler = dialogueBox.GetComponentInChildren<_DialogueInputHandler>();

        // dialogueOptions.SetActive(false);

        interactPromptPrefab = GameObject.FindWithTag("InteractPrompt");
        smallDialogueBox = GameObject.FindWithTag("SmallDialogueBox");
    }

    void Start() {
        continueArrow = GameObject.FindWithTag("Continue Arrow");
    }

    void Update() {
        Collider2D npcCollider = Physics2D.OverlapCircle(player.transform.position, detectionRadius, NPCLayer);
        newNPC = npcCollider ? npcCollider.gameObject : null;

        if (newNPC == null) {
            if (currentInteractPrompt != null) {
                Destroy(currentInteractPrompt);
                currentInteractPrompt = null;
            }

            currentNPC = null;
            CloseDialogueBox();
            return;
        }

        if (currentNPC != newNPC) {
            if (currentInteractPrompt != null) {
                Destroy(currentInteractPrompt);
                currentInteractPrompt = null;
            }

            currentNPC = null;
            CloseDialogueBox();
            if (isDialogueActive) return;
        }

        // Assign new NPC
        currentNPC = newNPC;
        dialogueBoxHandler = currentNPC.GetComponent<DialogueBoxHandler>();
        if (dialogueBoxHandler == null) {
            return;
        }

        dialogueProfile.sprite = dialogueBoxHandler.npcProfile;

        continueArrow.SetActive(!typeWriter.isTyping || typeWriter.waitingForPause);

        // If there's no prompt, instantiate one
        if (currentInteractPrompt == null) {
            currentInteractPrompt = Instantiate(
                interactPromptPrefab,
                Camera.main.WorldToScreenPoint(currentNPC.transform.position + Vector3.up * 1.5f),
                Quaternion.identity,
                GameObject.FindGameObjectWithTag("Overworld UI").transform
            );
            currentInteractPrompt.transform.SetSiblingIndex(0);
        } else {
            currentInteractPrompt.transform.position =
                Camera.main.WorldToScreenPoint(currentNPC.transform.position + Vector3.up * 1.5f);
        }

        // Handle interaction
        if (Input.GetKeyDown(KeyCode.E)) {
            if (currentNPC.CompareTag("Interactable")) {
                if (currentSmallDialogueBox == null) {
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
            } else if (currentNPC.CompareTag("NPC")) {
                OpenDialogueBox();
            }
        }
    }

    void UpdateTypewriter() {
        // Set talking sfx to current clip set in dialogBoxHandler, which is determined by attached NPC dialog script
        typeWriter.SetSfxTypingClip(dialogueBoxHandler.SfxTalkingClip);
        typeWriter.StartTypewriter(dialogueBoxHandler.GetCurrentDialogueLine());
        typeWriter.skipTyping = false;
        typeWriter.hasStartedTyping = true;
    }

    public void OpenDialogueBox() {
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

        player.GetComponent<Player>().isPlayerInControl = true;

        // if (dialogueBoxHandler.hasChoice) {ShowChoice();}
        UpdateTypewriter();
    }

    public void UpdateDialogueBox() {
        if (typeWriter.isTyping) {
            if (typeWriter.waitingForPause) {
                typeWriter.waitingForPause = false;
                return;
            } else {
                typeWriter.skipTyping = true;
                return;
            }
        } else if (dialogueBoxHandler.CanClose()) {
            CloseDialogueBox();
            return;
        }

        // if (dialogueBoxHandler.hasChoice) {ShowChoice();}
        UpdateTypewriter();
    }

    public void CloseDialogueBox() {
        if (!isDialogueActive) {
            return;
        }

        isDialogueActive = false;
        darkScreenAnimator.Play("Lighten Screen");
        dialogueAnimator.Play("Dialogue Dissapear");

        player.GetComponent<Player>().isPlayerInControl = false;

        if (dialogueBoxHandler.afterDialogue != null) {
            dialogueBoxHandler.afterDialogue();
            dialogueBoxHandler.afterDialogue = null;
        }
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
