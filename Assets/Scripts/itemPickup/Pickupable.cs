﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    private bool pickUp=false;
    public Item item;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collison)
    {
        if (collison.gameObject.tag == "Player")
        {
            
            pickUp = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collison)
    {
        if (collison.gameObject.tag == "Player")
        {
            pickUp = false;
        }

    }
    public Item GetItem() { return item; }
    public void DestroyInWorld()
    {
        Destroy(gameObject);
    }

}
