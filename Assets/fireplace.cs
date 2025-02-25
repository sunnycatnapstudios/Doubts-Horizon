using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireplace : MonoBehaviour
{

    public List<GameObject> positions;
    public Vector3 positionSpawn;
    private Vector3 originalLoc;
    private bool isActive;
    private GameObject player;


    public Dictionary<string, GameObject> possibleNpcs;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other) {
        
    }

    private void SpawnMembers() {

    }
}
