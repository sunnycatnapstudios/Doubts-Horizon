using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using System;

public class Boss : MonoBehaviour
{
    GameStatsManager gameStatsManager;
    _BattleUIHandler _battleUIHandler;
    public EnemyObjectManager enemyObjectManager;
    DialogueBoxHandler dialogueHandler;


    private readonly string tagTarget = "Player";
    private Player player;
    private PartyManager manager;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
        public AudioClip musicBossBattle;
    }

    [SerializeField] private AudioClips audioClips;

    void Start() {
        gameStatsManager = GameStatsManager.Instance;
        _battleUIHandler = GameStatsManager.Instance._battleUIHandler;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        dialogueHandler = GetComponent<DialogueBoxHandler>();
        manager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
        dialogueHandler.dialogueContents = new List<string> {
            "BEEP(Windows XP sound plays)",
            "HUMAN DETECTED; PROTOCOL INITIATED...",
            "TERMINATE ALL LIFEFORMS!!!"

        };
        dialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);
        dialogueHandler.afterDialogue =new Action(AfterDialogue);

    }

    void AfterDialogue() {
        GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
        _battleUIHandler.curEnemy = gameObject;
        _battleUIHandler.enemyObjectManager = enemyObjectManager;
        StartCoroutine(destroySelf());
        _battleUIHandler.SetBossBattleMusic(audioClips.musicBossBattle);
        _battleUIHandler.EnterCombat();


    }
    IEnumerator destroySelf() {
        yield return new WaitForSecondsRealtime(2);
        Destroy(gameObject);
    }


    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(tagTarget)) {
            if (player == null) {
                GameObject playerObject = other.gameObject;
                player = other.gameObject.GetComponent<Player>();
            }
            Debug.Log("Hit Dialogue Collider.");
            GameStatsManager.Instance._dialogueHandler.OpenDialogueWith(gameObject);
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
}
