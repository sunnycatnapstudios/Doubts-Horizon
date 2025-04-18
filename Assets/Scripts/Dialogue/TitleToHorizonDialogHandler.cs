﻿using System;
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
    private TitleToHorizon titleToHorizon;

    [SerializeField] public AudioClip sfxTypingClip;
    [SerializeField] public AudioClip ambienceIntroWind;
    [SerializeField] public AudioClip sfxExplosionIntro;

    private List<string> dialogueContents = new List<string> {
        "....",
        "...........",
        "I hear something...",
        ".. .. ..",
        "Where am I?",
    };

    private int currentLineIndex = 0;

    void Awake() {
        continueArrow = GameObject.FindWithTag("Continue Arrow");

        typeWriter = GameObject.FindWithTag("Dialogue Text").GetComponent<TypeWriter>();
        titleToHorizon = GetComponent<TitleToHorizon>();
    }

    public void StartDialogue() {
        continueArrow.SetActive(true);
        StartCoroutine(AnimateMovingEBox());

        isDialogueActive = true;
    }

    public void Update() {
        // Once dialog has begun, start updating text
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && !typeWriter.isTyping && isDialogueActive) {
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
        }

        if (currentLineIndex == dialogueContents.Count - 1) {
            // Explosion and fade out at last line (The blank)
            StartCoroutine(CloseDialogueBox());
            //return;
        }

        UpdateTypewriter();
    }

    private IEnumerator CloseDialogueBox() {
        if (!isDialogueActive) {
            yield break;
        }
        isDialogueActive = false;

        // Delay before explosion and fade
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlaySound(sfxExplosionIntro);

        // Fade text out to black
        titleToHorizon.DuplicateBlackFader();
        StartCoroutine(titleToHorizon.FadeOutToBlack());
        AudioManager.Instance.CrossFadeAmbienceToZero(2f);
        yield return new WaitForSeconds(2f);

        // End the intro by loading Horizon Scene
        SceneManager.LoadScene("Horizon", LoadSceneMode.Single);
    }

    private IEnumerator AnimateMovingEBox() {
        // Get canvas object
        Canvas canvas = continueArrow.transform.root.GetComponentInChildren<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Get bottom right corner of canvas
        Vector3[] corners = new Vector3[4];
        canvasRect.GetWorldCorners(corners);
        Vector3 target = corners[3] + new Vector3(-1f, 1f, 0);    // Corner target + padding

        Vector3 startPos = continueArrow.transform.position;
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
