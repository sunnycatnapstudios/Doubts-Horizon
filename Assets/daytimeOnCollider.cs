using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class daytimeOnCollider : MonoBehaviour
{

    private readonly string tagTarget = "Player";
    private Player player;
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


   void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("HEERRREEE");
        if (other.CompareTag(tagTarget)) {
            if (player == null) {

                GameObject playerObject = other.gameObject;
                player = other.gameObject.GetComponent<Player>();
            }
            GameStatsManager.Instance.nightFilter.SetActive(false);
            Debug.Log("HEERRREEE");
        }


    }
}
