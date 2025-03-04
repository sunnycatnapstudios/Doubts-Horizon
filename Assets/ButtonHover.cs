using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform buttonPosition;
    private RectTransform shadowRect;
    private Vector2 defaultPos, targetPos;
    public Vector2 liftAmount;
    public Color shadowColor;
    private float liftSpeed = 20f;
    private GameObject shadowObject;

    void Start()
    {
        buttonPosition = GetComponent<RectTransform>();
        defaultPos = buttonPosition.anchoredPosition;
        targetPos = defaultPos;

        CreateShadow();
    }

    private void CreateShadow() // A quick means to create shadows for buttons that hover
    {
        shadowObject = new GameObject($"{this.name} Shadow");
        shadowObject.transform.SetParent(transform.parent, false); // Set same parent but not as child

        shadowRect = shadowObject.AddComponent<RectTransform>();
        // Copy all RectTransform properties
        shadowRect.anchorMin = buttonPosition.anchorMin;
        shadowRect.anchorMax = buttonPosition.anchorMax;
        shadowRect.pivot = buttonPosition.pivot;
        shadowRect.sizeDelta = buttonPosition.sizeDelta;
        shadowRect.anchoredPosition = buttonPosition.anchoredPosition;
        shadowRect.localScale = buttonPosition.localScale;
        shadowRect.rotation = buttonPosition.rotation;

        // Add and configure shadow image
        Image shadowImage = shadowObject.AddComponent<Image>();
        Image buttonImage = GetComponent<Image>(); // Get the original button image
        if (buttonImage != null)
        {
            shadowImage.sprite = buttonImage.sprite;
            shadowImage.type = buttonImage.type;  // Handles Simple, Sliced, Tiled, and Filled images
            shadowImage.pixelsPerUnitMultiplier = buttonImage.pixelsPerUnitMultiplier;
            shadowImage.preserveAspect = buttonImage.preserveAspect;
            shadowImage.fillCenter = buttonImage.fillCenter; // Important for sliced images

            // Handle Fill Type (for Filled images)
            shadowImage.fillMethod = buttonImage.fillMethod;
            shadowImage.fillAmount = buttonImage.fillAmount;
            shadowImage.fillClockwise = buttonImage.fillClockwise;
            shadowImage.fillOrigin = buttonImage.fillOrigin;
        }
        shadowImage.color = (shadowColor == Color.clear) ? Color.black : shadowColor;

        shadowObject.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPos = defaultPos + liftAmount;
        StopAllCoroutines();
        StartCoroutine(LerpButtonPosition(targetPos));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetPos = defaultPos;
        StopAllCoroutines();
        StartCoroutine(LerpButtonPosition(targetPos));
    }

    private IEnumerator LerpButtonPosition(Vector2 target)
    {
        while ((buttonPosition.anchoredPosition - target).sqrMagnitude > 0.01f)
        {
            buttonPosition.anchoredPosition = Vector2.Lerp(buttonPosition.anchoredPosition, target, Time.unscaledDeltaTime * liftSpeed);
            yield return null;
        }
        buttonPosition.anchoredPosition = target;
    }
}
