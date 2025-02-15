using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    private string tagTarget = "Player";
    // public Scene sceneToLoad;
    public GameObject transitionAnimator;

    private Animator sceneAnimation;

    public Vector3 entranceDirection, exitDirection, exitLocation, endPosition;

    private GameObject Player;
    private Transform playerTransform;
    public Vector3 playerPosition;
    public Vector3 refPosition;
    public bool changedLevel, inTransition;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxEnterTransition;
        public AudioClip sfxExitTransition;
    }
    [SerializeField] private AudioClips audioClips;

    private void OnTriggerEnter2D(Collider2D other) {

        // if (other.CompareTag(tagTarget)&& !inTransition)
        if (other.CompareTag(tagTarget))
        {
            // if (CompareTag("Same Level")||CompareTag("Battle UI Level"))
            {
                Debug.Log(tagTarget + " has entered " + name);

                Player = other.gameObject;
                Player.GetComponent<Player>().movePoint.transform.position += entranceDirection;
                Player.GetComponent<Player>().isPlayerInControl = true;
                Player.GetComponent<Player>().moveSpeed = 3f;

                // inTransition = true;
                sceneAnimation.SetTrigger("Leave Scene");
                AudioManager.Instance.PlaySound(audioClips.sfxEnterTransition);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag(tagTarget))
        {
            // if (CompareTag("Same Level")||CompareTag("Battle UI Level"))
            {
                Debug.Log(tagTarget + " has left "+ name);
                Player.GetComponent<Player>().isPlayerInControl = false;

                AudioManager.Instance.PlaySound(audioClips.sfxExitTransition);
                changedLevel = true;
            }
        }
    }

    void ChangeLevel()
    {
        // if (CompareTag("Same Level") && !inTransition)
        // if (CompareTag("Same Level")||CompareTag("Battle UI Level"))
        {
            sceneAnimation.SetTrigger("Enter Scene");

            Player.GetComponent<Player>().movePoint.transform.position = exitLocation+exitDirection;
            Player.transform.position = exitLocation;

            int i = 0;
            foreach (GameObject partyMember in Player.GetComponent<PartyManager>().spawnedPartyMembers)
            {
                partyMember.transform.position = exitLocation+exitDirection;
                Player.GetComponent<Player>().moveHist[i] = exitLocation+exitDirection; i++;
            }

            Player.GetComponent<Player>().isPlayerInControl = false;

            changedLevel = false;

            // inTransition = true;
        }
    }

    void Start()
    {
        // if (sceneToLoad == null) {
        //     Debug.Log(this.name + "has no scene to load");
        // }
        if (transitionAnimator == null) {
            Debug.Log(this.name + " has no animation to load");
        } else {
            sceneAnimation = transitionAnimator.GetComponent<Animator>();
            // sceneAnimation.SetTrigger("Enter Scene");
        }
    }

    void Update()
    {
        // if (CompareTag("Same Level")||CompareTag("Battle UI Level"))
        {
            if (changedLevel && !sceneAnimation.GetCurrentAnimatorStateInfo(0).IsName("Scene Transition In"))
            {
                ChangeLevel();
            }
        }
        // if (inTransition && sceneAnimation.GetCurrentAnimatorStateInfo(0).IsName("Scene Transition Empty"))
        // {
        //     inTransition = false;
        // }
    }
}
