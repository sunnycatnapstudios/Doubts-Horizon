using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]

public class healingPotion : Item,UsableInInventory
{
    public Pickupable _healingPotion;
    public Sprite Sprite;
    public AudioClip sfxPickup;


    public override string GetDesc() {
        return "red 50";

    }
    public override string GetFlavour() {
        return "not sure what this is";
    }

    public override string GetName() {
        return "Potion of healing";
    }

    public override Sprite GetSprite() {
        return Sprite;
    }

    public override void Use() {

    }
    public override bool UsableInInventory() => true;
    public void UseOnMember(Survivor survivor) {
        survivor.AddHealth(100);
        Debug.Log(survivor.CurHealth);
        

    }

    public override AudioClip GetPickupSound() {
        return sfxPickup;
    }
}
