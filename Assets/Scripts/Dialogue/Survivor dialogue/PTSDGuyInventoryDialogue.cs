using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTSDGuyInventoryDialogue : InventoryDialogues {
     public override string LowHealthAndLowHunger() {
        return "This is it..";
    }
    public override string LowHealthDialogue() {
        return "I am gonna die!!!";
    }
    public override string LowHungerDialogue() {
        return "My stomach hurts like hell.";
    }
    public override string NormalDialogue() {
        return "Grass hasn't been this soft since I was young.";
    }
}
