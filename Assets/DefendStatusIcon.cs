using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DefendStatusIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    private RectTransform defendStatusIcon;
    private Vector2 defaultSize = new Vector2(25f, 25f), targetSize, startPivot = new Vector2(.5f, .5f);
    private Vector2 enabledStartPosition = new Vector2(160f, -22.5f), endPosition = new Vector2(-13f, -22.5f), startPosition = new Vector2(32.5f, -22.5f);
    private Vector2 tempPosition = new Vector2(47.5f, -22.5f);
    public DefendIndicator defendIndicator;
    public PartySlotHandler partySlotHandler;
    private Coroutine ExpandOut, ExpandIn;
    private Image iconImage;
    private float expandSpeed = 20f, expand = 30f;
    // private bool inAnimation = false;
    public bool triggerSlideIn, triggerSlideOut;

    void Start() {
        defendStatusIcon = GetComponent<RectTransform>();
        iconImage = GetComponent<Image>();
        targetSize = new Vector2(expand, expand);
    }
    void OnEnable() {
        defendStatusIcon = GetComponent<RectTransform>();
        defendStatusIcon.anchoredPosition = enabledStartPosition;
        triggerSlideIn = triggerSlideOut = false;
    }

    void TriggerSlideIn() {
        triggerSlideIn = false;
        StartCoroutine(SlideIn());
    }
    void TriggerSlideOut() {
        triggerSlideOut = false;
        StartCoroutine(SlideOut());
    }
    public IEnumerator SlideIn() {
        defendStatusIcon.anchoredPosition = enabledStartPosition;
        iconImage.preserveAspect = false;

        float time = 0f, duration = .5f, sizeDuration = .125f;
        Vector2 initialWidth = defendStatusIcon.sizeDelta;
        float maxStretch = Mathf.Abs(enabledStartPosition.x - startPosition.x) / 2f;

        while (time < duration) {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            float sizeT = Mathf.Clamp01(time / sizeDuration);
            float stretch = Mathf.Sin(sizeT * Mathf.PI) * maxStretch;

            defendStatusIcon.anchoredPosition = Vector2.Lerp(defendStatusIcon.anchoredPosition, startPosition, t);
            defendStatusIcon.sizeDelta = initialWidth + new Vector2(stretch, 0f);
            yield return null;
        }
        defendStatusIcon.anchoredPosition = startPosition;
        defendStatusIcon.sizeDelta = initialWidth;
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = true;
    }
    public IEnumerator SlideOut() {
        iconImage.preserveAspect = false; iconImage.raycastTarget = false;
        defendStatusIcon.anchoredPosition = startPosition;

        float time = 0f, duration = .5f, sizeDuration = .125f;
        Vector2 initialWidth = defendStatusIcon.sizeDelta;
        float maxStretch = Mathf.Abs(tempPosition.x - endPosition.x) / 2f;

        while (time < .2f) {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            defendStatusIcon.anchoredPosition = Vector2.Lerp(defendStatusIcon.anchoredPosition, tempPosition, t);
            yield return null;
        }

        defendStatusIcon.anchoredPosition = tempPosition;
        time = 0f;

        while (time < duration) {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            float sizeT = Mathf.Clamp01(time / sizeDuration);
            float stretch = Mathf.Sin(sizeT * Mathf.PI) * maxStretch;

            defendStatusIcon.anchoredPosition = Vector2.Lerp(defendStatusIcon.anchoredPosition, endPosition, t);
            defendStatusIcon.sizeDelta = initialWidth + new Vector2(stretch, 0f);
            yield return null;
        }

        defendStatusIcon.anchoredPosition = endPosition;
        iconImage.preserveAspect = true;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // inAnimation = true;
        if (ExpandIn != null) StopCoroutine(ExpandIn);
        ExpandOut = StartCoroutine(LerpIconSize(targetSize));
    }

    public void OnPointerExit(PointerEventData eventData) {
        // inAnimation = true;
        if (ExpandOut != null) StopCoroutine(ExpandOut);
        ExpandIn = StartCoroutine(LerpIconSize(defaultSize));
    }
    public void OnPointerClick(PointerEventData eventData) {
        if (partySlotHandler._battleUIHandler.currentDefender != null) {
            partySlotHandler.MoveToActivePlayer(partySlotHandler._battleUIHandler.currentDefender, false);
            StartCoroutine(BobIcon(10f, 0.25f));
        } else {
            StartCoroutine(ShakeDefendIcon(.3f, .8f));
            // StartCoroutine(BobIcon(10f, 0.25f));
        }
    }
    public IEnumerator ShakeDefendIcon(float duration, float strength) {
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float jitterAmount = Mathf.Sin(elapsedTime * 30f) * strength;
            defendStatusIcon.pivot = startPivot + new Vector2(jitterAmount, 0f);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        defendStatusIcon.pivot = startPivot;
    }
    private IEnumerator BobIcon(float bobHeight, float duration) {
        Vector2 targetPosition = startPosition + new Vector2(0, bobHeight);
        float time = 0f;

        while (time < duration) {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            float offset = Mathf.Sin(t * Mathf.PI) * bobHeight;

            defendStatusIcon.anchoredPosition = startPosition + new Vector2(0, offset);
            yield return null;
        }
        defendStatusIcon.anchoredPosition = startPosition;
    }

    private IEnumerator LerpIconSize(Vector2 target) {
        if (target == targetSize) { iconImage.maskable = false; }

        while ((defendStatusIcon.sizeDelta - target).sqrMagnitude > 0.01f) {
            defendStatusIcon.sizeDelta = Vector2.Lerp(defendStatusIcon.sizeDelta, target, Time.unscaledDeltaTime * expandSpeed);
            yield return null;
        }
        defendStatusIcon.sizeDelta = target;

        if (target == defaultSize) { iconImage.maskable = true; }
        // inAnimation = false;
    }

    void Update() {
        bool defenderSet = (partySlotHandler._battleUIHandler.currentDefender != null);

        if (triggerSlideIn) TriggerSlideIn();
        if (triggerSlideOut) TriggerSlideOut();
    }
}
