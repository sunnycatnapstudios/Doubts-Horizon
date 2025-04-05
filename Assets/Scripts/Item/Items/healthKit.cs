using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]

public class healthKit : Item,UsableInInventory
{
    public Pickupable _healthKit;
    public Sprite Sprite;
    public AudioClip sfxPickup;


    public override string GetDesc() {
        return "heal a member";

    }
    public override string GetFlavour() {
        return "a kit full of old medical supplies...";
    }

    public override string GetName() {
        return "Health Kit";
    }

    public override Sprite GetSprite() {
        return Sprite;
    }

    public override void Use() {

    }
    public override bool UsableInInventory() => true;
    public void UseOnMember(Survivor survivor) {
        int healAmount = survivor.maxHealth / 2;
        survivor.AddHealth(healAmount);
    }

    public override AudioClip GetPickupSound() {
        return sfxPickup;
    }
}
