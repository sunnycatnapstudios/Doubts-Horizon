using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour {
    private readonly string tagTarget = "Player";

    public GameObject transitionAnimator;

    private Animator sceneAnimation;

    public Vector3 entranceDirection, exitDirection, exitLocation, endPosition;

    private GameObject playerObject;
    private Player player;

    public bool changedLevel;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxEnterTransition;
        public AudioClip sfxExitTransition;
    }

    [SerializeField] private AudioClips audioClips;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(tagTarget)) {
            if (player == null) {
                playerObject = other.gameObject;
                player = other.gameObject.GetComponent<Player>();
            }

            player.movePoint.transform.position += entranceDirection;
            player.isPlayerInControl = true;
            player.moveSpeed = 3f;

            // inTransition = true;
            sceneAnimation.SetTrigger("Leave Scene");
            AudioManager.Instance.PlaySound(audioClips.sfxEnterTransition);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag(tagTarget)) {
            if (player == null) {
                playerObject = other.gameObject;
                player = other.gameObject.GetComponent<Player>();
            }

            player.isPlayerInControl = false;
            AudioManager.Instance.PlaySound(audioClips.sfxExitTransition);
            changedLevel = true;
        }
    }

    void ChangeLevel() {
        sceneAnimation.SetTrigger("Enter Scene");

        player.movePoint.transform.position = exitLocation + exitDirection;
        playerObject.transform.position = exitLocation;

        int i = 0;
        var partyMembers = playerObject.GetComponent<PartyManager>().spawnedPartyMembers;
        foreach (GameObject partyMember in partyMembers) {
            partyMember.transform.position = exitLocation + exitDirection;
            player.moveHist[i] = exitLocation + exitDirection;
            i++;
        }

        player.isPlayerInControl = false;

        changedLevel = false;
    }

    void Start() {
        if (transitionAnimator == null) {
            Debug.Log(this.name + " has no animation to load");
        } else {
            sceneAnimation = transitionAnimator.GetComponent<Animator>();
        }
    }

    void Update() {
        if (changedLevel && !sceneAnimation.GetCurrentAnimatorStateInfo(0).IsName("Scene Transition In")) {
            ChangeLevel();
        }
    }
}
