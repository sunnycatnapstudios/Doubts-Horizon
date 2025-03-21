﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractPrompt : MonoBehaviour {
    public LayerMask playerLayer;
    private Player player;
    private float interactRange = 1.5f;
    int interactCount = 0;

    private GameObject popUpPrefab;
    private GameObject currentPopUp;
    private TMP_Text popUpText;
    private Animator popUpAnimator;


    public GameObject interactBoxPrefab;
    private GameObject currentInteractBox;
    private TMP_Text interactBoxText;
    private Animator interactBoxAnimator;
    public string interactBoxTextReplacement;

    private GameObject dialogueBox;
    private TMP_Text nameText, dialogueText;
    private Animator dialogueAnimator, screenPanelAnimator;
    private TypeWriter nameTypeWriter, bodyTypeWriter;
    public Sprite characterProfile;
    private Image charProfile;

    public bool isDialogueOpen = false, dialogueFinished = false;

    public DialogueBoxHandler npcDialogueHandler;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxOnInteract;
        public AudioClip sfxTypewriter;
    }

    [SerializeField] private AudioClips audioClips;

    void OnDrawGizmos() {
        // Draws a Debug for NPC interact radius
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    // public void PopUp(string text)
    // {
    //     if (currentPopUp){
    //         popUpText.text = text;
    //         // popUpAnimator.SetTrigger("pop");
    //     }
    // }

    void OpenDialogue(string text) {
        // dialogueBox.SetActive(true);
        bodyTypeWriter.skipTyping = false;
        dialogueAnimator.Play("Dialogue Appear");
        screenPanelAnimator.SetTrigger("Darken Screen");
        bodyTypeWriter.hasStartedTyping = true;

        if (bodyTypeWriter != null) {
            bodyTypeWriter.SetSfxTypingClip(audioClips.sfxTypewriter);  // Set the talking blip when typewriting
            bodyTypeWriter.StartTypewriter(text);
        } else {
            dialogueText.text = text;
            Debug.LogWarning("Typewriter isn't attached");
        } // If typewriter isn't active

        isDialogueOpen = true;
    }

    void UpdateDialogue(string text) {
        bodyTypeWriter.skipTyping = false;
        bodyTypeWriter.hasStartedTyping = true;
        if (bodyTypeWriter != null) {
            bodyTypeWriter.StartTypewriter(text);
        } else {
            dialogueText.text = text;
            Debug.LogWarning("Typewriter isn't attached");
        } // If typewriter isn't active
    }

    void CloseDialogue() {
        if (isDialogueOpen) {
            screenPanelAnimator.ResetTrigger("Darken Screen");
            dialogueAnimator.Play("Dialogue Dissapear");
            screenPanelAnimator.Play("Lighten Screen");
            screenPanelAnimator.SetTrigger("Lighten Screen");
            StartCoroutine(DeactivateAfterDelay(0.5f));
            isDialogueOpen = false;
            bodyTypeWriter.skipTyping = true;
        }
    }

    IEnumerator DeactivateAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        // charProfile.sprite = null;
    }

    void Awake() {
        dialogueBox = GameObject.FindGameObjectWithTag("Dialogue Box");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        popUpPrefab = GameObject.FindGameObjectWithTag("InteractPrompt");
    }

    void Start() {
        if (CompareTag("NPC")) {
            nameText = GameObject.FindGameObjectWithTag("Name Card").GetComponent<TMP_Text>();
            dialogueText = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<TMP_Text>();
            charProfile = GameObject.FindGameObjectWithTag("Character Profile").GetComponent<Image>();

            if (nameText != null) nameText.text = this.name;
            // Debug.Log("" + nameText.text);

            // nameTypeWriter = nameText.GetComponent<TypeWriter>();
            bodyTypeWriter = dialogueText.GetComponent<TypeWriter>();

            screenPanelAnimator = GameObject.FindGameObjectWithTag("Dark Screen").GetComponent<Animator>();
            screenPanelAnimator.Play("Blank");
            dialogueAnimator = dialogueBox.GetComponent<Animator>();
            dialogueAnimator.Play("Dialogue Hidden");
            // dialogueBox.SetActive(false);
            npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        }
        // if (CompareTag("Interactable")) {
        //     Debug.Log("YEEEEEEEEEEEAAAAAAAAAAAAAAAAAAAAAAAAAHHHHHHHHH");
        // }
    }

    public void forceDialogueEnd() {
        interactCount++;
        if (CompareTag("Interactable")) {
            Debug.Log($"YEP, YOU'VE TAPPED {this.name} {interactCount} TIMES!!!");
        } else if (CompareTag("NPC")) {
            // Debug.Log($"Dialogue Interacted with {interactCount} times");
            if (!isDialogueOpen && !bodyTypeWriter.isTyping) {
                nameText.text = this.name;
                // isDialogueOpen = true;
                charProfile.sprite = characterProfile;
                npcDialogueHandler.ResetDialogue();
                string nextLine = npcDialogueHandler.GetNextLine();

                if (nextLine != null) {
                    OpenDialogue(nextLine);
                    dialogueFinished = false;
                }
            } else if (!bodyTypeWriter.isTyping && !bodyTypeWriter.hasStartedTyping) {
                string nextLine = npcDialogueHandler.GetNextLine();

                if (nextLine != null) {
                    UpdateDialogue(nextLine);
                } else {
                    CloseDialogue();
                    Debug.Log("Finished Dialogue Segment");
                    dialogueFinished = true;
                }
            }
        }


    }
    public void forceFinishDialogue() {
        dialogueFinished = true;
    }

    void Update() {
        bool playerInRange = Physics2D.OverlapCircle(transform.position, interactRange, playerLayer);

        if (playerInRange) {
            if (currentPopUp == null) {
                currentPopUp = Instantiate(popUpPrefab,
                    Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f), Quaternion.identity,
                    GameObject.FindGameObjectWithTag("Overworld UI").transform);
                currentPopUp.transform.SetSiblingIndex(0);
                popUpText = currentPopUp.GetComponentInChildren<TMP_Text>();
                popUpAnimator = currentPopUp.GetComponent<Animator>();
            } else {
                currentPopUp.transform.position =
                    Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
            }

            if (currentInteractBox == null && CompareTag("Interactable")) {
                if (Input.GetKeyDown(KeyCode.E)) {
                    currentInteractBox = Instantiate(interactBoxPrefab,
                        Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.7f), Quaternion.identity,
                        GameObject.FindGameObjectWithTag("Overworld UI").transform);
                    interactBoxText = currentInteractBox.GetComponentInChildren<TMP_Text>();
                    interactBoxText.text = this.interactBoxTextReplacement;
                    popUpAnimator = currentInteractBox.GetComponent<Animator>();
                }
            } else if (CompareTag("Interactable")) {
                currentInteractBox.transform.position =
                    Camera.main.WorldToScreenPoint((transform.position + Vector3.up * 1.7f));
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                interactCount++;
                if (CompareTag("Interactable")) {
                    AudioManager.Instance.PlayUiSound(audioClips.sfxOnInteract);
                    Debug.Log($"YEP, YOU'VE TAPPED {this.name} {interactCount} TIMES!!!");
                } else if (CompareTag("NPC")) {
                    // Debug.Log($"Dialogue Interacted with {interactCount} times");
                    if (!isDialogueOpen && !bodyTypeWriter.isTyping) {
                        nameText.text = this.name;
                        // isDialogueOpen = true;
                        charProfile.sprite = characterProfile;
                        npcDialogueHandler.ResetDialogue();
                        string nextLine = npcDialogueHandler.GetNextLine();

                        if (nextLine != null) {
                            OpenDialogue(nextLine);
                            dialogueFinished = false;
                        }
                    } else if (!bodyTypeWriter.isTyping && !bodyTypeWriter.hasStartedTyping) {
                        string nextLine = npcDialogueHandler.GetNextLine();

                        if (nextLine != null) {
                            UpdateDialogue(nextLine);
                        } else {
                            CloseDialogue();
                            Debug.Log("Finished Dialogue Segment");
                            dialogueFinished = true;
                        }
                    }
                }
            }
        } else {
            if (currentPopUp) {
                Destroy(currentPopUp);
                currentPopUp = null;
            }

            if (currentInteractBox) {
                Destroy(currentInteractBox);
                currentInteractBox = null;
            }

            if (isDialogueOpen) {
                CloseDialogue();
                // charProfile.sprite = null;
            }
        }

        if (dialogueFinished && npcDialogueHandler.afterDialogue != null) {
            npcDialogueHandler.afterDialogue();
            npcDialogueHandler.afterDialogue = null;
        }
    }

    void OnDestroy() {
        if (currentPopUp) {
            Destroy(currentPopUp);
            currentPopUp = null;
        }

        if (currentInteractBox) {
            Destroy(currentInteractBox);
            currentInteractBox = null;
        }

        if (isDialogueOpen) {
            CloseDialogue();
            // charProfile.sprite = null;
        }
    }
}
