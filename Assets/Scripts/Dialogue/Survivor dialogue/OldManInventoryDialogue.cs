using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManInventoryDialogue : InventoryDialogues
{
    public override string LowHealthAndLowHunger() {
        return "My bones ain't what they used to be.";
    }
    public override string LowHealthDialogue() {
        return "That last hit nearly sent me to the other side. Got any medicine, kid?";
    }
    public override string LowHungerDialogue() {
        return "Back in the day, we never went this long without a meal.";
    }
    public override string NormalDialogue() {
        return "Reminds me of the good ol’ days, just a little less noisy.";
    }
}
