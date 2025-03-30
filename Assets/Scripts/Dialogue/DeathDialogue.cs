using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDialogue : MonoBehaviour
{
    // Start is called before the first frame update
    private DialogueBoxHandler NPCDialogueHandler;
    public List<string> dialogueLines;
    public Survivor _survivor;
    BattleTransition transition;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
    }

    [SerializeField] private AudioClips audioClips;

    public void setSurvivor(Survivor survivor) {
        this._survivor = survivor;


    }
    public void resetBeforeAndAfterDialogue() {
        NPCDialogueHandler = GetComponent<DialogueBoxHandler>();

        NPCDialogueHandler.afterDialogue = new Action(AfterDialogue);
        NPCDialogueHandler.beforeDialogue = new Action(BeforeDialogue);
    }


    public void setTransition(BattleTransition currbattletrans) {
        transition = currbattletrans;

    }
    void Start() {
        NPCDialogueHandler = GetComponent<DialogueBoxHandler>();
        NPCDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        NPCDialogueHandler.dialogueContents = dialogueLines;
        NPCDialogueHandler.afterDialogue = new Action(AfterDialogue);
        NPCDialogueHandler.beforeDialogue = new Action(BeforeDialogue);
    }

    void Update() {
    }
    void BeforeDialogue() {
        dialogueLines = new List<string>();
        dialogueLines = _survivor.deathDialogue;
        NPCDialogueHandler.dialogueContents = dialogueLines;
        GameStatsManager.Instance._dialogueHandler.dialogueName.text =_survivor.name;
        GameStatsManager.Instance._dialogueHandler.dialogueProfile.sprite = _survivor.Sprite;



    }
    void AfterDialogue() {
        

        GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        StartCoroutine(transition.closeTeammateDeathScreen());
        Debug.Log("call after close hope");
    }

    IEnumerable closeDialogue() {

        yield return new WaitForSecondsRealtime(1);
        transition.closeTeammateDeathScreen();
    }
}
