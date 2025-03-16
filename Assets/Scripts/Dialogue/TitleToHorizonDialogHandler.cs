using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleToHorizonDialogHandler : MonoBehaviour {
    private GameObject continueArrow; // To display

    private TextMeshProUGUI smallDialogueText;
    private bool isDialogueActive = false;

    private TypeWriter typeWriter;

    public AudioClip sfxTypingClip;

    private List<string> dialogueContents = new List<string> {
        "Thank you so much for saving me from that beast!",
        "My hero <3"
    };

    public int currentLineIndex = 0;
    void Awake() {
        continueArrow = GameObject.FindWithTag("Continue Arrow");

        typeWriter = GameObject.FindWithTag("Dialogue Text").GetComponent<TypeWriter>();
        typeWriter.SetSfxTypingClip(sfxTypingClip);

        smallDialogueText = GameObject.FindWithTag("Dialogue Text").GetComponent<TextMeshProUGUI>();
    }

    public void StartDialogue() {
        //continueArrow.SetActive(!typeWriter.isTyping || typeWriter.waitingForPause);
        typeWriter.SetSfxTypingClip(sfxTypingClip);
        isDialogueActive = true;
    }

    public void Update() {
        // Once dialog has begun, start updating text
        if ((Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetMouseButton(0)) && !typeWriter.isTyping) {
            if (isDialogueActive) {
                UpdateDialogueBox();
                return;
            }
            UpdateTypewriter();
        }
    }

    void UpdateTypewriter() {
        typeWriter.SetSfxTypingClip(sfxTypingClip);
        typeWriter.StartTypewriter(dialogueContents[currentLineIndex++]);
        typeWriter.skipTyping = false;
        typeWriter.hasStartedTyping = true;
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
        } else if (currentLineIndex == dialogueContents.Count) {
            CloseDialogueBox();
            return;
        }

        UpdateTypewriter();
    }

    public void CloseDialogueBox() {
        if (!isDialogueActive) {
            return;
        }

        isDialogueActive = false;

        // Transition to Horizon Scene
        SceneManager.LoadScene("Horizon");
    }
}
