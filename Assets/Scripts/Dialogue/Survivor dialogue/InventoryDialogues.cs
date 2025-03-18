using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryDialogues : MonoBehaviour 
{
     public abstract string NormalDialogue();

    public abstract string LowHealthAndLowHunger();

    public abstract string LowHealthDialogue();

    public abstract string LowHungerDialogue();



}
