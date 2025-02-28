using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class _DialogueInputHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Player player;
    private _DialogueHandler dialogueHandler;
    private DialogueButtons dialogueButtons;
    private TextMeshProUGUI choiceAText, choiceBText;
    public CanvasGroup DialogueInputCanvasGroup;
    public bool hovering;

    void OnEnable()
    {
        dialogueButtons = GetComponent<DialogueButtons>();
        dialogueHandler = FindObjectOfType<_DialogueHandler>();

        DialogueInputCanvasGroup = GetComponent<CanvasGroup>();

        DialogueInputCanvasGroup.alpha = 0f;
        // DialogueInputCanvasGroup.interactable
        DialogueInputCanvasGroup.blocksRaycasts = false;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // Debug.Log("DialogueInPutHandler OnPointerEnter()");
        hovering = true;
    }
    public void OnPointerExit(PointerEventData eventData) {
        // Debug.Log("DialogueInPutHandler OnPointerExit()");
        hovering = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
