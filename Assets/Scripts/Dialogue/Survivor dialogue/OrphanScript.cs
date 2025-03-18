using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrphanScript : InventoryDialogues {
    public override string LowHealthAndLowHunger() {
        return "Mom? is that you?";
    }
    public override string LowHealthDialogue() {
        return "Wahhh Where is my mom :(";
    }
    public override string LowHungerDialogue() {
        return "Can we wait a bit? I am so hungry";
    }
    public override string NormalDialogue() {
        return "I want to go home...";
    }
}

