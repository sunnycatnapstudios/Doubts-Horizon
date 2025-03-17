using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class TitleToHorizon : MonoBehaviour {
    private Coroutine fadeCoroutine; // Used to end fade in early

    public bool isClicked = false; // Ensure no spam clicking
    public GameObject blackFader;
    RawImage blackFaderImage;

    public TitleToHorizonDialogHandler textHandler;

    public AudioClip musicIntro;
    public AudioClip sfxButtonClick;

    // Trigger on button click
    public void OnStartButtonClicked() {
        // Prevent multiple clicks from happening
        if (isClicked) {
            return;
        }

        // Don't allow multiple retriggers
        isClicked = true;
        AudioManager.Instance.PlaySound(sfxButtonClick);

        Debug.Log("start");
        // Stop the fade in coroutine
        StopCoroutine(fadeCoroutine);

        // Set the arrow above the fade for cool effect (Yes the order in hierarchy is very important)
        GameObject continueArrow = GameObject.FindWithTag("Continue Arrow");
        continueArrow.transform.SetParent(continueArrow.transform.parent.parent);
        continueArrow.transform.SetSiblingIndex(continueArrow.transform.GetSiblingIndex() + 1);

        // Reset fader to transparent before fade
        blackFaderImage.color =
            new Color(blackFaderImage.color.r, blackFaderImage.color.g, blackFaderImage.color.b,
                0);

        // Fade the canvas to black
        StartCoroutine(FadeOutToBlack());
    }

    private IEnumerator FadeOutToBlack() {
        blackFader.gameObject.SetActive(true);

        while (blackFaderImage.color.a < 1) {
            float fadeAmount = blackFaderImage.color.a + (Time.deltaTime * 0.8f);
            Color newColor = new Color(blackFaderImage.color.r, blackFaderImage.color.g, blackFaderImage.color.b,
                fadeAmount);
            blackFaderImage.color = newColor;
            yield return null;
        }

        // Show Dialog after fade out
        blackFader.gameObject.SetActive(true);
        textHandler.StartDialogue();
    }

    private IEnumerator FadeInFromBlack() {
        blackFader.gameObject.SetActive(true);
        while (blackFaderImage.color.a > 0) {
            float fadeAmount = blackFaderImage.color.a - (Time.deltaTime * 0.3f);
            Color newColor = new Color(blackFaderImage.color.r, blackFaderImage.color.g, blackFaderImage.color.b,
                fadeAmount);
            blackFaderImage.color = newColor;
            yield return null;
        }

        // Disable the fader
        blackFader.gameObject.SetActive(false);
    }

    public void Awake() {
        blackFaderImage = blackFader.GetComponent<RawImage>();
        textHandler = GetComponent<TitleToHorizonDialogHandler>();
        fadeCoroutine = StartCoroutine(FadeInFromBlack());
    }

    public void Start() {
        AudioManager.Instance.CrossFadeAmbienceSound(musicIntro, 3, 1, 0);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            OnStartButtonClicked();
        }
    }
}
