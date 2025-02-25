using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]


public class Key : Item
{
    public Pickupable key;
    public Sprite sprite;

    public override string GetDesc()
    {
        return "Can be used to open a door.";
    }

    public override string GetFlavour() 
    {
        return "A rusted key. May fall apart at any moment.";
    }

    public override string GetName()
    {
        return "Key";
    }

    public override Sprite GetSprite()
    {
        return sprite;
    }

    public override bool UsableInInventory() => false;

    public override void Use()
    {
       
    }
    // Start is called before the first frame update

}
