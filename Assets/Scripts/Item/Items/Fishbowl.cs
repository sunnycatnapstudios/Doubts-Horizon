using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Fishbowl : Item {
    public Pickupable bowl;
    public Sprite sprite;
    public AudioClip sfxPickup;

    public override string GetDesc() {
        return "lots of algae";
    }

    public override string GetFlavour() {
        return "I wouldnt drink this if it was the last fresh water on earth";
    }

    public override string GetName() {
        return "Fishbowl";
    }

    public override Sprite GetSprite() {
        return sprite;
    }

    public override bool UsableInInventory() => false;

    public override void Use() {
    }

    public override AudioClip GetPickupSound() {
        return sfxPickup;
    }
}
