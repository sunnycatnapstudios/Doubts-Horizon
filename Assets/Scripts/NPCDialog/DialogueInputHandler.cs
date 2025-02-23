using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DialogueInputHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    private Dictionary<string, Action> dialogueChoices;
    private NPCDialogueHandler npcDialogueHandler;
    private TMP_Text dialogueText;
    private bool hovering;
    private string curLinkID;

    public void AddDialogueChoice(string id, Action callBack) {
        Debug.Assert(dialogueChoices != null);
        Debug.Assert(!dialogueChoices.ContainsKey(id));
        dialogueChoices.Add(id, callBack);
    }

    void Awake() {
        dialogueChoices = new Dictionary<string, Action>();
        dialogueText = GetComponent<TMP_Text>();
        npcDialogueHandler = GetComponent<NPCDialogueHandler>();
    }

    void LateUpdate() {
        if (hovering) {
            // Check if mouse intersects with any links. (based on TMP Example 12a)
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(dialogueText, Input.mousePosition, null);
            if (linkIndex == -1) {
                return;
            }
            TMP_LinkInfo linkInfo = dialogueText.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();
            if (linkId != curLinkID) {
                // Can make link react on mouse-over here.
                Debug.Log($"Hovering {linkInfo.GetLinkID()}");
                curLinkID = linkId;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("DialogueInPutHandler OnPointerEnter()");
        hovering = true;
    }


    public void OnPointerExit(PointerEventData eventData) {
        Debug.Log("DialogueInPutHandler OnPointerExit()");
        hovering = false;
        curLinkID = null;
    }

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("DialogueInPutHandler OnPointerClick()");

        int charIndex = TMP_TextUtilities.FindNearestCharacter(dialogueText, Input.mousePosition, null, false);
        if (charIndex != -1) {
            Debug.Log($"nearest to chr {dialogueText.textInfo.characterInfo[charIndex].character} at {charIndex}");
        }

        // Check if mouse intersects with any links. (based on TMP Example 12a)
        Debug.Log($"checking intersection with {dialogueText.text}");
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(dialogueText, Input.mousePosition, null);
        if (linkIndex == -1) {
            return;
        }
        Debug.Log($"linkIndex {linkIndex}");
        TMP_LinkInfo linkInfo = dialogueText.textInfo.linkInfo[linkIndex];
        string linkId = linkInfo.GetLinkID();
        if (dialogueChoices.ContainsKey(linkId)) {
            Debug.Log("Gonna call callback.");
            dialogueChoices[linkId]();
        } else {
            Debug.Log("callabck was null");
        }
    }
}
