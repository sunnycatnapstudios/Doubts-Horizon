using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireplace : MonoBehaviour
{

    public List<GameObject> objects;
    public Vector3 positionSpawn;
    private Vector3 originalLoc;
    private bool isActive;
    private GameObject player;
    private PartyManager partyManager;


    public Dictionary<string, GameObject> possibleNpcs;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        partyManager = player.GetComponent<PartyManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other) {
        
        SpawnMembers();
        
    }

    private void SpawnMembers() {
        foreach (GameObject npc in objects) {
            Identity identity = npc.GetComponent<Identity>(); ;
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
