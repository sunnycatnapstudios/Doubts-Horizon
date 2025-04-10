using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]

public class Knife : Item, UsableInInventory {
    public Pickupable _healingPotion;
    public Sprite Sprite;
    public AudioClip sfxPickup;
    public override string GetDesc() => "member gain permanent 10 attack";
    public override string GetFlavour() => "I think it has been used";
    public override string GetName() => "Knife";
    public override AudioClip GetPickupSound() => sfxPickup;
    public override Sprite GetSprite() => Sprite;
    public override bool UsableInInventory() => true;
    public override void Use() { }
    public void UseOnMember(Survivor member) {

        member.AddDamage(10);
        Debug.Log(member.Damage);

    }
}
