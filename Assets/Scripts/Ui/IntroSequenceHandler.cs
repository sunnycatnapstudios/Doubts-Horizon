using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSequenceHandler : MonoBehaviour {
    // Two overlays used for the red intro
    public GameObject radicalOverlay;
    public GameObject solidOverlay;

    private Image radicalImage;
    private Image solidImage;

    void Start() {
        // Fetch the two overlays and disable by default
        if (radicalOverlay == null) {
            radicalOverlay = GameObject.FindGameObjectWithTag("Intro Radical Overlay");
        }

        if (solidOverlay == null) {
            solidOverlay = GameObject.FindGameObjectWithTag("Intro Solid Overlay");
        }

        radicalOverlay.SetActive(false);
        solidOverlay.SetActive(false);
        radicalImage = radicalOverlay.GetComponent<Image>();
        solidImage = solidOverlay.GetComponent<Image>();
    }


    public void StartIntroSequence() {
        StartCoroutine(FadeOutFromSolid());
        radicalOverlay.SetActive(true);
    }

    // Call to disable overlays and other logic
    public void EndIntroSequence() {
        radicalOverlay.SetActive(false);
        solidOverlay.SetActive(false);
    }

    // Yes I'm reusing the titleToHorizon code cause I'm a lazy bastard
    private IEnumerator FadeOutFromSolid() {
        solidOverlay.gameObject.SetActive(true);
        while (solidImage.color.a > 0) {
            float fadeAmount = solidImage.color.a - (Time.deltaTime * 0.4f);
            Color newColor = new Color(solidImage.color.r, solidImage.color.g, solidImage.color.b,
                fadeAmount);
            solidImage.color = newColor;
            yield return null;
        }

        // Disable the fader
        solidOverlay.gameObject.SetActive(false);
    }
}
