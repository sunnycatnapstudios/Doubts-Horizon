using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireplace : MonoBehaviour {
    public List<GameObject> objects;
    private Vector3 originalLoc;
    private bool isActive;
    private GameObject player;
    private PartyManager partyManager;
    GameObject[] followers;

    public Dictionary<string, GameObject> possibleNpcs;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        partyManager = player.GetComponent<PartyManager>();
    }

    public GameObject[] getFollowers() {
        return followers;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponentInParent<Player>() != null) {
            StartCoroutine(StartFireplaceEvent());
        }
    }

    public IEnumerator StartFireplaceEvent() {
        GameStatsManager.Instance.resetNpcCounter();
        GameStatsManager.Instance.updateBedStatus();


        SpawnMembers();
        followers = GameObject.FindGameObjectsWithTag("Followers");
        foreach (GameObject follower in followers) {
            //follower.SetActive(false);
            follower.GetComponent<SpriteRenderer>().enabled = false;
        }


        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }

    // TODO need a function to re-enable the transition objects once fireplace event is ended
    // Call in CampfireExitDialogue? Exit campfire transition?
    public IEnumerator EndFireplaceEvent() {
        // TODO
        yield return new WaitForSeconds(2);
        gameObject.SetActive(true);
    }

    private void SpawnMembers() {
        foreach (GameObject npc in objects) {
            Identity identity = npc.GetComponent<Identity>();
            ;
            if (identity != null) {
                Debug.Log(identity.GetSurvivor().GetName());

                Survivor sur = identity.GetSurvivor();
                if (!partyManager.currentPartyMembers.Contains(sur)) {
                    npc.SetActive(false);
                } else {
                    npc.SetActive(true);
                }
            }
        }
    }
}
