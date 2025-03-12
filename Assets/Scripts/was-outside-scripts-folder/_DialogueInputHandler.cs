using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class _DialogueInputHandler : MonoBehaviour {
    private Player player;
    private _DialogueHandler dialogueHandler;
    private DialogueButtons dialogueButtons;
    public TextMeshProUGUI promptText, choiceAText, choiceBText;
    private CanvasGroup DialogueInputCanvasGroup;

    void OnEnable() {
        dialogueButtons = GetComponent<DialogueButtons>();
        dialogueHandler = FindObjectOfType<_DialogueHandler>();

        DialogueInputCanvasGroup = GetComponent<CanvasGroup>();

        if (DialogueInputCanvasGroup != null) {
            DialogueInputCanvasGroup.alpha = 0f;
            // DialogueInputCanvasGroup.interactable
            DialogueInputCanvasGroup.blocksRaycasts = false;
        }
    }
    public void ShowChoices(string prompt, string choiceA, string choiceB) {
        DialogueInputCanvasGroup.alpha = 1f; DialogueInputCanvasGroup.blocksRaycasts = true;

        SetupChoiceTypewriter(promptText, prompt);
        SetupChoiceTypewriter(choiceAText, choiceA);
        SetupChoiceTypewriter(choiceBText, choiceB);
    }
    void SetupChoiceTypewriter(TextMeshProUGUI textGameObject, string text) {
        textGameObject.GetComponent<TypeWriter>().StartTypewriter(text);
        textGameObject.GetComponent<TypeWriter>().skipTyping = false;
        textGameObject.GetComponent<TypeWriter>().hasStartedTyping = true;
    }
}
