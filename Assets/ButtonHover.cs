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
        shadowRect.sizeDelta = buttonPosition.sizeDelta; // Match button size

        shadowRect.anchoredPosition = defaultPos;

        Image shadowImage = shadowObject.AddComponent<Image>();
        shadowImage.sprite = GetComponent<Image>().sprite;
        shadowImage.color = new Color(0, 0, 0, 0.6f);
        // shadowImage.color = Color.black;

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
