using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour {
    private readonly string tagTarget = "Player";

    public GameObject transitionAnimator;

    private Animator sceneAnimation;

    public Vector3 entranceDirection, exitDirection, exitLocation, endPosition;
    public Transform targetTransition;

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

            //player.movePoint.transform.position += entranceDirection;
            player.isPlayerInControl = true;
            player.moveSpeed = 3f;

            changedLevel = true;
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
        }

        sceneAnimation.SetTrigger("Enter Scene");



        int i = 0;
        var partyMembers = playerObject.GetComponent<PartyManager>().spawnedPartyMembers;
        foreach (GameObject partyMember in partyMembers) {
            partyMember.transform.position = exitLocation;
            player.moveHist[i] = exitLocation;
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
        if (sceneAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            exitLocation = targetTransition.position; // Temp
            player.movePoint.transform.position = exitLocation;
            playerObject.transform.position = exitLocation;
        }
    }
}
