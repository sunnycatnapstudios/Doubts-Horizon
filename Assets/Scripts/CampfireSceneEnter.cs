using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireSceneEnter : MonoBehaviour
{

    public List<Vector3> locations = new List<Vector3>();
    GameStatsManager statsManager;
    PartyManager partyManager;

    // Start is called before the first frame update
    void Start() {
        statsManager = GameStatsManager.Instance;
        partyManager = statsManager.partyManager;
        foreach (Survivor survivor in partyManager.currentPartyMembers) {




        }







    }
}
