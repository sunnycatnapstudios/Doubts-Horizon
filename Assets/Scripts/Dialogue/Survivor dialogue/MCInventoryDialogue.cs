using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCInventoryDialogue : InventoryDialogues
{
    public override string LowHealthAndLowHunger() {
        return "I cannot die!!";
    }
    public override string LowHealthDialogue() {
        return "Better patch myself up fast";
    }
    public override string LowHungerDialogue() {
        return "Was that sound my stomach";
    }
    public override string NormalDialogue() {
        return "Hopefully there is less trouble today.";
    }
}
