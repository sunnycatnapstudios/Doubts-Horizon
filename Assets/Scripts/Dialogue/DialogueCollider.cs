using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCollider : MonoBehaviour {
    private readonly string tagTarget = "Player";
    private Player player;
    private PartyManager manager;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        manager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
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
