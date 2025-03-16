using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class TitleToHorizon : MonoBehaviour {
    public bool isClicked = false; // Ensure no spam clicking
    public GameObject blackFader;
    RawImage blackFaderImage;

    public TitleToHorizonDialogHandler textHandler;

    // Trigger on button click
    public void OnStartButtonClicked() {
        if (isClicked) {
            return;
        } else {
            isClicked = true;
        }
        // Fade the canvas to black
        StartCoroutine(FadeOutToBlack());

        // After fade to black, begin type writer
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
        textHandler.StartDialogue();
    }
    public void Awake() {
        blackFaderImage = blackFader.GetComponent<RawImage>();
        textHandler = GetComponent<TitleToHorizonDialogHandler>();
    }

    public void Start() {
            // TODO tetsing
    }
}
