using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour {
    private readonly string tagTarget = "Player";

    public GameObject transitionAnimator;

    private Animator sceneAnimation;

    public Transform targetTransition;      // A transform under the transition to target where to place the player
    private Vector3 exitLocation;

    private GameObject playerObject;
    private Player player;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxEnterTransition;
        public AudioClip sfxExitTransition;
    }

    [SerializeField] private AudioClips audioClips;

    private IEnumerator OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(tagTarget)) {
            if (player == null) {
                playerObject = other.gameObject;
                player = other.gameObject.GetComponent<Player>();
            }

            player.isPlayerInControl = true;

            sceneAnimation.SetTrigger("Leave Scene");
            AudioManager.Instance.PlaySound(audioClips.sfxEnterTransition);

            yield return new WaitForSeconds(sceneAnimation.GetCurrentAnimatorStateInfo(0).length);

            // We wait for the transition to complete before moving the player and party
            player.movePoint.transform.position = exitLocation;
            playerObject.transform.position = exitLocation;

            int i = 0;
            var partyMembers = playerObject.GetComponent<PartyManager>().spawnedPartyMembers;
            foreach (GameObject partyMember in partyMembers) {
                partyMember.transform.position = exitLocation;
                player.moveHist[i] = exitLocation;
                i++;
            }

            // Wait a second before playing the exit transition
            yield return new WaitForSeconds(1);

            sceneAnimation.SetTrigger("Enter Scene");
            AudioManager.Instance.PlaySound(audioClips.sfxExitTransition);

            player.isPlayerInControl = false;
        }
    }

    void Start() {
        exitLocation = targetTransition.position;
        if (transitionAnimator == null) {
            Debug.Log(this.name + " has no animation to load");
        } else {
            sceneAnimation = transitionAnimator.GetComponent<Animator>();
        }
    }
}
