using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTSDGuyInventoryDialogue : InventoryDialogues {
     public override string LowHealthAndLowHunger() {
        return "Hey... Can we wait a bit, i think i need to catch my breath";
    }
    public override string LowHealthDialogue() {
        return "Owch, do we have any potions left? i think i broke something";
    }
    public override string LowHungerDialogue() {
        return "I could use a burger dawg";
    }
    public override string NormalDialogue() {
        return "Woah the sun is so bright today";
    }
}
