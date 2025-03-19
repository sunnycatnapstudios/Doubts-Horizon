using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManInventoryDialogue : InventoryDialogues
{
    public override string LowHealthAndLowHunger() {
        return "Not feeling so good.. I might go belly up... Blub.";
    }
    public override string LowHealthDialogue() {
        return "Blub! That last hit nearly scaled me!";
    }
    public override string LowHungerDialogue() {
        return "Blub!! Would you kindly give some snacks?";
    }
    public override string NormalDialogue() {
        return "Blub-blub! Smells like rain... Good day for a swim.";
    }
}
