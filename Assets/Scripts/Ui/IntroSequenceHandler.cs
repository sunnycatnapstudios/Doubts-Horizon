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

    private bool wasTriggered = false;

    public AudioClip musicIntro;

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

    // Called in GameStatsManager upon startup
    // Should only trigger when coming from Title scene. Aka not during editing cause it might be annoying
    public void StartIntroSequence() {
        wasTriggered = true;
        StartCoroutine(FadeOutFromSolid(solidOverlay, solidImage, 0.4f));
        radicalOverlay.SetActive(true);
        AudioManager.Instance.CrossFadeMusicSound(musicIntro, 3f);
    }

    // Call to disable overlays and other logic
    public void EndIntroSequence() {
        if (wasTriggered) {
            StartCoroutine(FadeOutFromSolid(radicalOverlay, radicalImage, 0.8f));
            solidOverlay.SetActive(false);
            // Crossfadetozero handled in explosion Audio Transition
        }
    }

    // Yes I'm reusing the titleToHorizon code cause I'm a lazy bastard
    private IEnumerator FadeOutFromSolid(GameObject obj, Image image, float fadeSpeed) {
        obj.gameObject.SetActive(true);
        while (image.color.a > 0) {
            float fadeAmount = image.color.a - (Time.deltaTime * fadeSpeed);
            Color newColor = new Color(image.color.r, image.color.g, image.color.b,
                fadeAmount);
            image.color = newColor;
            yield return null;
        }

        // Disable the fader
        obj.gameObject.SetActive(false);
    }
}
