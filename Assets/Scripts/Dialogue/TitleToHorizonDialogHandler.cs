using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleToHorizonDialogHandler : MonoBehaviour {
    private GameObject continueArrow; // To display

    private bool isDialogueActive = false;

    private TypeWriter typeWriter;

    [SerializeField] public AudioClip sfxTypingClip;

    private List<string> dialogueContents = new List<string> {
        "....",
        "...........",
        "I hear something...",
        ".. .. ..",
        "Where am I?"
    };

    private int currentLineIndex = 0;

    void Awake() {
        continueArrow = GameObject.FindWithTag("Continue Arrow");

        typeWriter = GameObject.FindWithTag("Dialogue Text").GetComponent<TypeWriter>();
        typeWriter.SetSfxTypingClip(sfxTypingClip);
    }

    public void StartDialogue() {
        continueArrow.SetActive(true);
        StartCoroutine(AnimateMovingEBox());
        typeWriter.SetSfxTypingClip(sfxTypingClip);

        isDialogueActive = true;
    }

    public void Update() {
        // Once dialog has begun, start updating text
        if ((Input.GetKey(KeyCode.E) || Input.GetMouseButtonDown(0)) && !typeWriter.isTyping && isDialogueActive) {
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

    private IEnumerator AnimateMovingEBox() {
        Vector3 startPos = continueArrow.transform.position;
        Vector3 target = new Vector3(6.6f, -5f, continueArrow.transform.position.z);
        float duration = 1.0f; // Animation duration in seconds
        float elapsedTime = 0;

        while (elapsedTime < duration) {
            float t = elapsedTime / duration; // Calculate percentage complete (0 to 1)

            t = Mathf.SmoothStep(0, 1, t); // Add easing for smoother animation

            // Lerp between start and target positions
            continueArrow.transform.position = Vector3.Lerp(startPos, target, t);

            elapsedTime += Time.deltaTime; // Increment time

            yield return null;
        }
    }
}
