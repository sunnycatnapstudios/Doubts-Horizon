﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ration : Item{
    public Pickupable ration;
    public Sprite Sprite;
    public AudioClip sfxPickup;

    public override string GetDesc() {
        return "enough food for one person.";
    }

    public override string GetFlavour() {
        return "donair poutine";
    }

    public override string GetName() {
        return "Ration";
    }

    public override Sprite GetSprite() {
        return Sprite;
    }

    public override void Use()
    {
        
    }
    public override bool UsableInInventory() => false;
    //public void UseOnMember(Survivor survivor) {
    //    survivor.Fed = true;

    //}
    // Start is called before the first frame update

    

    public override AudioClip GetPickupSound() {
        return sfxPickup;
    }
}
