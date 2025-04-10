using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class _DialogueHandler : MonoBehaviour {
    private float detectionRadius = 1.2f;
    public GameObject currentNPC, newNPC;
    private GameObject overworldUI;

    private GameObject player,
        dialogueBox,
        currentInteractPrompt,
        currentSmallDialogueBox,
        dialogueOptions,
        continueArrow;

    public GameObject interactPromptPrefab, smallDialogueBox;
    public LayerMask NPCLayer;

    [System.NonSerialized]
    public TextMeshProUGUI dialogueText, dialogueName, smallDialogueText;
    public Image dialogueProfile;
    private Animator dialogueAnimator, darkScreenAnimator;
    private bool isDialogueActive = false;

    private DialogueBoxHandler dialogueBoxHandler;
    private _DialogueInputHandler _dialogueInputHandler;
    private TypeWriter typeWriter;

    private List<string> positiveAnswer, negativeAnswer;
    private List<string> currentDialogue;
    public bool isCloseable = true;


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

        overworldUI = GameObject.FindGameObjectWithTag("Overworld UI");
    }

    void Start() {
        continueArrow = GameObject.FindWithTag("Continue Arrow");
    }

    GameObject GetNearbyNpc() {
        Collider2D npcCollider = Physics2D.OverlapCircle(player.transform.position, detectionRadius, NPCLayer);
        return npcCollider ? npcCollider.gameObject : null;
    }

    void SetCurrentNpc(GameObject newNPC) {
        if (newNPC == null) {
            if (currentInteractPrompt != null) {
                Destroy(currentInteractPrompt);
                currentInteractPrompt = null;
            }

            currentNPC = null;
            //             CloseDialogueBox();
            return;
        }

        if (currentNPC != newNPC) {
            if (currentInteractPrompt != null) {
                Destroy(currentInteractPrompt);
                currentInteractPrompt = null;
            }

            currentNPC = null;
            //             CloseDialogueBox();
// returning early here will screw up where we want to set a new npc for doing two dialogues in a row
// I moved the statement `isDialogueActive = false` inside of CloseDialogueBox inside the isCloseable loop
// and it caused this to be an issue.
//             if (isDialogueActive) {
//                 Debug.Log("return early");
//                 return;
//             }
        }

        // Assign new NPC
        currentNPC = newNPC;
        dialogueBoxHandler = currentNPC.GetComponent<DialogueBoxHandler>();
        if (dialogueBoxHandler == null) {
            return;
        }

        dialogueProfile.sprite = dialogueBoxHandler.npcProfile;
    }

    public void OpenDialogueWith(GameObject dialogueSource) {
        Debug.Log($"OpenDialogueWith {dialogueSource.name}");
        SetCurrentNpc(dialogueSource);
        OpenDialogueBox();
    }

    void Update() {
        if (!isDialogueActive) {
            SetCurrentNpc(GetNearbyNpc());
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            if (isDialogueActive) {
                UpdateDialogueBox();
                return;
            }
        }

        if (currentNPC == null) {
            return;
        }

        continueArrow.SetActive(!typeWriter.isTyping || typeWriter.waitingForPause);

        if (!isDialogueActive) {
            // If there's no prompt, instantiate one
            if (currentInteractPrompt == null) {
                if (overworldUI.activeSelf) {
                    currentInteractPrompt = Instantiate(
                        interactPromptPrefab,
                        Camera.main.WorldToScreenPoint(currentNPC.transform.position + Vector3.up * 1.5f),
                        Quaternion.identity,
                        overworldUI.transform
                    );
                    currentInteractPrompt.transform.SetSiblingIndex(0);
                }
            } else {
                currentInteractPrompt.transform.position =
                    Camera.main.WorldToScreenPoint(currentNPC.transform.position + Vector3.up * 1.5f);
            }
            currentInteractPrompt.SetActive(true);
        } else if (currentInteractPrompt != null) {
            currentInteractPrompt.SetActive(false);
        }

        // Handle interaction
        if (Input.GetKeyDown(KeyCode.E) && overworldUI.activeSelf && !player.GetComponent<Player>().isPlayerInControl && player.GetComponent<Player>().canControlCam) {
            if (currentNPC.CompareTag("Interactable")) {
                if (currentSmallDialogueBox == null) {
                    currentSmallDialogueBox = Instantiate(
                        smallDialogueBox,
                        Camera.main.WorldToScreenPoint(currentNPC.transform.position + Vector3.up * 1.7f),
                        Quaternion.identity,
                        overworldUI.transform);

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
        string line = dialogueBoxHandler.GetCurrentDialogueLine();
        Debug.Log($"UpdateTypewriter with {line}");
        typeWriter.StartTypewriter(line);
        typeWriter.skipTyping = false;
        typeWriter.hasStartedTyping = true;
    }

    public void OpenDialogueBox() {
        dialogueName.text = currentNPC ? currentNPC.name : "???";

        if (dialogueBoxHandler.beforeDialogue != null) {
            Debug.Log("Calling beforeDialogue");
            var toCall = dialogueBoxHandler.beforeDialogue;
            dialogueBoxHandler.beforeDialogue = null;
            toCall();
        } else {
            Debug.Log("Before Dialogue is null");
        }

        isDialogueActive = true;
        dialogueBoxHandler.currentLineIndex = 0;
        dialogueBoxHandler.lastLineDisplayed = false;

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
        } else {
            if (dialogueBoxHandler != null && dialogueBoxHandler.CanClose()) {
                Debug.Log("UpdateDialogueBox and not typing and can close");
                CloseDialogueBox();
            }
            return;
        }

        // if (dialogueBoxHandler.hasChoice) {ShowChoice();}
        UpdateTypewriter();
    }

    public void CloseDialogueBox() {
        if (!isDialogueActive) {
            return;
        }

        Debug.Log("in CloseDialogueBox");

        if (dialogueBoxHandler.afterDialogue != null) {
            Debug.Log("Calling afterDialogue");
            var toCall = dialogueBoxHandler.afterDialogue;
            dialogueBoxHandler.afterDialogue = null;
            toCall();
        }

        if (isCloseable) {
            Debug.Log("isCloseable");
            isDialogueActive = false;
            darkScreenAnimator.Play("Lighten Screen");
            dialogueAnimator.Play("Dialogue Dissapear");
            player.GetComponent<Player>().isPlayerInControl = false;
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
