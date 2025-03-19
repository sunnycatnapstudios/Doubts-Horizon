using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleTransition : MonoBehaviour {
    public Image left, right;

    private Transform deathTransform;
    public TextMeshProUGUI text, buttonText;
    public Image black, button;

    public bool _start;
    public float fadeSpeed = 0.5f;

    void Start() {
        left = this.transform.Find("Left").GetComponent<Image>();
        right = this.transform.Find("Right").GetComponent<Image>();

        // Behold, the most jank method of fading in a death animation
        deathTransform = this.transform.Find("Death");
        black = deathTransform.Find("Black").GetComponent<Image>();
        text = deathTransform.Find("Died Text").GetComponent<TextMeshProUGUI>();
        buttonText = deathTransform.Find("Button").Find("DeathButtonText").GetComponent<TextMeshProUGUI>();
        button = deathTransform.Find("Button").GetComponent<Image>();

        left.fillAmount = right.fillAmount = 0;

        left.fillOrigin = (int)Image.OriginHorizontal.Left;
        right.fillOrigin = (int)Image.OriginHorizontal.Right;
    }

    public void LeaveBattle() {
        _start = false;
        StartCoroutine(LeaveBattleAnim());
    }

    public IEnumerator LeaveBattleAnim() {
        left.fillOrigin = (int)Image.OriginHorizontal.Left;
        right.fillOrigin = (int)Image.OriginHorizontal.Right;

        float elapsedTime = 0f;
        float duration = .5f;
        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3);

            left.fillAmount = right.fillAmount = Mathf.Lerp(0f, 1f, easedT);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        left.fillAmount = right.fillAmount = 1f;

        left.fillOrigin = (int)Image.OriginHorizontal.Right;
        right.fillOrigin = (int)Image.OriginHorizontal.Left;

        yield return new WaitForSecondsRealtime(.4f);

        elapsedTime = 0f;
        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3);

            left.fillAmount = right.fillAmount = Mathf.Lerp(1f, 0f, easedT);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        left.fillAmount = right.fillAmount = 0f;

        left.fillOrigin = (int)Image.OriginHorizontal.Left;
        right.fillOrigin = (int)Image.OriginHorizontal.Right;
    }

    public void HadDied() {
        deathTransform.gameObject.SetActive(true);
        _start = false;
        StartCoroutine(HadDiedAnim());
    }

    public IEnumerator HadDiedAnim() {
        while (black.color.a < 1) {
            float fadeAmount = black.color.a + (Time.unscaledDeltaTime * fadeSpeed);
            Color newColor = new Color(black.color.r, black.color.g, black.color.b,
                fadeAmount);
            black.color = newColor;

            newColor = new Color(text.color.r, text.color.g, text.color.b, fadeAmount);
            text.color = newColor;
            buttonText.color = newColor;

            newColor = new Color(button.color.r, button.color.g, button.color.b,
                fadeAmount);
            button.color = newColor;
            yield return null;
        }

        // Show Dialog after fade out
        //textHandler.StartDialogue();
    }

    void Update() {
        if (_start) {
            LeaveBattle();
        }
    }
}
