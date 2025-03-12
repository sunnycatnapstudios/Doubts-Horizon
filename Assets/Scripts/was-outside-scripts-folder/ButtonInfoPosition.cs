using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInfoPosition : MonoBehaviour {
    private Canvas canvas;
    public RectTransform buttonInfo, canvasRect;
    private TextMeshProUGUI infoButtonText;
    private CanvasGroup buttonCanvasGroup;
    private Coroutine openBoxCoroutine, closeBoxCoroutine, jutterCoroutine;
    private bool isHovering = false;
    public float infoWidth, jutterOffset;

    void OnEnable() {
        canvas = GetComponentInParent<Canvas>();
        buttonCanvasGroup = GetComponent<CanvasGroup>();
        buttonInfo = GetComponent<RectTransform>();
        canvasRect = canvas.GetComponent<RectTransform>();
        infoButtonText = GetComponentInChildren<TextMeshProUGUI>();

        buttonCanvasGroup.alpha = 0f;
        buttonCanvasGroup.interactable = false;
        buttonCanvasGroup.blocksRaycasts = false;

        buttonInfo.sizeDelta = new Vector2(0f, buttonInfo.sizeDelta.y);
        infoButtonText.text = "";
    }

    public void IWantAHint(List<string> buttonInfoList) {
        if (closeBoxCoroutine != null) StopCoroutine(closeBoxCoroutine);
        if (openBoxCoroutine != null) StopCoroutine(openBoxCoroutine); closeBoxCoroutine = StartCoroutine(CloseBox(true));
        if (jutterCoroutine != null) StopCoroutine(jutterCoroutine);

        // closeBoxCoroutine = StartCoroutine(CloseBox(true));
        openBoxCoroutine = StartCoroutine(OpenBox(buttonInfoList));
    }
    public void NVM(bool quickClose) {
        if (openBoxCoroutine != null) StopCoroutine(openBoxCoroutine);
        if (closeBoxCoroutine != null) StopCoroutine(closeBoxCoroutine);
        closeBoxCoroutine = StartCoroutine(CloseBox(quickClose));
    }
    private IEnumerator OpenBox(List<string> textList) {
        isHovering = true;
        yield return new WaitForSecondsRealtime(.8f);

        if (isHovering) {
            string buttonText = textList[UnityEngine.Random.Range(0, textList.Count)];
            infoButtonText.text = buttonText;
            infoWidth = ((infoButtonText.preferredWidth / 2) > 150) ? (infoButtonText.preferredWidth / 2) : 150;

            float elapsedTime = 0f, duration = 0.2f;
            Vector2 startSize = new Vector2(0f, buttonInfo.sizeDelta.y);
            Vector2 targetSize = new Vector2(infoWidth, buttonInfo.sizeDelta.y);
            float startAlpha = 0f, targetAlpha = 1f;

            while (elapsedTime < duration) {
                float t = elapsedTime / duration;
                float easedT = 1f - Mathf.Pow(1f - t, 3); // Smooth ease-out

                buttonInfo.sizeDelta = Vector2.Lerp(startSize, targetSize, easedT);
                buttonCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, easedT);

                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            jutterCoroutine = StartCoroutine(JutterEffect());
            buttonInfo.sizeDelta = targetSize;
            buttonCanvasGroup.alpha = targetAlpha;
        }
    }
    private IEnumerator JutterEffect() {
        float jutterDuration = 0.1f;
        float jutterAmount = 10f;
        float elapsedTime = 0f;
        float speedFactor = 1f;

        for (int i = 0; i < 7f; i++) {
            // Move to the right
            while (elapsedTime < jutterDuration) {
                jutterOffset = Mathf.Lerp(0f, jutterAmount, elapsedTime / jutterDuration);
                elapsedTime += Time.unscaledDeltaTime * 2f * speedFactor;
                yield return null;
            }

            // Move back to the center
            elapsedTime = 0f;
            while (elapsedTime < jutterDuration) {
                jutterOffset = Mathf.Lerp(jutterAmount, 0f, elapsedTime / jutterDuration);
                elapsedTime += Time.unscaledDeltaTime * speedFactor;
                yield return null;
            }
            jutterOffset = 0f;
            elapsedTime = 0f;
            speedFactor = ((((7f - i) / 7) * .6f) + .4f);
            yield return new WaitForSecondsRealtime(1f);
        }
    }
    private IEnumerator CloseBox(bool quickClose) {
        isHovering = false;
        float elapsedTime = 0f, duration = quickClose ? .05f : .15f;
        jutterOffset = 0f;

        Vector2 startSize = buttonInfo.sizeDelta, targetSize = new Vector2(0f, startSize.y);
        float startAlpha = buttonCanvasGroup.alpha, targetAlpha = 0f;

        yield return new WaitForSecondsRealtime(.1f);

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3); // Smooth ease-out

            buttonInfo.sizeDelta = Vector2.Lerp(startSize, targetSize, easedT);
            buttonCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, easedT);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        buttonInfo.sizeDelta = targetSize;
        buttonCanvasGroup.alpha = targetAlpha;
        infoButtonText.text = "";
    }

    void Update() {
        if (canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 localMousePos
        );

        // Offset the button's position relative to the mouse
        buttonInfo.anchoredPosition = new Vector2(localMousePos.x + (buttonInfo.sizeDelta.x / 2f) + 5f + jutterOffset, localMousePos.y + (buttonInfo.sizeDelta.y / 2f) + 5f);
    }
}
