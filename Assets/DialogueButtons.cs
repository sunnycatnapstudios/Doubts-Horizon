using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButtons : MonoBehaviour
{
    private GameStatsManager gameStatsManager;
    private _DialogueHandler dialogueHandler;

    public bool choiceA, choiceB;

    public void Start()
    {
        gameStatsManager = GameStatsManager.Instance;
        dialogueHandler = gameStatsManager.GetComponentInChildren<_DialogueHandler>();
    }

    public void ChoiceA() {
        Debug.Log("ChoiceA");
        choiceA = true;
        choiceB = false;
        // dialogueHandler.OnChoiceMade("A");
    }
    public void ChoiceB() {
        Debug.Log("ChoiceB");
        choiceA = false;
        choiceB = true;
        // dialogueHandler.OnChoiceMade("B");
    }

    public void ResetChoice() {choiceA = choiceB = false;}
}
